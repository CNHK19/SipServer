// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;

namespace Uccapi
{
	public interface IImSession
		: ISession
	{
		event EventHandler<ImSessionEventArgs1> SendResult;
		event EventHandler<ImSessionEventArgs2> IncomingMessage;

		IOutgoingMessage Send(string contentType, string content);
		void Composing(bool composing);

		ITransfersManager TransfersManager { get; }
	}

    public class ImSessionEventArgs1
        : EventArgs
    {
        public ImSessionEventArgs1(IOutgoingMessage message)
        {
            this.Message = message;
        }

        public IOutgoingMessage Message { get; private set; }
    }

    public class ImSessionEventArgs2
        : EventArgs
    {
        public ImSessionEventArgs2(IIncomingMessage message)
        {
            this.Message = message;
        }

        public IIncomingMessage Message { get; private set; }
    }
}
