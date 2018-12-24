using System.Linq;
using App.Metrics.Counter;
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
                string[] tagKeys = invocation.Method.GetParameters().Select(parameter => parameter.Name).ToArray();
                string[] tagValues = invocation.Arguments.Select(argument => argument.ToString()).ToArray();
                MetricTags metricTags = new MetricTags(tagKeys, tagValues);
                invocation.ReturnValue = _metricsProvider.Counter.Instance(counterOptions, metricTags);
            }
        }
    }
}
