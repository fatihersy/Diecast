using DieEditor.Components;
using DieEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace DieEditor.GameProject
{
	[DataContract]
    public class Scene : ViewModelBase
	{
		private string _name;
		[DataMember]
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		[DataMember]
		public Project Project { get; private set; }

		private bool _isActive;
		[DataMember]
		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(nameof(IsActive));
				}
			}
		}

		[DataMember(Name = nameof(GameEntities))]
		private readonly ObservableCollection<Components.GameEntity> _gameEntities = new ObservableCollection<Components.GameEntity>(); 
		public ReadOnlyObservableCollection<Components.GameEntity> GameEntities { get; private set; }

		public ICommand AddGameEntitiyCommand { get; private set; }
		public ICommand RemoveGameEntitiyCommand { get; private set; }

		private void AddGameEntitiy(GameEntity entity)
		{
			Debug.Assert(!_gameEntities.Contains(entity));
			_gameEntities.Add(entity);
		}
		private void RemoveGameEntitiy(GameEntity entity)
		{
			Debug.Assert(_gameEntities.Contains(entity));
			_gameEntities.Remove(entity);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (_gameEntities != null)
			{
				GameEntities = new ReadOnlyObservableCollection<Components.GameEntity>(_gameEntities);
				OnPropertyChanged(nameof(GameEntities));
			}

			AddGameEntitiyCommand = new RelayCommand<GameEntity>(entitiy =>
			{
				AddGameEntitiy(entitiy);
				var index = _gameEntities.Count - 1;

				Project.UndoRedoManager.Add(new UndoRedoAction(
					() => RemoveGameEntitiy(entitiy),
					() => _gameEntities.Insert(index, entitiy),
					$"Added Entitiy:{entitiy.Name} to index"
				));
			});

			RemoveGameEntitiyCommand = new RelayCommand<GameEntity>(entitiy =>
			{
				var index = _gameEntities.IndexOf(entitiy);
				RemoveGameEntitiy(entitiy);

				Project.UndoRedoManager.Add(new UndoRedoAction(
					() => _gameEntities.Insert(index, entitiy),
					() => _gameEntities.Remove(entitiy),
					$"Removed Entity:{entitiy.Name}"
				));
			});
		}
		public Scene(Project project, string name)
		{
			Debug.Assert(project != null);
			Project = project;
			Name = name;
			OnDeserialized(new StreamingContext());
		}
	}
}