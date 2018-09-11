using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RiverSystem;
using RiverSystem.ManagedExtensions;
using TIME.Core;
using TIME.DataTypes;
using TIME.DataTypes.TimeSeriesImplementation;
using TIME.ManagedExtensions;
using TIME.Management;
using TIME.ScenarioManagement;
using TIME.ScenarioManagement.RunManagement;

namespace SourceOutputStreaming
{
    public class StreamingOutputManager :
        IPluginInitialise<RiverSystemScenario>,
        IPluginRunStart<RiverSystemScenario>,
        IPluginRunEnd<RiverSystemScenario>,
        IPluginAfterStep<RiverSystemScenario>
    {
        public StreamingOutputOverwriteOption OverwriteOption
        {
            get { return StateFactory.OverwriteOption; }
            set { StateFactory.OverwriteOption = value; }
        }

        public ITimeSeriesStateFactory StateFactory
        {
            get;
            set;
        }

        public TimeSeries MakeStreamingTimeSeries(
            DateTime start, DateTime end, TimeStep ts, string name, Unit units)
        {
            return StateFactory.NewTimeSeries(start, end, ts, name, units);
        }

        public void ScenarioInitialise(RiverSystemScenario scenario, RunningConfiguration config)
        {
            StateFactory.NewRun();

            scenario.RunManager.CurrentConfiguration.MakeResultTimeSeries = MakeStreamingTimeSeries;
        }

        public void ScenarioRunStart(RiverSystemScenario scenario)
        {
        }

        public void ScenarioRunEnd(RiverSystemScenario scenario)
        {
            StateFactory.AfterRun();
        }

        public void ScenarioAfterStep(RiverSystemScenario scenario, DateTime step)
        {
            StateFactory.AfterStep(step);
        }

        public int id { get; set; }

        public static StreamingOutputManager EnableStreaming(RiverSystemScenario scenario, string destinationFilename)
        {
            if (scenario.PluginDataModels.OfType<StreamingOutputManager>().Any())
            {
                return scenario.PluginDataModels.OfType<StreamingOutputManager>().First();
            }
            var streamer = new StreamingOutputManager();

            var factoryType =
                AssemblyManager.FindTypes(typeof(ITimeSeriesStateFactory))
                    .Where(t =>
                    {
                        var ext = t.GetAttribute<FileExtensionAttribute>()?.Extension;
                        if (ext == null)
                        {
                            return false;
                        }
                        return destinationFilename.EndsWith(ext);
                    })
                    .FirstOrDefault();

            if (factoryType == null)
            {
                throw new ArgumentException("Unknown file extension: " + destinationFilename);
            }
            streamer.StateFactory = (ITimeSeriesStateFactory) Activator.CreateInstance(factoryType);
            streamer.StateFactory.BufferSize = TimeSeriesStateFactory.DEFAULT_BUFFER_SIZE;
            streamer.StateFactory.Destination = destinationFilename;
            scenario.PluginDataModels.Add(streamer);
            return streamer;
        }

        public static void DisableStreaming(RiverSystemScenario scenario)
        {
            if (!scenario.PluginDataModels.OfType<StreamingOutputManager>().Any())
            {
                return;
            }

            scenario.PluginDataModels.RemoveAll(plugin => plugin is StreamingOutputManager);
        }
    }
}
