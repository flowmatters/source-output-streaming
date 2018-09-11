using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiverSystem.ManagedExtensions;
using SourceOutputStreaming.TimeSeriesStates;
using TIME.Core;
using TIME.DataTypes;
using TIME.DataTypes.TimeSeriesImplementation;

namespace SourceOutputStreaming
{
    public enum StreamingOutputOverwriteOption
    {
        Fail,
        Overwrite,
        Increment
    }

    public abstract class TimeSeriesStateFactory : IDomainObject, ITimeSeriesStateFactory
    {
        public const int DEFAULT_BUFFER_SIZE = 1024; // Time steps

        public int id { get; set; }
        public virtual string Destination { get; set; }
        public StreamingOutputOverwriteOption OverwriteOption { get; set; }
        public int BufferSize { get; set; }
        protected List<TimeSeries> BufferedTimeSeries;
        protected List<BufferedWriteTimeSeriesState> BufferedStates;
        protected List<TimeSeries> ProxyTimeSeries;
        protected List<ProxyTimeSeriesState> ProxyStates;

        private int step;

        protected TimeSeriesStateFactory()
        {
            BufferSize = DEFAULT_BUFFER_SIZE;
            OverwriteOption = StreamingOutputOverwriteOption.Fail;
        }

        public virtual TimeSeries NewTimeSeries(DateTime start, DateTime end, TimeStep ts, string name, Unit units)
        {
            var bufferedState = new BufferedWriteTimeSeriesState(BufferSize,start,end,ts);
            var bufferedTS = new TimeSeries(bufferedState);
            bufferedTS.name = name;
            bufferedTS.units = units;
            BufferedStates.Add(bufferedState);
            BufferedTimeSeries.Add(bufferedTS);

            var proxyState = new ProxyTimeSeriesState(bufferedTS);
            var proxyTimeSeries = new TimeSeries(proxyState);
            proxyTimeSeries.name = name;
            proxyTimeSeries.units = units;
            ProxyStates.Add(proxyState);
            ProxyTimeSeries.Add(proxyTimeSeries);
            return proxyTimeSeries;
        }

        public void NewRun()
        {
            if (OutputFileExists())
            {
                if (OverwriteOption == StreamingOutputOverwriteOption.Fail)
                {
                    throw new Exception("Output file exsits");
                }

                if (OverwriteOption == StreamingOutputOverwriteOption.Increment)
                {
                    do
                    {
                        IncrementFilename();
                    } while (OutputFileExists());
                }
            }

            InitialiseOutputFile();

            BufferedStates = new List<BufferedWriteTimeSeriesState>();
            BufferedTimeSeries = new List<TimeSeries>();
            ProxyStates = new List<ProxyTimeSeriesState>();
            ProxyTimeSeries = new List<TimeSeries>();
            step = 0;
        }

        protected abstract void InitialiseOutputFile();
        protected abstract void IncrementFilename();

        protected virtual bool OutputFileExists()
        {
            return File.Exists(Destination);
        }

        protected abstract void FlushData();
        public abstract void SwitchToReadableStates();
        public virtual void FinaliseOutputs() { }

        public virtual void AfterStep(DateTime timestamp)
        {
            step++;
            if ((step%BufferSize) == 0)
            {
                FlushData();
                BufferedStates.ForEach(s=>s.InitialiseBuffer(step));
            }
        }

        public virtual void AfterRun()
        {
            FlushData();
            FinaliseOutputs();

            SwitchToReadableStates();
            BufferedStates = null;
            BufferedTimeSeries = null;
            ProxyStates = null;
            ProxyTimeSeries = null;
        }
    }
}
