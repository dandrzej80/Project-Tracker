using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Tracker.Settings
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class HomeScreen : Page
	{
		public HomeScreen()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			switch(Globals.appSettings.Containers["HomeScreen"].Values["SortMethod"].ToString())
			{
				case "projectName":
					{
						SortRadio1.IsChecked = true;
						SortRadio2.IsChecked = false;
						break;
					}
				case "lastModified":
					{
						SortRadio1.IsChecked = false;
						SortRadio2.IsChecked = true;
						break;
					}
			}
		}
		private void SortRadio1_Checked(object sender, RoutedEventArgs e)
		{
			Globals.appSettings.Containers["HomeScreen"].Values["SortMethod"] = "projectName";
			SortRadio1.IsChecked = true;
			SortRadio2.IsChecked = false;
			Globals.SortProjectList();
		}
		private void SortRadio2_Checked(object sender, RoutedEventArgs e)
		{
			Globals.appSettings.Containers["HomeScreen"].Values["SortMethod"] = "lastModified";
			SortRadio1.IsChecked = false;
			SortRadio2.IsChecked = true;
			Globals.SortProjectList();
		}
	}
}
