// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Uccapi
{
    public interface ITransfersState
        : INotifyPropertyChanged
    {
        long TotalBytes { get; }
        long TransferedBytes { get; }
        double TransferedPercents { get; }
        int CurrentIndex { get; }
        int MaxIndex { get; }
        ITransferItem CurrentFile { get; }
        ReadOnlyObservableCollection<ITransferItem> Files { get; }
    }
}