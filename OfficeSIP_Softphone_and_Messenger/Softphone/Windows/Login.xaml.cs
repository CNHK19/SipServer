// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;

namespace Messenger.Windows
{
    /// <summary>
    /// SOFTPHONE
    /// </summary>
    public partial class Login 
		: WindowEx
    {
		private readonly PropertiesBinding[] propertiesBinding;
		
		public Login()
		{
			#region propertiesBinding = new PropertiesBinding[] {..}

			propertiesBinding = new PropertiesBinding[]
			{
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.SignInAddress; },
					SourceSetter = (object value) => { Properties.Settings.Default.SignInAddress = (string)value; },
					TargetGetter = () => { return this.SignInAddress; },
					TargetSetter = (object value) => { this.SignInAddress = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.UseDefaultCredential; },
					SourceSetter = (object value) => { Properties.Settings.Default.UseDefaultCredential = (bool)value; },
					TargetGetter = () => { return this.UseDefaultCredential; },
					TargetSetter = (object value) => { this.UseDefaultCredential = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.UseSpecifiedCredential; },
					SourceSetter = (object value) => { Properties.Settings.Default.UseSpecifiedCredential = (bool)value; },
					TargetGetter = () => { return this.UseSpecifiedCredential; },
					TargetSetter = (object value) => { this.UseSpecifiedCredential = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.Username; },
					SourceSetter = (object value) => { Properties.Settings.Default.Username = (string)value; },
					TargetGetter = () => { return this.Username; },
					TargetSetter = (object value) => { this.Username = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.Password; },
					SourceSetter = (object value) => { Properties.Settings.Default.Password = (string)value; },
					TargetGetter = () => { return this.Password; },
					TargetSetter = (object value) => { this.Password = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.AutoConfigServer; },
					SourceSetter = (object value) => { Properties.Settings.Default.AutoConfigServer = (bool)value; },
					TargetGetter = () => { return this.AutoConfigServer; },
					TargetSetter = (object value) => { this.AutoConfigServer = (bool)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.ServerAddress; },
					SourceSetter = (object value) => { Properties.Settings.Default.ServerAddress = (string)value; },
					TargetGetter = () => { return this.ServerAddress; },
					TargetSetter = (object value) => { this.ServerAddress = (string)value; }
				},
				new PropertiesBinding() 
				{
					SourceGetter = () => { return Properties.Settings.Default.IpProtocol; },
					SourceSetter = (object value) => { Properties.Settings.Default.IpProtocol = (int)(Uccapi.TransportMode)value; },
					TargetGetter = () => { return this.TransportMode; },
					TargetSetter = (object value) => { this.TransportMode = (Uccapi.TransportMode)value; }
				},
			};

			#endregion

			PropertiesBinding.CopyToTarget(propertiesBinding);

			DataContext = this;
			InitializeComponent();

			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
		}

		public string SignInAddress { get; set; }
		public bool UseDefaultCredential { get; set; }
		public bool UseSpecifiedCredential { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool AutoConfigServer { get; set; }
		public string ServerAddress { get; set; }
		public Uccapi.TransportMode TransportMode { get; set; }

		public Array TransportModes
		{
			get { return Enum.GetValues(typeof(Uccapi.TransportMode)); }
		}

		public Object LoginParameter { get; set; }

		#region Password Databinding

		private void Password_Loaded(object sender, RoutedEventArgs e)
		{
			password.Password = Password;
		}

		private void Password_PasswordChanged(object sender, RoutedEventArgs e)
		{
			Password = password.Password;
		}

		#endregion

		#region UseDefault_Click, UseSpecified_Click

		private void UseDefault_Click(object sender, RoutedEventArgs e)
		{
			if (useDefault.IsChecked == false)
				useSpecified.IsChecked = true;
		}

		private void UseSpecified_Click(object sender, RoutedEventArgs e)
		{
			if (useSpecified.IsChecked == false)
				useDefault.IsChecked = true;
		}

		#endregion

		#region CommandBindings Event Handlers

		private void OkBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			PropertiesBinding.CopyToSource(propertiesBinding);
			Result = true;
			Close();
		}

		private void CancelBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Close();
		}

		#endregion
	}
}
