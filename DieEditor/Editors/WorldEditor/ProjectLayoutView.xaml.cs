using DieEditor.Components;
using DieEditor.GameProject;
using System.Windows;
using System.Windows.Controls;

namespace DieEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void AddGameEntitiyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntitiyCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
		}

        private void OnGameEntitiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null && e.AddedItems.Count > 0)
            {
                GameEntityView.Instance.DataContext = e.AddedItems[0] as GameEntity;
			}
		}
	}
}
