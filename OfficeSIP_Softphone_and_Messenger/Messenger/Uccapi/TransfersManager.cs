// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace Uccapi
{
    public static class TransferCommands
    {
        public const string TransferRequest = @"TRQ";    // TRQ:TransfersNumber:TransferID:FileName:FileSize:MD5:...
        public const string ReadyToAccept = @"RAC";      // RAC:TransferID
        public const string DataFrameTansfer = @"DFT";   // DFT:TransferID:Data
        public const string GotDataFrame = @"GDF";       // GDF:TransferID
        public const string StopTransfer = @"STP";       // STP:StoppedTransfersState:TransfersNumber:TransferID:...
        public const string TestMode = @"TST";           // TST
    }

    /*     Sender--->(TRQ:TransfersNumber:TransferID:FileName:FileSize:MD5:...)--->Receiver--
     *                                                                                      |
     *        (STP:RejectedByUser:TransfersNumber:TransferID:...)(<---(No)<----(Did user accept transfers?)
     *                                                                                      |
     *                                                                                      V
     *                                                                                    (Yes)
     *                                                                                      |
     *                                                                                      V
     *                                                                                      |
     *                                                          /(RAC:TransferID_1)<--------| 
     *                                                          |(RAC:TransferID_2)<--------|
     *                                                          |...........................|
     *     Sender<---------------------------------------------{(RAC:TransferID_n)<---------|
     *     |
     *     (if have sent whole file, finalize transfer. Switch to next accepted transfer)
     *     |
     *     ------>(DFT:TransferID_1:Data)------------------------------------------>Receiver-->(if got whole file, finalize transfer)
     *     |     ^                                                                          |
     *     |     |                                                                          |
     *     |     |<---------------------(GDF:TransferID_1)<----------------------------------
     *     |
     *     |
     *     (*** switching to the next accepted transfer ***)
     *     |
     *     (if have sent whole file, finalize transfer. Switch to next accepted transfer)
     *     |
     *     ------>(DFT:TransferID_2:Data)------------------------------------------>Receiver-->(if got whole file, finalize transfer)
     *     |     ^                                                                          |
     *     |     |                                                                          |
     *     |     |<---------------------(GDF:TransferID_2)<----------------------------------
     *     |
     *     |
     *     (*** switching to the next accepted transfer ***)
     *     .....
     *     |
     *     (if have sent whole file, finalize transfer. Switch to next accepted transfer)
     *     |
     *     ------>(DFT:TransferID_n:Data)------------------------------------------>Receiver-->(if got whole file, finalize transfer)
     *     |     ^                                                                          |
     *     |     |                                                                          |
     *     |     |<---------------------(GDF:TransferID_n)<----------------------------------
     *     |
     *     |
     *     (all transfers sent)
     */

    public class TransfersManagerEventArgs : EventArgs
    {
        public TransfersManagerEventArgs(IEnumerable<ITransferItem> items, string fromUri)
        {
            Items = items;
			FromUri = fromUri;
        }

        public IEnumerable<ITransferItem> Items { get; private set; }
        public string FromUri { get; private set; }
    }

    public class TransferErrorEventArgs : TransfersManagerEventArgs
    {
        public TransferErrorEventArgs(IEnumerable<ITransferItem> items, string fromUri, FileTransferState itemsState, bool isLocal)
            : base(items, fromUri)
        {
            this.ItemsState = itemsState;
            this.IsLocal = isLocal;
        }

        public FileTransferState ItemsState { get; private set; }
        public bool IsLocal { get; private set; }
    }

    public class TransfersManager
        : ITransfersManager
    {
        private const int DATA_FRAME_SIZE = 30000;
        private const int COMMAND_LENGTH = 3;
        private const string PROTOCOL_V1_MARK = "OFSV1";
        private const string TRANSFERS_COMMAND_SEPARATOR = ":";
        private Dictionary<string, FileStream> fileStreams = new Dictionary<string, FileStream>();
        private IImSession session;
        private TransfersState outgoing;
        private TransfersState incoming;

        public event EventHandler<TransfersManagerEventArgs> OutgoingTransfer;
        public event EventHandler<TransfersManagerEventArgs> IncomingTransferRequest;
        public event EventHandler<TransfersManagerEventArgs> TransferEnded;
        public event EventHandler<TransferErrorEventArgs> TransferError;

        public TransfersManager(IImSession session1)
        {
            session = session1;
            incoming = new TransfersState();
            outgoing = new TransfersState();
            session.PartipantLogs.CollectionChanged += ParticipantsWatcher;
        }

        private void ParticipantsWatcher(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ParticipantLog NewItem in e.NewItems)
                    if (NewItem.IsLocal == false)
                        NewItem.PropertyChanged += ConnectionStateWatcher;
        }

        private void ConnectionStateWatcher(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == @"State")
                if ((sender as ParticipantLog).State == PartipantLogState.SessionTerminated)
                    ClearAllTransfers(FileTransferState.StoppedByError);
        }

        public ITransfersState Incoming
        {
            get { return incoming; }
        }

        public ITransfersState Outgoing
        {
            get { return outgoing; }
        }

        private void OnIncomingMessage(Object sender, ImSessionEventArgs2 e)
        {
            if (e.Message.ContentType == MessageContentType.FileData)
                ProcessTransferMessage(e.Message.Message, e.Message.FromUri);
        }

        public void ProcessTransferMessage(string message, string fromURI)
        {
            if (message.Substring(0, PROTOCOL_V1_MARK.Length) != PROTOCOL_V1_MARK)
                return;

            switch (message.Substring(PROTOCOL_V1_MARK.Length, COMMAND_LENGTH))
            {
                case TransferCommands.TransferRequest:
                    ProcessIncomingTransfer(message.Substring(PROTOCOL_V1_MARK.Length + COMMAND_LENGTH + 1), fromURI);
                    break;
                case TransferCommands.DataFrameTansfer:
                    ProcessIncomingDataFrame(message.Substring(PROTOCOL_V1_MARK.Length + COMMAND_LENGTH + 1));
                    break;
                case TransferCommands.ReadyToAccept:
                    StartOutgoingTransfer(message.Substring(PROTOCOL_V1_MARK.Length + COMMAND_LENGTH + 1), fromURI);
                    break;
                case TransferCommands.GotDataFrame:
                    ProcessContinueTransfer(message.Substring(PROTOCOL_V1_MARK.Length + COMMAND_LENGTH + 1));
                    break;
                case TransferCommands.StopTransfer:
                    ProcessStopCommand(message.Substring(PROTOCOL_V1_MARK.Length + COMMAND_LENGTH + 1));
                    break;
                case TransferCommands.TestMode:
#if DEBUG
                    IncomingTransferRequest = null;
                    TransferError = null;
                    TransferEnded = null;
                    if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSetIncoming")))
                        Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSetIncoming"));
                    IncomingTransferRequest += new EventHandler<TransfersManagerEventArgs>(OnTestIncomingTransferRequest);
#endif
                    break;
            }
        }

        private void OnTestIncomingTransferRequest(object sender, TransfersManagerEventArgs e)
        {
            TransfersManager transfersManager = (TransfersManager)sender;
            transfersManager.Accept(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSetIncoming\\"), e.Items);
        }

        // STP:StoppedTransfersState:TransfersNumber:TransferID:...
        private void ProcessStopCommand(string commandText)
        {
            int c;
            FileTransferState newState;
            TransferItem fileTransfer;
            List<ITransferItem> stoppedTransfers = new List<ITransferItem>();

            string[] commandParts = commandText.Split(TRANSFERS_COMMAND_SEPARATOR.ToCharArray());
            newState = (FileTransferState)Int32.Parse(commandParts[0]);

            for (c = 0; c < Int32.Parse(commandParts[1]); c++)
            {
                fileTransfer = outgoing.GetTransferItem(commandParts[c + 2]);

                if (fileTransfer != null)
                {
                    fileTransfer.State = newState;
                    if (newState == FileTransferState.RejectedRemote)
                        outgoing.MaxIndex--;
                    if (fileStreams.ContainsKey(fileTransfer.TrId) && fileStreams[fileTransfer.TrId] != null)
                        outgoing.TransferedBytes += fileTransfer.FileSize - fileStreams[fileTransfer.TrId].Position;
                    stoppedTransfers.Add(fileTransfer);
                }
            }

            if (stoppedTransfers.Count == 0)
                return;

            OnTransferError(new TransferErrorEventArgs(stoppedTransfers, stoppedTransfers[0].OtherSideURI, newState, false));

            foreach (ITransferItem currentTransfer in stoppedTransfers)
                FinalizeTransfer(currentTransfer.TrId, newState);
            SwitchToActiveTransfer();
        }

        // GDF:TransferID
        private void ProcessContinueTransfer(string trID)
        {
            int readBytes;
            byte[] buf;
            string base64Buf;

            TransferItem fileTransfer = outgoing.GetTransferItem(trID);
            if (fileTransfer == null)
            {
                StopTransfer(trID, FileTransferState.StoppedByError, true);
                return;
            }

            outgoing.TransferedBytes += fileStreams[fileTransfer.TrId].Position - fileTransfer.TransferedBytes;
            fileTransfer.TransferedBytes = fileStreams[fileTransfer.TrId].Position;

            if (fileStreams[fileTransfer.TrId].Position < fileStreams[fileTransfer.TrId].Length)
            {
                buf = new byte[DATA_FRAME_SIZE];
                try
                {
                    readBytes = fileStreams[fileTransfer.TrId].Read(buf, 0, buf.Length);
                }
                catch (IOException)
                {
                    StopTransfer(fileTransfer.TrId, FileTransferState.StoppedByError, true);
                    SwitchToActiveTransfer();
                    return;
                }

                base64Buf = Convert.ToBase64String(buf, 0, readBytes);

                // DFT:TransferID:Data
                SendData(TransferCommands.DataFrameTansfer + TRANSFERS_COMMAND_SEPARATOR +
                    fileTransfer.TrId.ToString() + TRANSFERS_COMMAND_SEPARATOR + base64Buf);
                outgoing.CurrentFile = fileTransfer;
            }
            else
            {
                FinalizeTransfer(fileTransfer, FileTransferState.SuccessfullyEnded);
                SwitchToActiveTransfer();
            }
        }

        private void SwitchToActiveTransfer()
        {
            //* If we already have an active transfer, do nothing.
            TransferItem tempItem = outgoing.Files.FirstOrDefault(x => x.State == FileTransferState.Active) as TransferItem;
            if (tempItem != null)
                return;

            tempItem = outgoing.Files.FirstOrDefault(x => x.State == FileTransferState.Accepted) as TransferItem;

            if (tempItem != null)
            {
                tempItem.State = FileTransferState.Active;
                outgoing.CurrentIndex++;
                ProcessContinueTransfer(tempItem.TrId);
            }
        }

        // RAC:TransferID
        private void StartOutgoingTransfer(string commandText, string receiverURI)
        {
            string[] commandParts = commandText.Split(TRANSFERS_COMMAND_SEPARATOR.ToCharArray());
            TransferItem fileTransfer = outgoing.GetTransferItem(commandParts[0]);
            fileStreams.Add(fileTransfer.TrId, new FileStream(fileTransfer.FilePath, FileMode.Open, FileAccess.Read));
            fileTransfer.State = FileTransferState.Accepted;
            fileTransfer.OtherSideURI = receiverURI;
            outgoing.TotalBytes += fileTransfer.FileSize;
            if (outgoing.Files.FirstOrDefault(x => x.State == FileTransferState.Active) == null)
            {
                fileTransfer.State = FileTransferState.Active;
                ProcessContinueTransfer(fileTransfer.TrId);
            }
        }

        public void Stop(IEnumerable<ITransferItem> stoppingItems)
        {
            if (stoppingItems == null || stoppingItems.Count() == 0)
                return;

            string bufMessage = TransferCommands.StopTransfer +
                    TRANSFERS_COMMAND_SEPARATOR + (int)FileTransferState.StoppedByUser +
                    TRANSFERS_COMMAND_SEPARATOR + stoppingItems.Count() + TRANSFERS_COMMAND_SEPARATOR;

            OnTransferError(new TransferErrorEventArgs(stoppingItems, stoppingItems.First().OtherSideURI, FileTransferState.StoppedByUser, true));

            while (stoppingItems.Count() > 0)
            {
                if (fileStreams.ContainsKey(stoppingItems.First().TrId) && fileStreams[stoppingItems.First().TrId] != null)
                    incoming.TransferedBytes += stoppingItems.First().FileSize - fileStreams[stoppingItems.First().TrId].Position;
                bufMessage += stoppingItems.First().TrId + TRANSFERS_COMMAND_SEPARATOR;
                incoming.CurrentIndex++;
                FinalizeTransfer(stoppingItems.First().TrId, FileTransferState.StoppedByUser);
            }
            session.Send(MessageContentType.FileData, bufMessage);
        }

        public void Stop(ITransferItem stoppingItem)
        {
            StopTransfer(stoppingItem.TrId, FileTransferState.StoppedByUser, false);
        }

        private void StopTransfer(string trID, FileTransferState newState, bool isOutgoing)
        {
            TransferItem fileTransfer;

            SendData(TransferCommands.StopTransfer + TRANSFERS_COMMAND_SEPARATOR +
               (int)newState + TRANSFERS_COMMAND_SEPARATOR +
               "1" + TRANSFERS_COMMAND_SEPARATOR + trID + TRANSFERS_COMMAND_SEPARATOR);

            fileTransfer = isOutgoing ? outgoing.GetTransferItem(trID) : incoming.GetTransferItem(trID);

            if (fileTransfer != null)
            {
                List<ITransferItem> tempList = new List<ITransferItem>();
                tempList.Add(fileTransfer);
                if (fileStreams.ContainsKey(fileTransfer.TrId) && fileStreams[fileTransfer.TrId] != null)
                    incoming.TransferedBytes += fileTransfer.FileSize - fileStreams[fileTransfer.TrId].Position;
                OnTransferError(new TransferErrorEventArgs(tempList, fileTransfer.OtherSideURI, newState, !isOutgoing));

                if (isOutgoing)
                    outgoing.CurrentIndex++;
                else
                    incoming.CurrentIndex++;

                FinalizeTransfer(fileTransfer, newState);
            }
        }

        private void FinalizeTransfer(string trId, FileTransferState newState)
        {
            TransferItem finalizingTransfer =
                incoming.GetTransferItem(trId) == null ? outgoing.GetTransferItem(trId) : incoming.GetTransferItem(trId);
            FinalizeTransfer(finalizingTransfer, newState);
        }

        private void FinalizeTransfer(TransferItem finalizingTransfer, FileTransferState newState)
        {
            if (finalizingTransfer == null)
                return;

            if (finalizingTransfer.State != newState)
                finalizingTransfer.State = newState;

            if (fileStreams.ContainsKey(finalizingTransfer.TrId) && fileStreams[finalizingTransfer.TrId] != null)
                fileStreams[finalizingTransfer.TrId].Close();

            fileStreams.Remove(finalizingTransfer.TrId);

            incoming.RemoveTransferItem(finalizingTransfer);
            outgoing.RemoveTransferItem(finalizingTransfer);

            if (incoming.Files.Count == 0)
                incoming.Initialize();

            if (outgoing.Files.Count == 0)
                outgoing.Initialize();

            List<ITransferItem> tempList = new List<ITransferItem>();
            tempList.Add(finalizingTransfer);
            OnTransferEnded(new TransfersManagerEventArgs(tempList, finalizingTransfer.OtherSideURI));
        }

        // GDF:TransferID
        private void SendAcknowledgement(string trID)
        {
            SendData(TransferCommands.GotDataFrame + TRANSFERS_COMMAND_SEPARATOR + trID);
        }

        // TRQ:TransfersNumber:TransferID:FileName:FileSize:MD5:...
        private void ProcessIncomingTransfer(string commandText, string fromURI)
        {
            int c;
            List<ITransferItem> requestedFiles = new List<ITransferItem>();
            string[] commandParts = commandText.Split(TRANSFERS_COMMAND_SEPARATOR.ToCharArray());

            for (c = 0; c < Int32.Parse(commandParts[0]); c++)
            {
                TransferItem newTransfer = new TransferItem(session);

                newTransfer.State = FileTransferState.Requested;
                newTransfer.TrId = commandParts[c * 4 + 1];
                newTransfer.FileName = commandParts[c * 4 + 2];
                newTransfer.FileSize = Int32.Parse(commandParts[c * 4 + 3]);
                newTransfer.FileMD5 = commandParts[c * 4 + 4];
                newTransfer.FilePath = newTransfer.FileName;
                newTransfer.OtherSideURI = fromURI;
                fileStreams[newTransfer.TrId] = null;
                incoming.AddTransferItem(newTransfer);
                requestedFiles.Add(newTransfer);
            }
            OnIncomingTransferRequest(new TransfersManagerEventArgs(requestedFiles, fromURI));

        }

        public void Accept(string path, ITransferItem item)
        {
            List<ITransferItem> tempList = new List<ITransferItem>();
            tempList.Add(item);
            Accept(path, tempList);
        }

        public void Reject(ITransferItem item)
        {
            List<ITransferItem> tempList = new List<ITransferItem>();
            tempList.Add(item);
            Reject(tempList);
        }

        public void Accept(string savingPath, IEnumerable<ITransferItem> items)
        {
            foreach (TransferItem requestedTransfer in items)
            {
                if (Helpers.CheckFileAvailability(Path.Combine(savingPath, requestedTransfer.FileName), true))
                {
                    requestedTransfer.FilePath = Path.Combine(savingPath, requestedTransfer.FileName);
                    requestedTransfer.State = FileTransferState.Accepted;
                    fileStreams[requestedTransfer.TrId] = new FileStream(requestedTransfer.FilePath, FileMode.Create, FileAccess.Write);
                    incoming.TotalBytes += requestedTransfer.FileSize;
                    SendData(TransferCommands.ReadyToAccept + TRANSFERS_COMMAND_SEPARATOR + requestedTransfer.TrId);
                }
            }

            while (items.FirstOrDefault(x => x.FileSize == 0 && x.State == FileTransferState.Accepted) != null)
                FinalizeTransfer(items.FirstOrDefault(x => x.FileSize == 0 && x.State == FileTransferState.Accepted).TrId,
                    FileTransferState.SuccessfullyEnded);

            while (items.FirstOrDefault(x => fileStreams[x.TrId] == null) != null)
                StopTransfer(items.FirstOrDefault(x => fileStreams[x.TrId] == null).TrId,
                    FileTransferState.StoppedByError, false);
        }

        public void Send(string[] filesList)
        {
            string tempMD5;
            FileInfo fi;
            TransferItem tempItem;
            List<ITransferItem> errorList = new List<ITransferItem>();
            List<ITransferItem> sentList = new List<ITransferItem>();

            var remoteReceiver = this.session.ParticipantLogs.FirstOrDefault(x => x.IsLocal == false);
            string otherSideUri = "";
            if (remoteReceiver != null)
                otherSideUri = remoteReceiver.Uri;

            string bufRequestCommand = TransferCommands.TransferRequest + TRANSFERS_COMMAND_SEPARATOR +
                filesList.Length + TRANSFERS_COMMAND_SEPARATOR;

            foreach (string filePath in filesList)
            {
                tempItem = new TransferItem(this.session);
                tempItem.TrId = Guid.NewGuid().ToString();
                tempItem.FileName = System.IO.Path.GetFileName(filePath);
                tempItem.FilePath = filePath;
                tempItem.OtherSideURI = otherSideUri;
                outgoing.AddTransferItem(tempItem);

                if (!Helpers.CheckFileAvailability(filePath, false))
                {
                    errorList.Add(tempItem);
                    continue;
                }

                tempMD5 = Helpers.GetMD5HashFromFile(filePath);
                fi = new FileInfo(filePath);
                tempItem.FileSize = fi.Length;
                tempItem.FileMD5 = tempMD5;

                bufRequestCommand += tempItem.TrId + TRANSFERS_COMMAND_SEPARATOR + tempItem.FileName +
                    TRANSFERS_COMMAND_SEPARATOR + tempItem.FileSize + TRANSFERS_COMMAND_SEPARATOR +
                    tempItem.FileMD5 + TRANSFERS_COMMAND_SEPARATOR;
                sentList.Add(tempItem);
            }

            if (errorList.Count > 0)
            {
                OnTransferError(new TransferErrorEventArgs(errorList, errorList.First().OtherSideURI, FileTransferState.ErrorDuringCreation, true));

                foreach (ITransferItem ErrorItem in errorList)
                    FinalizeTransfer(ErrorItem.TrId, FileTransferState.ErrorDuringCreation);
                outgoing.MaxIndex--;  
            }

            if (errorList.Count == filesList.Count())
                bufRequestCommand = "";

            if (!string.IsNullOrEmpty(bufRequestCommand))
            {
                SendData(bufRequestCommand);

                if (OutgoingTransfer != null)
                    OutgoingTransfer(this, new TransfersManagerEventArgs(sentList, otherSideUri));
            }
        }

        public void Reject(IEnumerable<ITransferItem> rejectedItems)
        {
            if (rejectedItems == null)
                return;

            string bufMessage = TransferCommands.StopTransfer +
                    TRANSFERS_COMMAND_SEPARATOR + (int)FileTransferState.RejectedRemote +
                    TRANSFERS_COMMAND_SEPARATOR + rejectedItems.Count() + TRANSFERS_COMMAND_SEPARATOR;

            foreach (ITransferItem rejectedItem in rejectedItems)
            {
                bufMessage += rejectedItem.TrId + TRANSFERS_COMMAND_SEPARATOR;
                incoming.MaxIndex--;
                incoming.CurrentIndex--;
                StopTransfer(rejectedItem.TrId, FileTransferState.RejectedRemote, false);
            }
            SendData(bufMessage);
        }

        // DFT:TransferID:Data
        private void ProcessIncomingDataFrame(string commandText)
        {
            byte[] buf;
            string[] commandParts = commandText.Split(TRANSFERS_COMMAND_SEPARATOR.ToCharArray());
            TransferItem fileTransfer = incoming.GetTransferItem(commandParts[0]);

            if (fileTransfer == null)
            {
                StopTransfer(commandParts[0], FileTransferState.StoppedByError, false);
                return;
            }

            try
            {
                buf = Convert.FromBase64String(commandParts[1]);
                fileStreams[fileTransfer.TrId].Write(buf, 0, buf.Length);
                fileStreams[fileTransfer.TrId].Flush();
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is FormatException || ex is IOException || ex is ObjectDisposedException)
                {
                    StopTransfer(fileTransfer.TrId, FileTransferState.StoppedByError, false);
                    return;
                }
                else
                    throw;
            }

            incoming.CurrentFile = fileTransfer;
            fileTransfer.TransferedBytes += buf.Length;
            incoming.TransferedBytes += buf.Length;
            SendAcknowledgement(fileTransfer.TrId);

            if (fileTransfer.TransferedBytes >= fileTransfer.FileSize)
            {
                fileStreams[fileTransfer.TrId].Close();
                if (incoming.CurrentIndex != incoming.MaxIndex)
                    incoming.CurrentIndex++;

                if (Helpers.GetMD5HashFromFile(fileTransfer.FilePath) != fileTransfer.FileMD5)
                    StopTransfer(fileTransfer.TrId, FileTransferState.Corrupted, false);
                else
                    FinalizeTransfer(fileTransfer, fileTransfer.State = FileTransferState.SuccessfullyEnded);
            }
        }

        public void ClearAllTransfers(FileTransferState clearingState)
        {
            while (incoming.Files.Count > 0)
                FinalizeTransfer(Incoming.Files[0].TrId, clearingState);

            while (outgoing.Files.Count > 0)
                FinalizeTransfer(outgoing.Files[0].TrId, clearingState);
        }

        public void StartTestMode()
        {
            TransferEnded = null;
            TransferError = null;
            OutgoingTransfer = null;
            SendData(TransferCommands.TestMode);
        }

        private void SendData(string message)
        {
            this.session.Send(MessageContentType.FileData, PROTOCOL_V1_MARK + message);
        }

        private void OnIncomingTransferRequest(TransfersManagerEventArgs e)
        {
            if (IncomingTransferRequest != null)
                IncomingTransferRequest(this, e);
        }

        private void OnTransferEnded(TransfersManagerEventArgs e)
        {
            if (TransferEnded != null)
                TransferEnded(this, e);
        }

        private void OnTransferError(TransferErrorEventArgs e)
        {
            if (TransferError != null)
                TransferError(this, e);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion INotifyPropertyChanged
    }
}
