using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Project_Tracker.Settings
{
	public sealed partial class SettingsShell : Page
    {
		public static SettingsShell Current;

        public SettingsShell()
        {
            this.InitializeComponent();

			Current = this;
			SampleTitle.Text = FEATURE_NAME;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			//Populate the scenario list from the Configuration File
			ScenarioControl.ItemsSource = scenarios;
			if (Window.Current.Bounds.Width < 640)
			{
				ScenarioControl.SelectedIndex = -1;
			}
			else
			{
				ScenarioControl.SelectedIndex = 0;
			}
			if (!Globals.appSettings.Containers.ContainsKey("Storage"))
			{
				ScenarioControl.SelectedIndex = 1;
				ScenarioFrame.Navigate(typeof(StorageSettings));
			}
		}

		private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//Clear the status Block when navigating scenarios
			NotifyUser(String.Empty, NotifyType.StatusMessage);

			ListBox scenarioListBox = sender as ListBox;
			Scenario s = scenarioListBox.SelectedItem as Scenario;
			if (s != null)
			{
				if (s.ClassType == typeof(MainPage))
				{
					Frame.Navigate(typeof(MainPage));
				}
				else
				{
					ScenarioFrame.Navigate(s.ClassType);
					if (Window.Current.Bounds.Width < 640)
					{
						Splitter.IsPaneOpen = false;
					}
				}
			}
		}

		public List<Scenario> Scenarios
		{
			get { return this.scenarios; }
		}

		public void NotifyUser(string strMessage, NotifyType type)
		{
			if (Dispatcher.HasThreadAccess)
			{
				UpdateStatus(strMessage, type);
			}
			else
			{
				var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
			}
		}

		private void UpdateStatus(string strMessage, NotifyType type)
		{
			switch (type)
			{
				case NotifyType.StatusMessage:
					StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
					break;
				case NotifyType.ErrorMessage:
					StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
					break;
			}

			StatusBlock.Text = strMessage;

			// Collapse the StatusBlock if it has no text to conserve real estate.
			StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
			if (StatusBlock.Text != String.Empty)
			{
				StatusBorder.Visibility = Visibility.Visible;
				StatusPanel.Visibility = Visibility.Visible;
			}
			else
			{
				StatusBorder.Visibility = Visibility.Collapsed;
				StatusPanel.Visibility = Visibility.Collapsed;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
		}

	}
	public enum NotifyType
	{
		StatusMessage,
		ErrorMessage
	};

	public class ScenarioBindingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			Scenario s = value as Scenario;
			return (SettingsShell.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return true;
		}
	}

}
