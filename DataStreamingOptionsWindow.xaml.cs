using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using RiverSystem;
using RiverSystem.Controls.Common.RelativePathLoader;
using RiverSystem.ManagedExtensions;
using SourceOutputStreaming;
using TIME.Management;
using TIME.ScenarioManagement;
using RiverSystemConfiguration = RiverSystem.RiverSystemConfiguration;

namespace FlowMatters.Source.HDF5IO
{
    /// <summary>
    /// Interaction logic for DataStreamingOptionsWindow.xaml
    /// </summary>
    public partial class DataStreamingOptionsWindow : Window, IScenarioHandler<RiverSystemScenario>
    {
        public bool StreamingEnabled { get; set; }

        public RiverSystemScenario Scenario { get; set; }

        public int TimeWindow { get; set; }
        public int PrecisionIndex { get; set; }
        public int OverwriteIndex { get; set; }
        public RelativePathFileSelectorViewModel RelativePathFileSelectorViewModel { get; set; }
        private Type[] implementations;

        public DataStreamingOptionsWindow(RiverSystemScenario scenario)
        {
            Scenario = scenario;
            Config = Scenario.CurrentRiverSystemConfiguration;
            StreamingEnabled = Scenario.AllPluginDataModels.OfType<StreamingOutputManager>().Any();

            var project = scenario.Project;
            var projectIsSaved = !string.IsNullOrEmpty(project.FileName);
            implementations = AssemblyManager.FindTypes(typeof(ITimeSeriesStateFactory)).ToArray();
            var defaultExtension = implementations.Select(t=>t.GetAttribute<FileExtensionAttribute>()?.Extension).FirstOrDefault(e => e!=null);
            var filters = implementations.Select(t =>
            {
                var ext = t.GetAttribute<FileExtensionAttribute>()?.Extension;
                var desc = t.GetAttribute<DescriptionAttribute>()?.Description;
                if ((ext == null) || (desc == null))
                    return null;
                return string.Format("{0}|*{1}", desc, ext);
            }).Where(f => f != null).ToArray();
            RelativePathFileSelectorViewModel = new RelativePathFileSelectorViewModel(projectIsSaved, "RunResults"+defaultExtension,
                currentProjectPath: projectIsSaved? project.FullFilename:"",
                filter:string.Join("|",filters),
                saveSelector:true);
            InitializeComponent();
        }

        public RiverSystemConfiguration Config { get; set; }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            ApplyChanges();
            Close();
        }

        private void ApplyChanges()
        {
//            MessageBox.Show("This form does nothing at the moment. Just sayin'");
            if (StreamingEnabled)
            {
                StreamingOutputManager.EnableStreaming(Scenario, RelativePathFileSelectorViewModel.FullPath);
            }
            else
            {
                StreamingOutputManager.DisableStreaming(Scenario);
            }
        }
    }
}
