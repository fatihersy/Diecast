using DieEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
		private readonly string _templatePath = @"..\..\DieEditor\ProjectTemplates\";

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        private string _projectName = " % Name % ";
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\DieProjects\";
        public string ProjectName         {
            get { return _projectName; }
            set
            {
                if (_projectName != value)
                {
					_projectName = value;
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
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

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
					
                    _projectTemplates.Add(template);
                }
            } 
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                // TODO: Log
            }
        }
    }
}
