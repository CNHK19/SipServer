using System;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Uccapi;

namespace Messenger
{
#if DEBUG
    public class FileTransferTests
    {
        private IImSession workingSession;

        private static class TestsHelper
        {
            public static int CurrentTestNumber;
            public static int CurrentCounter;
            public static System.Windows.Controls.Primitives.TextBoxBase DebugTextBox;
            public static FileStream TestFileStream;
            public static void DebugPrint(string TextToOutput)
            {
                TextToOutput = DateTime.Now.ToString("HH:mm:ss.fff") + ": " + TextToOutput + "\n";
                System.Diagnostics.Debug.Print(TextToOutput);
                if (DebugTextBox != null)
                    DebugTextBox.AppendText(TextToOutput);
            }
        }

        public void PerformFileTransfersTests(IImSession testSession)
        {
            PerformFileTransfersTests(testSession, null);
        }

        public void PerformFileTransfersTests(IImSession testSession, System.Windows.Controls.Primitives.TextBoxBase DBT)
        {
            workingSession = testSession;
            TestsHelper.DebugTextBox = DBT;
            TestsHelper.CurrentTestNumber = 0;
            testSession.TransfersManager.StartTestMode();
            testSession.TransfersManager.TransferEnded += new EventHandler<TransfersManagerEventArgs>(OnTransferEnded);
            testSession.TransfersManager.TransferError += new EventHandler<TransferErrorEventArgs>(OnTransferError);
            PrepareFileSystem();
            FileTests();
        }

