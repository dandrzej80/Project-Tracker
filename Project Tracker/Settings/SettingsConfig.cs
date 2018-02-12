using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Project_Tracker.Settings
{
	public partial class SettingsShell : Page
	{
		public const string FEATURE_NAME = "Settings";

		List<Scenario> scenarios = new List<Scenario>
		{
			new Scenario() {Title="Home Screen", ClassType=typeof(Settings.HomeScreen)},  
			new Scenario() {Title="Storage", ClassType=typeof(Settings.StorageSettings)},
			new Scenario() {Title="Return To Home", ClassType=typeof(MainPage)},
		};
	}

	public class Scenario
	{
		public string Title { get; set; }
		public Type ClassType { get; set; }
	}
}
