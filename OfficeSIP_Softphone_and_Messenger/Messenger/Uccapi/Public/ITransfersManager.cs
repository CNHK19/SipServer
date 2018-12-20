// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Uccapi
{
    public enum FileTransferState
    {
        Requested,
        Accepted,
        RejectedRemote,
        Active,
        SuccessfullyEnded,
        Corrupted,
        StoppedByUser,
        StoppedByError,
        ErrorDuringCreation
    }

    public interface ITransfersManager
        : INotifyPropertyChanged
    {
        ITransfersState Incoming { get; }
        ITransfersState Outgoing { get; }

        event EventHandler<TransfersManagerEventArgs> OutgoingTransfer;
        event EventHandler<TransfersManagerEventArgs> IncomingTransferRequest;
        event EventHandler<TransfersManagerEventArgs> TransferEnded;
        event EventHandler<TransferErrorEventArgs> TransferError;

        void Send(string[] files);

        void Stop(ITransferItem item);
        void Accept(string path, ITransferItem item);
        void Reject(ITransferItem item);

        void Stop(IEnumerable<ITransferItem> items);
        void Accept(string path, IEnumerable<ITransferItem> items);
        void Reject(IEnumerable<ITransferItem> items);

        void StartTestMode();
    }
}
