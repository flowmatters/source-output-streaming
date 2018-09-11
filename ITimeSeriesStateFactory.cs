using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIME.Core;
using TIME.DataTypes;
using TIME.DataTypes.TimeSeriesImplementation;

namespace SourceOutputStreaming
{
    public interface ITimeSeriesStateFactory
    {
        string Destination { get; set; }
        StreamingOutputOverwriteOption OverwriteOption { get; set; }
        int BufferSize { get; set; }
        TimeSeries NewTimeSeries(DateTime start, DateTime end, TimeStep ts, string name, Unit units);
        void AfterStep(DateTime step);
        void AfterRun();
        void NewRun();
    }
}
