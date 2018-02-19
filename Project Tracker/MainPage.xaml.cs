using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Project_Tracker
{
	public sealed partial class MainPage : Page
    {
		//Page-Wide Variables
		private DispatcherTimer currentTime;
		int count = 0;

		public MainPage()
        {
            this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			//Display Current Time
			currentTime = new DispatcherTimer();
			currentTime.Start();
			currentTime.Tick += realTime_Tick;

			//Settings And Storage Validation
			Globals.appSettings = ApplicationData.Current.RoamingSettings;
			if (Globals.appSettings.Containers.ContainsKey("Storage"))
			{
				if (Globals.appSettings.Containers["Storage"].Values.ContainsKey("DefaultLocation"))
				{
					Globals.storageLocation = await StorageFolder.GetFolderFromPathAsync(Convert.ToString(Globals.appSettings.Containers["Storage"].Values["DefaultLocation"]));
				}
				else
				{
					await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
					{
						this.Frame.Navigate(typeof(Settings.SettingsShell));
					});
				}
			}
			else
			{
				await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					this.Frame.Navigate(typeof(Settings.SettingsShell));
				});
			}
			if (!Globals.appSettings.Containers.ContainsKey("HomeScreen"))
			{
				Globals.appSettings.CreateContainer("HomeScreen", ApplicationDataCreateDisposition.Always);
				Globals.appSettings.Containers["HomeScreen"].Values["SortMethod"] = "projectName";
			}

			//Load Data and Add Buttons
			LoadData();

			//Load Messages
			List<ToDoList> list = new List<ToDoList> { };
			list = Globals.tasks.OrderByDescending(x => x.dateCompleted).ToList();
			foreach (var l in list)
			{
				if (l.projCompleted == true)
				{
					statusBlock.Text = "(" + l.projectName + ") " + l.projTask + " -- " + l.dateCompleted.ToShortDateString() + Environment.NewLine;
				}
			}
		}

		//Page Functions
		private async void LoadData()
		{
			//Load Project List
			if (Globals.projects.Count == 0)
			{
				if (await Globals.storageLocation.TryGetItemAsync("projectList") != null)
				{
					var Serializer = new DataContractSerializer(typeof(List<ProjectList>));
					using (var stream = await Globals.storageLocation.OpenStreamForReadAsync("projectList"))
					{
						Globals.projects = (List<ProjectList>)Serializer.ReadObject(stream);
						stream.Dispose();
					}
				}
				else
				{
					MessageDialog dialog = new MessageDialog("file does not exist");
					await dialog.ShowAsync();
				}
				Globals.SortProjectList();
			}

			//Load Task List
			if (Globals.tasks.Count == 0)
			{
				if (await Globals.storageLocation.TryGetItemAsync("taskList") != null)
				{
					var Serializer = new DataContractSerializer(typeof(List<ToDoList>));
					using (var stream = await Globals.storageLocation.OpenStreamForReadAsync("taskList"))
					{
						Globals.tasks = (List<ToDoList>)Serializer.ReadObject(stream);
						stream.Dispose();
					}
				}
				else
				{
					MessageDialog dialog = new MessageDialog("file does not exist");
					await dialog.ShowAsync();
				}
			}

			//Add Buttons
			AddButtons();
		}
		private async void AddButtons()
		{
			int tabIndex = 1;
			count++;
			var style = new Style();
			style = (Style)Application.Current.Resources["ProjButton"];
			DataContext = this;

			//Remove Program Buttons
			foreach (Button b in ButtonPanel.Children)
			{
				if (b.Name != addNewButton.Name)
				{
					ButtonPanel.Children.Remove(b);
				}
			}
			addNewButton.TabIndex = 99;

			//Add Buttons
			if (Globals.projects != null)
			{
				foreach (var project in Globals.projects)
				{
					int numTasks = 0;

					if (project != null)
					{
						foreach (var t in Globals.tasks)
						{
							if ((t.projectName == project.projectName) && (t.projCompleted == false))
								numTasks++;
						}
						RelativePanel panel = new RelativePanel
						{
							Margin = new Thickness(0, 105, 0, 0),
							Width = 130,
							Height = 25,
						};
						TextBlock blockName = new TextBlock
						{
							Text = project.projectName,
							TextAlignment = TextAlignment.Center,
							TextWrapping = TextWrapping.Wrap,
							FontSize = 10,
							Margin = new Thickness(25,0,0,0),
							Width = 80,
						};
						RelativePanel.SetAlignLeftWithPanel(blockName, true);
						panel.Children.Add(blockName);
						if (numTasks != 0)
						{
							TextBlock blockTasks = new TextBlock
							{
								Text = numTasks.ToString(),
								TextAlignment = TextAlignment.Center,
								Margin = new Thickness(-1, 2, 0, 0),
								FontSize = 10,
								Width = 20,
							};
							Brush shapeColor = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
							Ellipse shape = new Ellipse
							{
								Width = 20,
								Height = 20,
								Fill = shapeColor,
							};
							RelativePanel.SetAlignRightWithPanel(shape, true);
							RelativePanel.SetAlignRightWithPanel(blockTasks, true);
							panel.Children.Add(shape);
							panel.Children.Add(blockTasks);
						}

						Button newButton = new Button
						{
							Name = project.projectName.Replace(" ", "") + "Button",
							Content = panel,
							Foreground = new SolidColorBrush(Windows.UI.Colors.White),
							Width = 150,
							Height = 150,
							Margin = new Thickness(5),
							TabIndex = tabIndex,
							Style = style,
						};
						newButton.Click += ProjectButton_Click;
						if (tabIndex <= 5)
						{
							if (tabIndex == 1)
							{
								RelativePanel.SetAlignTopWithPanel(newButton, true);
								RelativePanel.SetAlignLeftWithPanel(newButton, true);
							}
							else
							{
								RelativePanel.SetAlignTopWithPanel(newButton, true);
								RelativePanel.SetRightOf(newButton, ButtonPanel.Children[tabIndex - 1]);

							}
						}
						if ((tabIndex > 5) && (tabIndex <= 10))
						{
							if (tabIndex == 6)
							{
								RelativePanel.SetBelow(newButton, ButtonPanel.Children[1]);
								RelativePanel.SetAlignLeftWithPanel(newButton, true);

							}
							else
							{
								RelativePanel.SetBelow(newButton, ButtonPanel.Children[1]);
								RelativePanel.SetRightOf(newButton, ButtonPanel.Children[tabIndex - 1]);
							}
						}
						if ((tabIndex > 10) && (tabIndex <= 14))
						{
							if (tabIndex == 11)
							{
								RelativePanel.SetAlignBottomWithPanel(newButton, true);
								RelativePanel.SetAlignLeftWithPanel(newButton, true);
							}
							else
							{
								RelativePanel.SetBelow(newButton, ButtonPanel.Children[6]);
								RelativePanel.SetRightOf(newButton, ButtonPanel.Children[tabIndex - 1]);
							}
						}
						//					if (tabIndex > 14)
						//					{
						//						MessageDialog dialog = new MessageDialog("Max Projects Reached", "Max Projects Reached");/////
						//
						//						await dialog.ShowAsync();
						//					}
						var backImg = new ImageBrush();
						if ((project.imgSource != "") && (project.imgSource != null))
						{
							StorageFile imageFile = await StorageFile.GetFileFromPathAsync(project.imgSource);
							var img = new BitmapImage();
							using (var stream = await imageFile.OpenAsync(FileAccessMode.Read))
							{
								img.SetSource(stream);
							}
							backImg.ImageSource = img;
							backImg.Stretch = Stretch.None;
							backImg.AlignmentY = AlignmentY.Top;
							backImg.AlignmentX = AlignmentX.Center;
							backImg.Opacity = .75;
							newButton.Background = backImg;
						}
						ButtonPanel.Children.Add(newButton);
						tabIndex++;
					}
				}
			}
		}
		private void realTime_Tick(object sender, object e)
		{
			dateTimeBlock.Text = DateTime.Now.ToString();
		}
		private async void addNewButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new AddProjectDialog();
			var result = await dialog.ShowAsync();
			bool exists = false;
			
			if (result == ContentDialogResult.Primary)
			{
				foreach (var rec in Globals.projects)
				{
					if (rec.projectName == dialog.Text)
					{
						exists = true;
					}
				}
				if (exists == false)
				{
					Globals.projects.Add(new ProjectList
					{
						projectName = dialog.Text,
						dateCreated = DateTime.Now,
						lastModified = DateTime.Now
					});
					Globals.SortProjectList();
				}
			}

			//Save List To File
			StorageFile storageFile = await Globals.storageLocation.CreateFileAsync("projectList", CreationCollisionOption.ReplaceExisting);

			IRandomAccessStream raStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);

			using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
			{
				//Serialize the Session State
				DataContractSerializer serializer = new DataContractSerializer(typeof(List<ProjectList>));
				serializer.WriteObject(outStream.AsStreamForWrite(), Globals.projects);
				await outStream.FlushAsync();
				outStream.Dispose();
				raStream.Dispose();
			}
			this.Frame.Navigate(typeof(MainPage));

		}
		private void ProjectButton_Click(object sender, RoutedEventArgs e)
		{
			Button buttonClicked = (sender as Button);
			RelativePanel buttonContents = buttonClicked.Content as RelativePanel;
			TextBlock buttonName = buttonContents.Children[0] as TextBlock;
			Globals.currentProject = buttonName.Text;
			this.Frame.Navigate(typeof(Projects.ProjectShell));
		}

		//Program Settings
		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(Settings.SettingsShell));
		}
	}
}
