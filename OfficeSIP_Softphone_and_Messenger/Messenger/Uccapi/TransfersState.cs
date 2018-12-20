// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Uccapi
{
    public class TransfersState : ITransfersState
    {
        private long totalBytes;
        private long transferedBytes;
        private double transferedPercents;
        private int currentIndex;
        private int maxIndex;
        private ITransferItem currentFile;
        private ObservableCollection<ITransferItem> files;
        private ReadOnlyObservableCollection<ITransferItem> readonlyFiles;

        public TransfersState()
        {
            files = new ObservableCollection<ITransferItem>();
            readonlyFiles = new ReadOnlyObservableCollection<ITransferItem>(files);
            Initialize();
        }

        public void Initialize()
        {
            TotalBytes = 0;
            TransferedBytes = 0;
            TransferedPercents = 0;
            MaxIndex = 0;
            CurrentIndex = 1;
            CurrentFile = null;
            files.Clear();
        }

        public long TotalBytes
        {
            get { return totalBytes; }
            set
            {
                if (totalBytes != value)
                {
                    totalBytes = value;
                    OnPropertyChanged(@"TotalBytes");
                }
            }
        }

        public long TransferedBytes
        {
            get { return transferedBytes; }
            set
            {
                if (transferedBytes != value)
                {
                    transferedBytes = value;

                    if (TotalBytes == 0)
                        TransferedPercents = 100;
                    else if (Math.Round((double)(transferedBytes * 100 / TotalBytes)) != Math.Round((double)TransferedPercents))
                        TransferedPercents = (transferedBytes * 100 / TotalBytes);

                    OnPropertyChanged(@"TransferedBytes");
                }
            }
        }

        public double TransferedPercents
        {
            get { return transferedPercents; }
            set
            {
                if (transferedPercents != value)
                {
                    transferedPercents = value;
                    OnPropertyChanged(@"TransferedPercents");
                }
            }
        }

        public int CurrentIndex
        {
            get { return currentIndex ; }
            set
            {
                if (currentIndex != value)
                {
                    currentIndex = value;
                    OnPropertyChanged(@"CurrentIndex");
                }
            }
        }

        public int MaxIndex
        {
            get { return maxIndex ; }
            set
            {
                if (maxIndex != value)
                {
                    maxIndex = value;
                    OnPropertyChanged(@"MaxIndex");
                }
            }
        }

        public ITransferItem CurrentFile
        {
            get { return currentFile; }
            set
            {
                if (currentFile != value)
                {
                    currentFile = value;
                    OnPropertyChanged(@"CurrentFile");
                }
            }
        }

        public ReadOnlyObservableCollection<ITransferItem> Files
        {
            get { return readonlyFiles; }
        }

        public TransferItem GetTransferItem(string trId)
        {
            return files.SingleOrDefault(x => x.TrId == trId) as TransferItem;
        }

        public void AddTransferItem(TransferItem item)
        {
            files.Add(item);
            MaxIndex++;
        }

        public void RemoveTransferItem(TransferItem item)
        {
            files.Remove(item);
        }

        public void RemoveTransferItem(string trId)
        {
            RemoveTransferItem(GetTransferItem(trId));
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