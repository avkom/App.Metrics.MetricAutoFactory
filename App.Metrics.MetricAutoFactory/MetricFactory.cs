using Castle.DynamicProxy;

namespace App.Metrics.MetricAutoFactory
{
    public class MetricFactory : IMetricFactory
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly IInterceptor _metricInterceptor;

        public MetricFactory(IMetrics metrics)
        {
            _proxyGenerator = new ProxyGenerator();
            _metricInterceptor = new MetricInterceptor(metrics.Provider);
        }

        public TMetric CreateMetric<TMetric>() where TMetric: class
        {
            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<TMetric>(_metricInterceptor);
        }
    }
}
