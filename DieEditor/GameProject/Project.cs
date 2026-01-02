using DieEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace DieEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".die";

        [DataMember]
        public string Name { get; private set; } = "New Project";

        [DataMember]
        public string Path { get; private set; }

        public string FullPath => $"{Path}{Name}{Extension}";
		public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedoManager { get; } = new UndoRedo();

		[DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }
        private Scene _activeScene;

        [DataMember]
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
		}
		public ICommand AddScene { get; private set; }
		public ICommand RemoveScene { get; private set; }
		public ICommand Undo { get; private set; }
		public ICommand Redo { get; private set; }

		public static Project Load(string file)
        {
            Debug.Assert(System.IO.File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        public static void Save(Project project)
        {
            Serializer.toFile(project, project.FullPath);
        }
        public void Unload() { }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(s => s.IsActive);

            AddScene = new RelayCommand<object>(x => 
            {
                AddSceneInternal($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var index = _scenes.IndexOf(newScene);
                UndoRedoManager.Add(new UndoRedoAction(
                    () => RemoveSceneInternal(newScene),
                    () => _scenes.Insert(index, newScene),
					$"Add Scene {newScene.Name}"
                ));
			});

            RemoveScene = new RelayCommand<Scene>(scene =>
            {
                var index = _scenes.IndexOf(scene);
                RemoveSceneInternal(scene);
                UndoRedoManager.Add(new UndoRedoAction(
                    () => _scenes.Insert(index, scene),
                    () => RemoveSceneInternal(scene),
                    $"Remove Scene {scene.Name}"
                ));
            }, scene => !scene.IsActive);

            Undo = new RelayCommand<object>(x => UndoRedoManager.Undo());
            Redo = new RelayCommand<object>(x => UndoRedoManager.Redo());
		}

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }

        public void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
			_scenes.Add(new Scene(this, sceneName));
		}
		public void RemoveSceneInternal(Scene scene)
		{
			Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
		}
	}
}
