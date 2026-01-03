using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieEditor.Utilities
{
    public interface IUndoableAction
    {
        string Name { get; }
        void Undo();
        void Redo();
    }
    public class UndoRedoAction : IUndoableAction
    {
        private readonly Action _undoAction;
        private readonly Action _redoAction;
        public string Name { get; }
        public void Redo() => _redoAction();
        public void Undo() => _undoAction();
        public UndoRedoAction(string name)
        {
            Name = name;
		}
		public UndoRedoAction(Action undoAction, Action redoAction, string name) : this(name)
		{
            Debug.Assert(undoAction != null && redoAction != null);
			_undoAction = undoAction;
			_redoAction = redoAction;
		}

        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name) :
            this(
                () => // Undo
                {
                    var propInfo = instance.GetType().GetProperty(property);
                    propInfo.SetValue(instance, undoValue);
                },
                () => // Redo
                {
                    var propInfo = instance.GetType().GetProperty(property);
                    propInfo.SetValue(instance, redoValue);
                },
                    name
				)
        {}
	}

	public class UndoRedo
    {
        private bool _enableAdd = true;
		private readonly ObservableCollection<IUndoableAction> _redoList = new ObservableCollection<IUndoableAction>();
        private readonly ObservableCollection<IUndoableAction> _undoList = new ObservableCollection<IUndoableAction>();

        public ReadOnlyObservableCollection<IUndoableAction> RedoList { get; }
        public ReadOnlyObservableCollection<IUndoableAction> UndoList { get; }

        public void Reset()
        {
            _redoList.Clear();
            _undoList.Clear();
        }
        public void Add(IUndoableAction action)
        {
            if (_enableAdd)
            {
				_undoList.Add(action);
				_redoList.Clear();
			}
		}

		public void Undo()
        {
            if (_undoList.Count > 0)
            {
                var action = _undoList[^1];
                _enableAdd = false;
				action.Undo();
                _enableAdd = true;
				_undoList.RemoveAt(_undoList.Count - 1);
                _redoList.Add(action);
            }
		}
        public void Redo()
        {
            if (_redoList.Count > 0)
            {
                var action = _redoList[^1];
                _enableAdd = false;
                action.Redo();
                _enableAdd = true;
				_redoList.RemoveAt(_redoList.Count - 1);
                _undoList.Add(action);
            }
        }

		public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoableAction>(_redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoableAction>(_undoList);
        }
    }
}