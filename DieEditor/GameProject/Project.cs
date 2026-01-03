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
		public ICommand AddSceneCommand { get; private set; }
		public ICommand RemoveSceneCommand { get; private set; }
		public ICommand UndoCommand { get; private set; }
		public ICommand RedoCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }

		public static Project Load(string file)
        {
            Debug.Assert(System.IO.File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        public static void Save(Project project)
        {
            Serializer.toFile(project, project.FullPath);
            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }
        public void Unload() {
            UndoRedoManager.Reset();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(s => s.IsActive);

            AddSceneCommand = new RelayCommand<object>(x => 
            {
                AddScene($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var index = _scenes.IndexOf(newScene);
                UndoRedoManager.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(index, newScene),
					$"Add Scene {newScene.Name}"
                ));
			});

            RemoveSceneCommand = new RelayCommand<Scene>(scene =>
            {
                var index = _scenes.IndexOf(scene);
                RemoveScene(scene);
                UndoRedoManager.Add(new UndoRedoAction(
                    () => _scenes.Insert(index, scene),
                    () => RemoveScene(scene),
                    $"Remove Scene {scene.Name}"
                ));
            }, scene => !scene.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedoManager.Undo());
            RedoCommand = new RelayCommand<object>(x => UndoRedoManager.Redo());
            SaveCommand = new RelayCommand<object>(x => Save(this));
		}

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }

        public void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
			_scenes.Add(new Scene(this, sceneName));
		}
		public void RemoveScene(Scene scene)
		{
			Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
		}
	}
}
