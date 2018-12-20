// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;

namespace Messenger.Windows
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences 
		: Window
    {
		private readonly PropertiesBinding[] propertiesBinding;
		
		public Preferences()
		{
			#region propertiesBinding = new PropertiesBinding[] {..}

			propertiesBinding = new PropertiesBinding[]
			{
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.LoginAtStartup; },
					SourceSetter = (object value) => { Properties.Settings.Default.LoginAtStartup = (bool)value; },
					TargetGetter = () => { return this.LoginAtStartup; },
					TargetSetter = (object value) => { this.LoginAtStartup = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.RunMinimized; },
					SourceSetter = (object value) => { Properties.Settings.Default.RunMinimized = (bool)value; },
					TargetGetter = () => { return this.RunMinimized; },
					TargetSetter = (object value) => { this.RunMinimized = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.MinimizeOnClose; },
					SourceSetter = (object value) => { Properties.Settings.Default.MinimizeOnClose = (bool)value; },
					TargetGetter = () => { return this.MinimizeOnClose; },
					TargetSetter = (object value) => { this.MinimizeOnClose = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.AlwaysOnTop; },
					SourceSetter = (object value) => { Properties.Settings.Default.AlwaysOnTop = (bool)value; },
					TargetGetter = () => { return this.AlwaysOnTop; },
					TargetSetter = (object value) => { this.AlwaysOnTop = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.NoSplash; },
					SourceSetter = (object value) => { Properties.Settings.Default.NoSplash = (bool)value; },
					TargetGetter = () => { return this.NoSplash; },
					TargetSetter = (object value) => { this.NoSplash = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.RestoreConnection; },
					SourceSetter = (object value) => { Properties.Settings.Default.RestoreConnection = (bool)value; },
					TargetGetter = () => { return this.RestoreConnection; },
					TargetSetter = (object value) => { this.RestoreConnection = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.IncomingMessageSound; },
					SourceSetter = (object value) => { Properties.Settings.Default.IncomingMessageSound = (string)value; },
					TargetGetter = () => { return this.IncomingMessageSound; },
					TargetSetter = (object value) => { this.IncomingMessageSound = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.OutgoingMessageSound; },
					SourceSetter = (object value) => { Properties.Settings.Default.OutgoingMessageSound = (string)value; },
					TargetGetter = () => { return this.OutgoingMessageSound; },
					TargetSetter = (object value) => { this.OutgoingMessageSound = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.IncomingCallSound; },
					SourceSetter = (object value) => { Properties.Settings.Default.IncomingCallSound = (string)value; },
					TargetGetter = () => { return this.IncomingCallSound; },
					TargetSetter = (object value) => { this.IncomingCallSound = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.AutoAway; },
					SourceSetter = (object value) => { Properties.Settings.Default.AutoAway = (bool)value; },
					TargetGetter = () => { return this.AutoAway; },
					TargetSetter = (object value) => { this.AutoAway = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.AutoAwaySeconds; },
					SourceSetter = (object value) => { Properties.Settings.Default.AutoAwaySeconds = (int)value; },
					TargetGetter = () => { return this.AutoAwaySeconds; },
					TargetSetter = (object value) => { this.AutoAwaySeconds = (int)value; }
				},
			};

			#endregion

			PropertiesBinding.CopyToTarget(propertiesBinding);

			DataContext = this;
			InitializeComponent();

			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
		}

		public bool LoginAtStartup { get; set; }
		public bool RunMinimized { get; set; }
		public bool MinimizeOnClose { get; set; }
		public bool AlwaysOnTop { get; set; }
		public bool NoSplash { get; set; }
		public bool RestoreConnection { get; set; }
		public string IncomingMessageSound { get; set; }
		public string OutgoingMessageSound { get; set; }
		public string IncomingCallSound { get; set; }
		public bool AutoAway { get; set; }
		public int AutoAwaySeconds { get; set; }

		private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			foreach (char char1 in e.Text)
				e.Handled |= !char.IsDigit(char1);
		}

		#region CommandBindings Event Handlers

		private void OkBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			ApplyBinding_Executed(sender, e);
			Close();
		}

		private void OkBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{
			ApplyBinding_CanExecute(sender, e);
		}

		private void CancelBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void ApplyBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			PropertiesBinding.CopyToSource(propertiesBinding);
		}

		private void ApplyBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = PropertiesBinding.SourceNotEqualsTarget(propertiesBinding);
		}

		#endregion
	}
}
