using System;
using TIME.DataTypes;
using TIME.DataTypes.TimeSeriesImplementation;

namespace SourceOutputStreaming.TimeSeriesStates
{
    public class ProxyTimeSeriesState : TimeSeriesState
    {
        public virtual TimeSeries Proxy { get; set; }

        protected ProxyTimeSeriesState()
        {
            
        }

        public ProxyTimeSeriesState(TimeSeries proxy)
        {
            Proxy = proxy;
        }

        public override int itemForTime(DateTime dt)
        {
            return Proxy.itemForTime(dt);
        }

        public override DateTime timeForItem(int i)
        {
            return Proxy.timeForItem(i);
        }

        public override double item(int i)
        {
            return Proxy[i];
        }

        public override void setItem(int i, double v)
        {
            Proxy[i] = v;
        }

        public override TimeSeriesState Clone()
        {
            throw new NotImplementedException();
        }

        public override void init(DateTime startTime, int numEntries)
        {
            Proxy.init(startTime, timeStep,numEntries);
        }

        public override void init(DateTime startTime, DateTime endTime)
        {
            Proxy.init(startTime, endTime, timeStep);
        }

        public override TimeStep timeStep { get { return Proxy.timeStep; } }
        public override int Count { get { return Proxy.Count; } }

        public override DateTime start
        {
            get { return Proxy.Start; }
            set { Proxy.Start = value; }
        }

        public override DateTime end
        {
            get { return Proxy.End; }
            set { Proxy.End = value; }
        }
    }
}
