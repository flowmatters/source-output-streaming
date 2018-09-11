using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceOutputStreaming.TimeSeriesStates;
using TIME.Core;
using TIME.DataTypes;
using TIME.DataTypes.IO.CsvFileIo;
using TIME.DataTypes.TimeSeriesImplementation;
using TIME.Management;

namespace SourceOutputStreaming
{
    [FileExtension(".res.csv"),Description("Comma Separate Values with separate metadata header")]
    public class ResCsvStreamingFactory : TimeSeriesStateFactory
    {
        private int _fnIncrement;
        private string _origFilename;
        private string _filename;

        public override string Destination
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
                _origFilename = value;
                _fnIncrement = 0;
            }
        }

        private ResultsCsvIoV2 _writer;
        private bool _headerWritten;
        private StreamWriter _streamWriter;

        public ResCsvStreamingFactory()
        {
            Destination = "run_results.res.csv";
            _writer = new ResultsCsvIoV2();
        }

        protected override void InitialiseOutputFile()
        {
            _streamWriter = File.CreateText(Destination);
        }

        protected override void IncrementFilename()
        {
            _fnIncrement++;
            _filename = _origFilename.Substring(0, _origFilename.Length - 8) + _fnIncrement.ToString() + ".res.csv";
        }

        protected override void FlushData()
        {
            if (BufferedTimeSeries == null)
            {
                throw new Exception("Run not initialised");
            }

            if (!_headerWritten)
            {
                _writer.SetDateTimeFormatter(BufferedTimeSeries.ToArray());
                _writer.WriteHeader(_streamWriter,BufferedTimeSeries.ToArray());
                _headerWritten = true;
            }

            var timestep = BufferedTimeSeries.First().timeStep;
            int currentTimestep = BufferedStates.First().CurrentTimestep;
            int numRows = Math.Min(BufferedTimeSeries.First().itemCount()-currentTimestep,BufferSize);
            var firstEntry = BufferedTimeSeries.First();
            var sb = new StringBuilder();
            var lines = new List<string>();

            for (var i = 0; i < numRows; i++)
            {
                sb.Clear();
                sb.Append(_writer.Formatter.toString(firstEntry.timeForItem(i+ currentTimestep)));
                foreach (var t in BufferedTimeSeries)
                {
                    sb.Append(Delimiter);
                    var val = t[i+currentTimestep];
                    var strval = val == t.NullValue ? _writer.NullValueString : val.ToString(CultureInfo.InvariantCulture);
                    sb.Append(strval);
                }

                lines.Add(sb.ToString());
            }

            foreach (var l in lines)
                _streamWriter.WriteLine(l);
        }

        public override void FinaliseOutputs()
        {
            base.FinaliseOutputs();
            _streamWriter.Close();
        }

        public override void SwitchToReadableStates()
        {
            var toBeReloaded = new DelayedReadTimeSeriesFile(Destination);
            for (var i = 0; i < ProxyStates.Count; i++)
            {
                var delayedReadState = new DelayedReadTimeSeriesState(toBeReloaded,i);
                ProxyStates[i].Proxy = new TimeSeries(delayedReadState); // TODO - look at simplifying this chain...
            }
        }

        public char Delimiter { get; } = ',';
    }
}
