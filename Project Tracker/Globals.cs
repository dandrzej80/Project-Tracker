using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Project_Tracker
{
	class Globals
	{
		#region Variables
		//App Settings
		public static ApplicationDataContainer appSettings = null;
		public static string currentProject = "";

		//Storage Settings
		public static StorageFolder storageLocation = KnownFolders.DocumentsLibrary;

		//Data Lists
		public static List<ProjectList> projects = new List<ProjectList> { };
		public static List<ToDoList> tasks = new List<ToDoList> { };
		#endregion

		#region Functions
		public static void SortProjectList()
		{
			string homeSort = Globals.appSettings.Containers["HomeScreen"].Values["SortMethod"].ToString();

			switch(homeSort)
			{
				case "projectName":
					{
						projects = projects.OrderBy(o => o.projectName).ToList();
						break;
					}
				case "dateCreated":
					{
						projects = projects.OrderByDescending(o => o.dateCreated).ToList();
						break;
					}
				case "lastModified":
					{
						projects = projects.OrderByDescending(o => o.lastModified).ToList();
						break;
					}
			}
		}
		public static async void SaveProjecList()
		{
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
		}
		public static void DisplayTasks(RelativePanel panel)
		{
			List<ToDoList> listComplete = new List<ToDoList> { };
			List<ToDoList> listIncomplete = new List<ToDoList> { };
			int count = 0;

			foreach (var t in tasks)
			{
				if (t.projectName == currentProject)
				{
					if (t.projCompleted == false)
					{
						listIncomplete.Add(t);
					}
					else
					{
						listComplete.Add(t);
					}
				}
			}
			listIncomplete = listIncomplete.OrderBy(x => x.projTask).ToList();
			listComplete = listComplete.OrderBy(x => x.projTask).ToList();
			foreach (var t in listIncomplete)
			{
				CheckBox check = new CheckBox
				{
					Tag = t.projTask,
					IsChecked = t.projCompleted,
					Width = 75,
					HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
					Margin = new Windows.UI.Xaml.Thickness(50, 5, 0, 0),
					HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center
				};
				HyperlinkButton block = new HyperlinkButton
				{
					Content = t.projTask,
					Margin = new Windows.UI.Xaml.Thickness(130, 5, 0, 0),
					Width = 820,
					HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
				};
				if (t.projCompleted == true)
				{
					block.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
				}
				if (count == 0)
				{
					RelativePanel.SetAlignTopWithPanel(check, true);
					RelativePanel.SetAlignLeftWithPanel(check, true);
					RelativePanel.SetAlignTopWithPanel(block, true);
					RelativePanel.SetAlignRightWithPanel(block, true);
				}
				else
				{
					RelativePanel.SetBelow(check, panel.Children[count - 1]);
					RelativePanel.SetAlignLeftWithPanel(check, true);
					RelativePanel.SetBelow(block, panel.Children[count - 1]);
					RelativePanel.SetAlignRightWithPanel(block, true);
				}
				panel.Children.Add(check);
				panel.Children.Add(block);
				count = count + 2; ;
			}
			foreach (var t in listComplete)
			{
				//t.projTask = t.projTask + " -- " + t.dateCompleted.Date.ToString();
				CheckBox check = new CheckBox
				{
					Tag = t.projTask,
					IsChecked = t.projCompleted,
					Width = 75,
					HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
					Margin = new Windows.UI.Xaml.Thickness(50, 5, 0, 0),
					HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center
				};
				HyperlinkButton block = new HyperlinkButton
				{
					Content = t.projTask,
					Margin = new Windows.UI.Xaml.Thickness(130, 5, 0, 0),
					Width = 820,
					HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
				};
				if (t.projCompleted == true)
				{
					block.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
				}
				if (count == 0)
				{
					RelativePanel.SetAlignTopWithPanel(check, true);
					RelativePanel.SetAlignLeftWithPanel(check, true);
					RelativePanel.SetAlignTopWithPanel(block, true);
					RelativePanel.SetAlignRightWithPanel(block, true);
				}
				else
				{
					RelativePanel.SetBelow(check, panel.Children[count - 1]);
					RelativePanel.SetAlignLeftWithPanel(check, true);
					RelativePanel.SetBelow(block, panel.Children[count - 1]);
					RelativePanel.SetAlignRightWithPanel(block, true);
				}
				panel.Children.Add(check);
				panel.Children.Add(block);
				count = count + 2; ;
			}
		}
		public static async void SaveTaskList()
		{
			StorageFile storageFile = await Globals.storageLocation.CreateFileAsync("taskList", CreationCollisionOption.ReplaceExisting);

			IRandomAccessStream raStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);

			using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
			{
				//Serialize the Session State
				DataContractSerializer serializer = new DataContractSerializer(typeof(List<ToDoList>));
				serializer.WriteObject(outStream.AsStreamForWrite(), Globals.tasks);
				await outStream.FlushAsync();
				outStream.Dispose();
				raStream.Dispose();
			}
		}
		#endregion
	}

	#region List Classes
	//List Classes
	public class ProjectList
	{
		public string projectName { get; set; }
		public DateTime dateCreated { get; set; }
		public DateTime lastModified { get; set; }
		public string imgSource { get; set; }
		public string webSources { get; set; }

		public override string ToString()
		{
			return projectName;
		}
	}
	public class ToDoList
	{
		public string projectName { get; set; }
		public bool projCompleted { get; set; }
		public string projTask { get; set; }
		public DateTime dateCreated { get; set; }
		public DateTime dateCompleted { get; set; }

		public override string ToString()
		{
			return projectName;
		}
	}
	#endregion
}
