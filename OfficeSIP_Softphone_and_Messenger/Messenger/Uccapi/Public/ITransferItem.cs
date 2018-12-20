// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;

namespace Uccapi
{
    public interface ITransferItem
        : INotifyPropertyChanged
    {
        long TransferedBytes { get; }
        double TransferedPercents { get; }
        FileTransferState State { get; }

        long FileSize { get; }
        string FileMD5 { get; }
        string FileName { get; }
        string FilePath { get; }
        string OtherSideURI { get; }
        string TrId { get; }
    }
}
