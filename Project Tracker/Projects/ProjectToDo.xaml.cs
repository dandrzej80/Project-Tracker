using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Project_Tracker.Projects
{
	public sealed partial class ProjectToDo : Page
	{
		string oldTask = "";

		public ProjectToDo()
		{
			this.InitializeComponent();
		}
		public void Link_Click(object sender, RoutedEventArgs e)
		{
			HyperlinkButton link = sender as HyperlinkButton;
			AddTaskButton.Content = "Edit...";
			AddTaskBox.Text = link.Content.ToString();
			oldTask = link.Content.ToString();
		}
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			Globals.DisplayTasks(ListPanel);

			foreach (HyperlinkButton button in FindVisualChildren<HyperlinkButton>(this))
			{
				button.Click += Link_Click;
			}
			foreach (CheckBox check in FindVisualChildren<CheckBox>(this))
			{
				check.Unchecked += Check_Unchecked;
				check.Checked += Check_Checked;
			}
		}
		private void Check_Checked(object sender, RoutedEventArgs e)
		{
			//Change Checked Task Format
			CheckBox check = sender as CheckBox;
			int c = 0;
			foreach (var t in Globals.tasks)
			{
				if(check.Tag.ToString() == t.projTask)
				{
					Globals.tasks[c].projCompleted = true;
					Globals.tasks[c].dateCompleted = DateTime.Now;
					break;
				}
				c++;
			}

			//Save List
			Globals.SaveTaskList();

			//Sort and Redisplay List
			Globals.DisplayTasks(ListPanel);
			Frame.Navigate(typeof(ProjectToDo));
		}
		private void Check_Unchecked(object sender, RoutedEventArgs e)
		{
			//Change Checked Task Format
			CheckBox check = sender as CheckBox;
			int c = 0;
			foreach (var t in Globals.tasks)
			{
				if (check.Tag.ToString() == t.projTask)
				{
					Globals.tasks[c].projCompleted = false;
					Globals.tasks[c].dateCompleted = Globals.tasks[c].dateCreated;
					break;
				}
				c++;
			}

			//Save List
			Globals.SaveTaskList();

			//Sort and Redisplay List
			Globals.DisplayTasks(ListPanel);
			Frame.Navigate(typeof(ProjectToDo));
		}
		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}
		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			//Add or Change Task
			bool exist = false;
			int count = 0;
			foreach (var t in Globals.tasks)
			{
				if (t.projTask == oldTask)
				{
					exist = true;
					break;
				}
				count++;
			}
			if ((AddTaskBox.Text != "") && (AddTaskBox.Text != null))
			{
				if (exist)
				{
					Globals.tasks[count].projTask = AddTaskBox.Text;
				}
				else
				{
					Globals.tasks.Add(new ToDoList
					{
						projectName = Globals.currentProject,
						projCompleted = false,
						projTask = AddTaskBox.Text,
						dateCreated = DateTime.Now,
						dateCompleted = DateTime.Now
					});
				}

				//Save List
				Globals.SaveTaskList();

				//Clear Box
				AddTaskBox.Text = "";
				AddTaskButton.Content = "Add...";
				oldTask = "";

				//Sort, Separate and Display List
				Globals.DisplayTasks(ListPanel);
				Frame.Navigate(typeof(ProjectToDo));
			}
		}
		private void RemoveTask_Click(object sender, RoutedEventArgs e)
		{
			//Remove Task
			List<ToDoList> list = new List<ToDoList>();
			foreach (var t in Globals.tasks)
			{
				if (t.projTask != AddTaskBox.Text)
				{
					list.Add(t);
				}
			}
			Globals.tasks = list;
			AddTaskBox.Text = "";

			//Save List
			Globals.SaveTaskList();

			//Sort and Redisplay List
			Globals.DisplayTasks(ListPanel);
			Frame.Navigate(typeof(ProjectToDo));
		}
	}

}
