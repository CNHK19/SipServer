// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace Uccapi
{
    public class TransferItem : ITransferItem
    {
        private long processedDataBytes;
        private double processedDataPercent;
        private FileTransferState state;
        private string filePath;

        public long TransferedBytes
        {
            get { return processedDataBytes; }
            set
            {
                if (processedDataBytes != value)
                {
                    processedDataBytes = value;

                    if (FileSize == 0)
                        TransferedPercents = 100;
                    else if (Math.Round((double)(processedDataBytes * 100 / FileSize)) != Math.Round((double)processedDataPercent))
                        TransferedPercents = (processedDataBytes * 100 / FileSize);

                    OnPropertyChanged(@"ProcessedDataBytes");
                }
            }
        }

        public double TransferedPercents
        {
            get { return processedDataPercent; }
            set
            {
                processedDataPercent = value;
                OnPropertyChanged(@"ProcessedDataPercent");
            }
        }

        public FileTransferState State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged(@"State");
            }
        }

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(@"FilePath");
            }
        }

        public long FileSize { get; set; }
        public string TrId { get; set; }
        public string OtherSideURI { get; set; }
        public string FileName { get; set; }
        public string FileMD5 { get; set; }
        public IImSession Session { get; private set; }

        public TransferItem(IImSession session)
        {
            Session = session;
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
