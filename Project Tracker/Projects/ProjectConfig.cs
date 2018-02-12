using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Project_Tracker.Projects
{
	public partial class ProjectShell : Page
	{
		public string FEATURE_NAME = Globals.currentProject;

		List<Scenario> scenarios = new List<Scenario>
		{
			new Scenario() {Title="Basic Info", ClassType=typeof(Projects.ProjectInfo)},
			new Scenario() {Title="To Do / Upgrades", ClassType=typeof(Projects.ProjectToDo)},
			new Scenario() {Title="Return To Home", ClassType=typeof(MainPage)},
		};
	}

	public class Scenario
	{
		public string Title { get; set; }
		public Type ClassType { get; set; }
	}
}
