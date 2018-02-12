using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.Storage;
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
	public sealed partial class StorageSettings : Page
	{
		public StorageSettings()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			DefaultBlock.Text = Globals.storageLocation.Path;
		}

		private async void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
			var folderPicker = new Windows.Storage.Pickers.FolderPicker();
			folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");

			StorageFolder folder = await folderPicker.PickSingleFolderAsync();
			if (folder != null)
			{
				folder = await folder.CreateFolderAsync("Project Tracker");
				Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
				Globals.storageLocation = folder;
				if (!Globals.appSettings.Containers.ContainsKey("Storage"))
				{
					Globals.appSettings.CreateContainer("Storage", ApplicationDataCreateDisposition.Always);
				}
				Globals.appSettings.Containers["Storage"].Values["DefaultLocation"] = Globals.storageLocation.Path;
				DefaultBlock.Text = Globals.storageLocation.Path;
			}
			else
			{
				var dialog = new MessageDialog("No folder selected!", "Operation Cancelled");

				await dialog.ShowAsync();
			}
			//Globals.appSettings.DeleteContainer("Storage");
		}
	}
}
