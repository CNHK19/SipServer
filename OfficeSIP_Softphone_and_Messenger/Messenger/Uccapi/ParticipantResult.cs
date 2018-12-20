// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	class ParticipantResult
		: UccapiBase
		, IParticipantResult
	{
		public ParticipantResult(string uri)
		{
			this.Uri = uri;
			this.IsComplete = false;
		}

		public string Uri { get; private set; }
		public bool IsComplete { get; private set; }
		public int StatusCode { get; private set; }
		public string StatusText { get; private set; }

		public void Set(IUccOperationProgressEvent operationProgress)
		{
			if (this.IsComplete != operationProgress.IsComplete)
			{
				this.IsComplete = operationProgress.IsComplete;
				this.OnPropertyChanged("IsComplete");
			}

			if(this.StatusCode != operationProgress.StatusCode)
			{
				this.StatusCode = operationProgress.StatusCode;
				this.OnPropertyChanged("StatusCode");
				this.OnPropertyChanged("Error");
			}

			if(this.StatusText != operationProgress.StatusText)
			{
				this.StatusText = operationProgress.StatusText;
				this.OnPropertyChanged("StatusText");
			}
		}

		public string Error
		{
			get
			{
				return Errors.ToString(this.StatusCode);
			}
		}
	}
}
