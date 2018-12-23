using Castle.DynamicProxy;

namespace App.Metrics.MetricRecorderFactory
{
    public class MetricFactory : IMetricFactory
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly IInterceptor _metricInterceptor;

        public MetricFactory(IMeasureMetrics measureMetrics)
        {
            _proxyGenerator = new ProxyGenerator();
            _metricInterceptor = new MetricInterceptor(measureMetrics);
        }

        public TMetric CreateMetric<TMetric>() where TMetric: class
        {
            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<TMetric>(_metricInterceptor);
        }
    }
}
