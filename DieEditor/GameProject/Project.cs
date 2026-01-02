using DieEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

		public static Project Current => Application.Current.MainWindow.DataContext as Project;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
			}
            ActiveScene = Scenes.FirstOrDefault(s => s.IsActive);
		}

		public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
		}
    }
}
