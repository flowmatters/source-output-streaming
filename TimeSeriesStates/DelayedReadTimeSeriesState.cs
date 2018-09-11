using System;
using TIME.DataTypes;

namespace SourceOutputStreaming.TimeSeriesStates
{
    public class DelayedReadTimeSeriesState : ProxyTimeSeriesState
    {
        private readonly DelayedReadTimeSeriesFile _file;
        private readonly int _index;
        private TimeSeries _proxy;

        public override TimeSeries Proxy
        {
            get
            {
                if (_proxy == null)
                {
                    _proxy = _file.Data[_index];
                }
                return _proxy;
            }
            set { throw new NotImplementedException(); }
        }

        public DelayedReadTimeSeriesState(DelayedReadTimeSeriesFile f, int i)
        {
            _file = f;
            _index = i;
        }
    }
}
