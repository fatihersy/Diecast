using DieEditor.GameProject;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace DieEditor.Editors
{
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += OnWorldEditorViewLoaded;
		}

        private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnWorldEditorViewLoaded;
            Focus();
            ((INotifyCollectionChanged)Project.UndoRedoManager.UndoList).CollectionChanged += (s, e) => Focus();
            ((INotifyCollectionChanged)Project.UndoRedoManager.RedoList).CollectionChanged += (s, e) => Focus();
		}
    }
}
