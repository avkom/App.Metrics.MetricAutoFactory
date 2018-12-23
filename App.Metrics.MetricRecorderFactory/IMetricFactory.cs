namespace App.Metrics.MetricRecorderFactory
{
    public interface IMetricFactory
    {
        TMetric CreateMetric<TMetric>() where TMetric : class;
    }
}
