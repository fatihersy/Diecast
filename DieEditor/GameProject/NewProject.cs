using DieEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace DieEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
		[DataMember]
		public byte[] Icon { get; set; }
		[DataMember]
		public byte[] Screenshot { get; set; }
		[DataMember]
        public string IconFilePath { get; set; }
		[DataMember]
		public string ScreenshotFilePath { get; set; }
		[DataMember]
		public string ProjectFilePath { get; set; }
	}

    class NewProject : ViewModelBase
	{
		// TODO: get the path from the installation location
		private readonly string _templatePath = @$"..\..\..\DieEditor\ProjectTemplates\";

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        private string _projectName = "New Project";
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\DieProjects\";
        public string ProjectName         {
            get { return _projectName; }
            set
            {
                if (_projectName != value)
                {
					_projectName = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectName));
                }
            }
        }
        public string ProjectPath
        {
            get { return _projectPath; }
            set
            {
                if (_projectPath != value)
                {
					_projectPath = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
			set
            {
                if (_isValid != value)
                {
                    _isValid = value;
					OnPropertyChanged(nameof(IsValid));
                }
            }
		}

        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
					OnPropertyChanged(nameof(ErrorMsg));
                }
            }
		}

		private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += ProjectName;

            IsValid = false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Project name cannot be empty.";
                return false;
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                ErrorMsg = "Project name contains invalid characters.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Project path cannot be empty.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                ErrorMsg = "Project path contains invalid characters.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "A project with the same name already exists and not empty";
			}
            else
            {
				ErrorMsg = string.Empty;
                IsValid = true;
			}

			return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!IsValid)
            {
                return string.Empty;
			}
            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
			var path = $@"{ProjectPath}{ProjectName}\";

            try 
            { 
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
				foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
				}
                var dirInfo = new DirectoryInfo(path + @".die\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
				File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                projectXml = string.Format(projectXml, ProjectName, path);
                File.WriteAllText(projectPath, projectXml);
				return path;
			}
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                
                return string.Empty;
            }
		}


		public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    
                    template.IconFilePath       = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
					
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "ss.png"));
					template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);

                    template.ProjectFilePath   = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

					_projectTemplates.Add(template);
				}
				ValidateProjectPath();
			} 
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, "Failed to create project");
                throw;
			}
        }
    }
}
