using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAspNetCore.Models
{
    public class IdWorker
    {
        public const long Twepoch = 1288834974657L;


        const int WorkerIdBits = 5;
        const int DatacenterIdBits = 5;
        const int SequenceBits = 12;
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);


        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);


        private long _sequence = 0L;
        private long _lastTimestamp = -1L;


        public IdWorker(long workerId, long datacenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            _sequence = sequence;

            // sanity check for workerId
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException(String.Format("worker Id can't be greater than {0} or less than 0", MaxWorkerId));
            }


            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(String.Format("datacenter Id can't be greater than {0} or less than 0", MaxDatacenterId));
            }


        }

        public long WorkerId { get; protected set; }
        public long DatacenterId { get; protected set; }


        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }




        readonly object _lock = new Object();

        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();


                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(String.Format(
                        "Clock moved backwards.  Refusing to generate id for {0} milliseconds", _lastTimestamp - timestamp));
                }


                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }


                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | _sequence;

                return id;
            }
        }


        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public virtual long TimeGen()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}
