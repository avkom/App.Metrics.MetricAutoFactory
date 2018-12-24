using System.Linq;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using Castle.DynamicProxy;
// ReSharper disable SuspiciousTypeConversion.Global

namespace App.Metrics.MetricAutoFactory
{
    public class MetricInterceptor : IInterceptor
    {
        private readonly IProvideMetrics _metricsProvider;

        public MetricInterceptor(IProvideMetrics metricsProvider)
        {
            _metricsProvider = metricsProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.ReturnType == typeof(ICounter))
            {
                CounterOptions counterOptions = new CounterOptions
                {
                    Name = invocation.Method.Name
                };
                MetricTags metricTags = GetTags(invocation);
                invocation.ReturnValue = _metricsProvider.Counter.Instance(counterOptions, metricTags);
            }
            else if (invocation.Method.ReturnType == typeof(IGauge))
            {
                GaugeOptions counterOptions = new GaugeOptions
                {
                    Name = invocation.Method.Name
                };
                MetricTags metricTags = GetTags(invocation);
                invocation.ReturnValue = _metricsProvider.Gauge.Instance(counterOptions, metricTags);
            }
        }

        private MetricTags GetTags(IInvocation invocation)
        {
            string[] tagKeys = invocation.Method.GetParameters().Select(parameter => parameter.Name).ToArray();
            string[] tagValues = invocation.Arguments.Select(argument => argument.ToString()).ToArray();
            return new MetricTags(tagKeys, tagValues);
        }
    }
}
