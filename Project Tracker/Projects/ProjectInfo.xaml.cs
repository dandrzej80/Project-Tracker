using System;
using System.IO;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Tracker.Projects
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ProjectInfo : Page
	{
		public ProjectInfo()
		{
			this.InitializeComponent();
		}
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ProjNameBox.Text = Globals.currentProject;
			foreach (var p in Globals.projects)
			{
				if (p.projectName == Globals.currentProject)
				{
					if ((p.imgSource != "") && (p.imgSource != null))
					{
						ImageFileBlock.Text = p.imgSource.Substring(p.imgSource.LastIndexOf("\\") + 1);
						StorageFile imageFile = await StorageFile.GetFileFromPathAsync(p.imgSource);
						using (var stream = await imageFile.OpenAsync(FileAccessMode.Read))
						{
							var img = new BitmapImage();
							img.SetSource(stream);
							ImageCanvas.Source = img;
							ImageCanvas.Stretch = Stretch.None;
							ImageCanvas.Height = 100;
							ImageCanvas.Width = 75;
							ImageCanvas.Opacity = .75;
							ImageCanvas.VerticalAlignment = VerticalAlignment.Top;
							ImageCanvas.HorizontalAlignment = HorizontalAlignment.Center;
						}
					}
					if((p.webSources != "") && (p.webSources != null))
					{
						string[] websites = p.webSources.Split('\t');
						foreach (var s in websites)
						{
							if (s != "")
							{
								string sTrim = s.TrimEnd('\n');
								HyperlinkButton link = new HyperlinkButton();
								link.Content = sTrim;
								link.Tag = sTrim;
								link.FontSize = 18;
								link.Click += Link_Click;
								WebSourceBlock.Children.Add(link);
							}
						}
					}
				}
			}
		}
		private void Link_Click(object sender, RoutedEventArgs e)
		{
			WebSourceView.Visibility = Visibility.Visible;

			HyperlinkButton link = sender as HyperlinkButton;
			string webpage = link.Tag as string;

			WebSourceView.Navigate(new Uri(webpage));
		}
		private void ProjNameBox_GotFocus(object sender, RoutedEventArgs e)
		{
			ProjNameBox.SelectAll();
		}
		private void ProjNameBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (ProjNameBox.Text != Globals.currentProject)
			{
				//Update name in Project list.
				foreach (var p in Globals.projects)
				{
					int c = 0;

					if (p.projectName == Globals.currentProject)
					{
						Globals.projects[c].projectName = ProjNameBox.Text;
						Globals.SaveProjecList();
					}
					c++;
				}
				Globals.currentProject = ProjNameBox.Text;
				this.Frame.Navigate(typeof(ProjectShell), "Project Info");
			}
		}
		private async void ImageButton_Click(object sender, RoutedEventArgs e)
		{
			//Get File
			var picker = new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
			picker.FileTypeFilter.Add(".png");

			Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				await file.CopyAsync(Globals.storageLocation, Globals.currentProject + ".png", NameCollisionOption.ReplaceExisting);

				//Add image data to Project Data.
				foreach (var p in Globals.projects)
				{
					int c = 0;

					if(p.projectName == Globals.currentProject)
					{
						Globals.projects[c].imgSource = Globals.storageLocation.Path + "\\" + Globals.currentProject + ".png";

						//Save Data To File
						Globals.SaveProjecList();

						//Load Path & Image
						ImageFileBlock.Text = Globals.projects[c].imgSource.Substring(p.imgSource.LastIndexOf("\\") + 1);
						StorageFile imageFile = await StorageFile.GetFileFromPathAsync(p.imgSource);
						using (var stream = await imageFile.OpenAsync(FileAccessMode.Read))
						{
							var img = new BitmapImage();
							img.SetSource(stream);
							ImageCanvas.Source = img;
							ImageCanvas.Stretch = Stretch.None;
							ImageCanvas.Height = 100;
							ImageCanvas.Width = 75;
							ImageCanvas.Opacity = .75;
							ImageCanvas.VerticalAlignment = VerticalAlignment.Top;
							ImageCanvas.HorizontalAlignment = HorizontalAlignment.Center;
						}
					}
					c++;
				}
			}

		}
		private void AddWebButton_Click(object sender, RoutedEventArgs e)
		{
			int c = 0;
			foreach (var p in Globals.projects)
			{
				if (p.projectName == Globals.currentProject)
				{
					Globals.projects[c].webSources = Globals.projects[c].webSources + NewWebBox.Text + '\t';
					string[] websites = Globals.projects[c].webSources.Split('\t');
					WebSourceBlock.Children.Clear();
					foreach (string s in websites)
					{
						if (s != "")
						{
							string sTrim = s.TrimEnd('\n');
							HyperlinkButton link = new HyperlinkButton();
							link.Content = sTrim;
							link.Tag = sTrim;
							link.FontSize = 18;
							link.Click += Link_Click;
							WebSourceBlock.Children.Add(link);
						}
					}
					Globals.SaveProjecList();
					break;
				}
				c++;
			}
			NewWebBox.Text = "";
		}
		private void CloseWebView_Click(object sender, RoutedEventArgs e)
		{
			WebSourceView.Visibility = Visibility.Collapsed;
		}
	}
}
