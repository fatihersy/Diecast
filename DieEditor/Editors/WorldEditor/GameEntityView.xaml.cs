using DieEditor.Components;
using DieEditor.GameProject;
using DieEditor.Utilities;
using System.Windows.Controls;
using System.Windows.Data;

namespace DieEditor.Editors
{
    public partial class GameEntityView : UserControl
    {
        private Action _undoAction;
        private string _propertyName;
        public static GameEntityView Instance { get; private set; }
		public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
            DataContextChanged += (_, __) =>
            {
                if (DataContext != null) 
                {
                    (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName; 
                }
            };
		}

        private Action GetRenameAction()
        {
			var vm = DataContext as MSEntity;
			var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
			return new Action(() =>
			{
				selection.ForEach(item => item.entity.Name = item.Name);
				(DataContext as MSEntity).Refresh();
			});
		}
		private Action GetIsEnabledAction()
		{
			var vm = DataContext as MSEntity;
			var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
			return new Action(() =>
			{
				selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
				(DataContext as MSEntity).Refresh();
			});
		}

		private void OnNameTextBox_KBFocus_Got(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            _undoAction = GetRenameAction();
		}

        private void OnNameTextBox_KBFocus_Lost(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if(_propertyName == nameof(MSEntity.Name) && _undoAction != null)
			{
                Project.UndoRedoManager.Add(new UndoRedoAction(_undoAction, GetRenameAction(), "Entit(y/ies) renamed"));
                _propertyName = null;
			}
            _undoAction = null;
		}

        private void OnIsEnabledCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var undoAction = GetIsEnabledAction();
			var IsChecked = (sender as CheckBox).IsChecked == true;
			(DataContext as MSEntity).IsEnabled = IsChecked;
			Project.UndoRedoManager.Add(new UndoRedoAction(undoAction, GetIsEnabledAction(), IsChecked ? "Entity is enabled" : "Entity is disabled"));
		}
	}
}
