using DieEditor.Components;
using DieEditor.GameProject;
using DieEditor.Utilities;
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
            GameEntityView.Instance.DataContext = null; 

			var listBox = sender as ListBox;
            if (listBox != null && e.AddedItems.Count > 0)
            {
                GameEntityView.Instance.DataContext = listBox.SelectedItems[0] as GameEntity;
			}

            var newSelections = listBox.SelectedItems.Cast<GameEntity>().ToList();
            var previousSelections = newSelections
                .Except(e.AddedItems.Cast<GameEntity>())
                .Concat(e.RemovedItems.Cast<GameEntity>())
                .ToList();
            Project.UndoRedoManager.Add(new UndoRedoAction(
                () => // Undo
                { 
                    listBox.UnselectAll();
                    previousSelections.ForEach(sel => (listBox.ItemContainerGenerator.ContainerFromItem(sel) as ListBoxItem).IsSelected = true);
				},
				() => // Redo
				{
					listBox.UnselectAll();
					newSelections.ForEach(sel => (listBox.ItemContainerGenerator.ContainerFromItem(sel) as ListBoxItem).IsSelected = true);
				},
				"Selection changed"
			));
		}
	}
}
