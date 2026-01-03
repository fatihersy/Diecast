using DieEditor.GameProject;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DieEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
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
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<GameComponent> _components = new ObservableCollection<GameComponent>();
        public ReadOnlyObservableCollection<GameComponent> Components { get; private set; }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
				Components = new ReadOnlyObservableCollection<GameComponent>(_components);
                OnPropertyChanged(nameof(Components));
			}
		}

		public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
		}
    }
}
