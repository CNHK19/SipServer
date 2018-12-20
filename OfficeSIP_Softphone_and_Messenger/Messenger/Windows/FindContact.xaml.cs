// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Uccapi;

namespace Messenger.Windows
{
	public partial class FindContact 
		: Window
		, INotifyPropertyChanged
	{
		private Endpoint endpoint;
		private int operationId;

		public FindContact(Endpoint endpoint1)
		{
			endpoint = endpoint1;

			InitializeComponent();

			Closed += FindContact_Closed;
			endpoint.UserSearchFinished += Endpoint_UserSearchFinished;
			endpoint.Presentities.CollectionChanged += Presentities_CollectionChanged;

			operationId = -1;
			DataContext = this;
		}

		private void FindContact_Closed(object sender, EventArgs e)
		{
			Closed -= FindContact_Closed;
			endpoint.UserSearchFinished -= Endpoint_UserSearchFinished;
			endpoint.Presentities.CollectionChanged -= Presentities_CollectionChanged;
		}

		public Endpoint Endpoint { get { return endpoint; } }

		public string SearchName { get; set; }
		public string SearchEmail { get; set; }
		
		public bool Searching { get; set; }
		public string SearchError { get; set; }
		public bool MoreAvailable { get; set; }
		public SearchUserRecord[] Results { get; set; }

		private void OnPropertiesGroup2Changed()
		{
			OnPropertyChanged(@"Searching");
			OnPropertyChanged(@"Results");
			OnPropertyChanged(@"MoreAvailable");
			OnPropertyChanged(@"SearchError");
		}

		private void Search_Click(object sender, RoutedEventArgs e)
		{
			Searching = true;
			Results = null;
			SearchError = null;

			OnPropertiesGroup2Changed();

			List<string> searchTerms = new List<string>();

			if (string.IsNullOrEmpty(SearchName) == false)
			{
				searchTerms.Add(@"givenName");
				searchTerms.Add(SearchName);
			}

			if (string.IsNullOrEmpty(SearchEmail) == false)
			{
				searchTerms.Add(@"givenEmail");
				searchTerms.Add(SearchEmail);
			}

			operationId = endpoint.BeginUserSearch(null, searchTerms);
		}

		private void Endpoint_UserSearchFinished(object sender, UserSearchEventArgs e)
		{
			if (operationId == e.OperationId)
			{
				Searching = false;

				if (e.IsOperationCompleteFailed)
				{
					SearchError = e.StatusText;
				}
				else
				{
					if (e.Results.Length > 0)
					{
						MoreAvailable = e.MoreAvailable;
						Results = e.Results;
					}
					else
					{
						SearchError = "Not found!";
					}
				}

				OnPropertiesGroup2Changed();
			}
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			operationId = -1;
			Searching = false;
			OnPropertyChanged(@"Searching");
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Presentities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			endpoint.UpdateSearchUserRecords(Results, e);
		}

		private void ListView_Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (resultsView.SelectedIndex != -1)
			{
				string uri = (resultsView.SelectedItem as SearchUserRecord).Uri;
				if (endpoint.IsValidUri(uri))
					endpoint.Presentities.Add(endpoint.CreatePresentity(uri, @""));
			}

			e.Handled = true;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			List<IPresentity> presentities = new List<IPresentity>();

			foreach (SearchUserRecord record in resultsView.SelectedItems)
				if (endpoint.IsValidUri(record.Uri))
					presentities.Add(endpoint.CreatePresentity(record.Uri, @""));

			endpoint.Presentities.AddRange(presentities);
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
