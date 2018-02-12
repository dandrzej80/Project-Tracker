using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Project_Tracker
{
	public sealed partial class AddProjectDialog : ContentDialog
	{
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
		   "Text", typeof(string), typeof(AddProjectDialog), new PropertyMetadata(default(string)));

		public AddProjectDialog()
		{
			this.InitializeComponent();
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}
