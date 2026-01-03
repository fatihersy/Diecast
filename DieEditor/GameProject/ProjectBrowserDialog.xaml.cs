using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DieEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialog.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += ProjectBrowserDialog_Loaded;
		}

        private void ProjectBrowserDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ProjectBrowserDialog_Loaded;
            if(!OpenProject.Projects.Any())
            {
                OpenProjectButton.IsEnabled = false;
				openProjectView.Visibility = Visibility.Hidden;
                ToggleProjectButton_Click(NewProjectButton, new RoutedEventArgs());
			}
		}

        private void ToggleProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if( sender == OpenProjectButton)
            {
                if (NewProjectButton.IsChecked == true)
                {
                    NewProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(0);
                }
                OpenProjectButton.IsChecked = true;
            } 
            else
            {
                if (OpenProjectButton.IsChecked == true)
                {
                    OpenProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(-800, 0, 0, 0);
                }
                NewProjectButton.IsChecked = true;
            }
        }

    }
}