        private void PrepareFileSystem()
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet"));
                CreateTestSet();
            }
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSetIncoming")))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSetIncoming"));

        }

        private void CreateTestSet()
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet");
            CreateTestFile(Path.Combine(basePath, "Test1File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test2File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test3File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test3File2.tst"), 0);
            CreateTestFile(Path.Combine(basePath, "Test4File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test4File2.tst"), 0);
            CreateTestFile(Path.Combine(basePath, "Test4File3.tst"), 345400);
            CreateTestFile(Path.Combine(basePath, "Test4File4.tst"), 30000);
            CreateTestFile(Path.Combine(basePath, "Test4File5.tst"), 90000);
            CreateTestFile(Path.Combine(basePath, "Test5File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test5File2.tst"), 0);
            CreateTestFile(Path.Combine(basePath, "Test5File3.tst"), 345400);
            CreateTestFile(Path.Combine(basePath, "Test5File4.tst"), 30000);
            CreateTestFile(Path.Combine(basePath, "Test5File5.tst"), 90000);
            CreateTestFile(Path.Combine(basePath, "Test6File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test7File1.tst"), 1);
            CreateTestFile(Path.Combine(basePath, "Test8File1.tst"), 1727000);
        }

        private void CreateTestFile(string FileName, int FileSize)
        {
            FileStream TestFile = File.Create(FileName);
            byte cb = 0;

            for (int c = 1; c <= FileSize; c++)
            {
                cb %= 10;
                TestFile.WriteByte(cb++);
            }
            TestFile.Close();
        }

        private void FileTests()
        {
            TestsHelper.CurrentTestNumber++;
            TestsHelper.CurrentCounter = 0;
            string[] sbuf;

            switch (TestsHelper.CurrentTestNumber)
            {
                case 1:
                    TestsHelper.DebugPrint("*** Test #1: Simple one file transfer ***");
                    sbuf = new string[1];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test1File1.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 2:
                    TestsHelper.DebugPrint("*** Test #2: One zero-sized file transfer. ***");
                    sbuf = new string[1];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test2File1.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 3:
                    TestsHelper.DebugPrint("*** Test #3: Transfering 2 files at once. ***");
                    sbuf = new string[2];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test3File1.tst");
                    sbuf[1] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test3File2.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 4:
                    TestsHelper.DebugPrint("*** Test #4: Transfering 5 files at once. Special files sizes. ***");
                    sbuf = new string[5];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test4File1.tst");
                    sbuf[1] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test4File2.tst");
                    sbuf[2] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test4File3.tst");
                    sbuf[3] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test4File4.tst");
                    sbuf[4] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test4File5.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 5:
                    TestsHelper.DebugPrint("*** Test #5: Transfering 5 files in 2 parts. ***");
                    sbuf = new string[3];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test5File1.tst");
                    sbuf[1] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test5File2.tst");
                    sbuf[2] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test5File3.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    sbuf = new string[2];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test5File4.tst");
                    sbuf[1] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test5File5.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 6:
                    TestsHelper.DebugPrint("*** Test #6: Transfering 1 file 2 times immediately. ***");
                    sbuf = new string[1];
                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test6File1.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 7:
                    TestsHelper.DebugPrint("*** Test #7: Transfering of busy file. ***");
                    sbuf = new string[1];
                    if (TestsHelper.TestFileStream != null)
                        TestsHelper.TestFileStream.Close();

                    TestsHelper.TestFileStream = new FileStream(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test7File1.tst"),
                        FileMode.Open, FileAccess.Write);

                    sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test7File1.tst");
                    workingSession.TransfersManager.Send(sbuf);
                    break;
                case 8:
                    //TestsHelper.DebugPrint("*** Test #8: Session termination during transfer. ***");
                    //sbuf = new string[1];
                    //WorkingSession.TransfersManager.FTransfers.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChangeTest8);
                    //sbuf[0] = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSet\\Test8File1.tst");
                    //WorkingSession.TransfersManager.SendFiles(sbuf);
                    break;
            }
        }

        private void OnTransferError(object sender, TransferErrorEventArgs  e)
        {
            if (TestsHelper.CurrentTestNumber == 7 && e.Items != null && e.Items.FirstOrDefault() != null &&
                e.ItemsState == FileTransferState.ErrorDuringCreation )
            {
                if (TestsHelper.CurrentTestNumber == 7)
                {
                    TestsHelper.TestFileStream.Close();
                    TestsHelper.TestFileStream = null;
                    TestsHelper.DebugPrint("*** Test #7 Passed ***");
                    FileTests();
                }
            }
        }

        private void OnTransferEnded(object sender, TransfersManagerEventArgs e)
        {

            var EndedState = e.Items.FirstOrDefault().State;

            switch (TestsHelper.CurrentTestNumber)
            {
                case 1:
                    if (EndedState == FileTransferState.SuccessfullyEnded)
                        TestsHelper.DebugPrint("*** Test #1 Passed ***");
                    else

                        TestsHelper.DebugPrint("*** Test #1 Failed ***");

                    FileTests();
                    break;
                case 2:
                    if (EndedState == FileTransferState.SuccessfullyEnded)
                        TestsHelper.DebugPrint("*** Test #2 Passed ***");
                    else
                        TestsHelper.DebugPrint("*** Test #2 Failed ***");

                    FileTests();
                    break;
                case 3:
                    TestsHelper.CurrentCounter++;

                    if ((EndedState == FileTransferState.SuccessfullyEnded) & TestsHelper.CurrentCounter == 2)
                    {
                        TestsHelper.DebugPrint("*** Test #3 Passed ***");
                        FileTests();
                    }
                    else if (EndedState != FileTransferState.SuccessfullyEnded)
                    {
                        TestsHelper.DebugPrint("*** Test #3 Failed ***");
                        FileTests();
                    }
                    break;
                case 4:
                    TestsHelper.CurrentCounter++;

                    if ((EndedState == FileTransferState.SuccessfullyEnded) & TestsHelper.CurrentCounter == 5)
                    {
                        TestsHelper.DebugPrint("*** Test #4 Passed ***");
                        FileTests();
                    }
                    else if (EndedState != FileTransferState.SuccessfullyEnded)
                    {
                        TestsHelper.DebugPrint("*** Test #4 Failed ***");
                        FileTests();
                    }

                    break;
                case 5:
                    TestsHelper.CurrentCounter++;

                    if ((EndedState == FileTransferState.SuccessfullyEnded) & TestsHelper.CurrentCounter == 5)
                    {
                        TestsHelper.DebugPrint("*** Test #5 Passed ***");
                        FileTests();
                    }
                    else if (EndedState != FileTransferState.SuccessfullyEnded)
                    {
                        TestsHelper.DebugPrint("*** Test #5 Failed ***");
                        FileTests();
                    }

                    break;
                case 6:
                    TestsHelper.CurrentCounter++;

                    if ((EndedState == FileTransferState.SuccessfullyEnded) & TestsHelper.CurrentCounter == 2)
                    {
                        TestsHelper.DebugPrint("*** Test #6 Passed ***");
                        FileTests();
                    }
                    else if (EndedState != FileTransferState.SuccessfullyEnded & TestsHelper.CurrentCounter == 2)
                    {
                        TestsHelper.DebugPrint("*** Test #6 Failed ***");
                        FileTests();
                    }

                    break;
                case 7:

                    break;
                case 8:
                    //TestsHelper.CurrentCounter++;

                    //if ((EndedState == FileTransferState.StoppedByError) & TestsHelper.CurrentCounter == 1)
                    //{
                    //    TestsHelper.DebugPrint("*** Test #8 Passed ***");
                    //}
                    //else if (EndedState != FileTransferState.SuccessfullyEnded)
                    //{
                    //    TestsHelper.DebugPrint("*** Test #8 Failed ***");
                    //}
                    break;
            }
        }

        private void OnIncomingTransferRequest(object sender, TransfersManagerEventArgs e)
        {
            TransfersManager TransfersManager = (TransfersManager)sender;
            TransfersManager.Accept(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesSetIncoming\\"), e.Items);
        }

        public void OnCollectionChangeTest8(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TestsHelper.CurrentTestNumber != 8)
                return;

            if (e.NewItems != null)
            {
                ((ITransferItem)e.NewItems[0]).PropertyChanged += new PropertyChangedEventHandler(OnStateChangeTest8);
            }
        }

        public void OnStateChangeTest8(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                if (((ITransferItem)sender).State == FileTransferState.Active)
                {
					throw new NotImplementedException();
                //    ((ITransferItem)sender).Session.Destroy();
                }
            }
        }
    }
#endif
}
