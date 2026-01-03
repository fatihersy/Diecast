using DieEditor.GameProject;
using DieEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace DieEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
	{
        private bool _isEnabled = true;

        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
				}
            }
		}

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
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<GameComponent> _components = new ObservableCollection<GameComponent>();
        public ReadOnlyObservableCollection<GameComponent> Components { get; private set; }

        public ICommand RenameCommand { get; set; }
		public ICommand IsEnabledCommand { get; set; }

		[OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
				Components = new ReadOnlyObservableCollection<GameComponent>(_components);
                OnPropertyChanged(nameof(Components));
			}

            RenameCommand = new RelayCommand<string>(newName => 
            {
                var oldName = _name;
                Name = newName;

                Project.UndoRedoManager.Add(
                    new UndoRedoAction(nameof(Name), this, oldName, newName, $"Entity:{oldName} renamed to {newName}")
                );
			}, x => x != _name);

			IsEnabledCommand = new RelayCommand<bool>(newValue =>
			{
				var oldValue = _isEnabled;
				IsEnabled = newValue;

				Project.UndoRedoManager.Add(
					new UndoRedoAction(nameof(IsEnabled), this, oldValue, newValue, newValue ? $"Entity:{Name} is Enabled" : $"Entity:{Name} is Disabled")
				);
			});
		}

		public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
		}
    }
}
