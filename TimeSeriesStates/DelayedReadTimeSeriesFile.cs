using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIME.DataTypes;
using TIME.Management;

namespace SourceOutputStreaming.TimeSeriesStates
{
    public class DelayedReadTimeSeriesFile
    {
        public DelayedReadTimeSeriesFile(string fn)
        {
            Filename = fn;
        }

        public string Filename { get; set; }

        private TimeSeries[] _data;

        // TODO - need to synchronise!
        public TimeSeries[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = (TimeSeries[]) NonInteractiveIO.Load(Filename);
                }
                return _data;
            }
        }
    }
}
