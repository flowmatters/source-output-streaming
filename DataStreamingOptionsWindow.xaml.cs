using System.Linq;
using System.Windows;
using RiverSystem;
using RiverSystem.Controls.Common.RelativePathLoader;
using SourceOutputStreaming;
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

        public DataStreamingOptionsWindow(RiverSystemScenario scenario)
        {
            Scenario = scenario;
            Config = Scenario.CurrentRiverSystemConfiguration;
            StreamingEnabled = Scenario.AllPluginDataModels.OfType<StreamingOutputManager>().Any();

            var project = scenario.Project;
            var projectIsSaved = !string.IsNullOrEmpty(project.FileName);
            RelativePathFileSelectorViewModel = new RelativePathFileSelectorViewModel(projectIsSaved, "RunResults.h5",
                currentProjectPath: projectIsSaved? project.FullFilename:"",
                filter:"HDF5 Files|*.h5",
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
