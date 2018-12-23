using System;

namespace App.Metrics.MetricRecorderFactory
{
    public class MetricFactory
    {
        public MetricFactory()
        {
            
        }

        public TMetric CreateMetric<TMetric>()
        {
            return default(TMetric);
        }
    }
}
