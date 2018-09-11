using System;
using TIME.DataTypes;
using TIME.DataTypes.TimeSeriesImplementation;

namespace SourceOutputStreaming.TimeSeriesStates
{
    public class BufferedWriteTimeSeriesState : TimeSeriesState
    {
        public int CurrentTimestep { get; private set; }
        private int _bufferSize;
        public readonly double[] Buffer;
        public BufferedWriteTimeSeriesState(int bufferSize, DateTime start, DateTime end, TimeStep ts)
        {
            this.start = start;
            timeStep = ts;
            Count = ts.numSteps(start, end);
            _bufferSize = bufferSize;
            Buffer = new double[_bufferSize];
            InitialiseBuffer(0);
        }

        public void InitialiseBuffer(int newTimestep)
        {
            CurrentTimestep = newTimestep;
            for (int i = 0; i < Buffer.Length; i++)
                Buffer[i] = double.NaN;
        }

        public override int itemForTime(DateTime dt)
        {
            return timeStep.numSteps(start, dt) - 1;
        }

        public override DateTime timeForItem(int i)
        {
            return timeStep.add(start, i);
        }

        public override double item(int i)
        {
            if ((i < CurrentTimestep) || (i >= (CurrentTimestep + _bufferSize)))
            {
                throw new ArgumentOutOfRangeException();
            }
            return Buffer[i - CurrentTimestep];
        }

        public override void setItem(int i, double v)
        {
            if ((i < CurrentTimestep) || (i >= (CurrentTimestep + _bufferSize)))
            {
                throw new ArgumentOutOfRangeException();
            }
            Buffer[i - CurrentTimestep] = v;
        }

        public override TimeSeriesState Clone()
        {
            throw new NotImplementedException();
        }

        public override void init(DateTime startTime, int numEntries)
        {
            throw new NotImplementedException();
        }

        public override void init(DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public override TimeStep timeStep { get; }
        public override int Count { get; }

        public override DateTime start { get; set; }

        public override DateTime end
        {
            get { return timeStep.add(start, Count - 1); }
            set { throw new NotSupportedException(); }
        }
    }
}
