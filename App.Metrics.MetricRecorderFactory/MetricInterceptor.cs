using System.Linq;
using App.Metrics.Counter;
using Castle.DynamicProxy;

namespace App.Metrics.MetricRecorderFactory
{
    public class MetricInterceptor : IInterceptor
    {
        private IMeasureMetrics _measureMetrics;

        public MetricInterceptor(IMeasureMetrics measureMetrics)
        {
            _measureMetrics = measureMetrics;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.ReturnType is ICounter)
            {
                CounterOptions counterOptions = new CounterOptions
                {
                    Name = invocation.Method.Name
                };
                string[] tagKeys = invocation.Method.GetParameters().Select(parameter => parameter.Name).ToArray();
                string[] tagValues = invocation.Arguments.Select(argument => argument.ToString()).ToArray();
                MetricTags metricTags = new MetricTags(tagKeys, tagValues);
                invocation.ReturnValue = new Counter(_measureMetrics.Counter, counterOptions, metricTags);
            }

            invocation.Proceed();
        }
    }
}
