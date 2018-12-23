namespace App.Metrics.MetricAutoFactory
{
    public interface IMetricFactory
    {
        TMetric CreateMetric<TMetric>() where TMetric : class;
    }
}
