﻿using System.Linq;
using App.Metrics.Counter;
using Castle.DynamicProxy;
// ReSharper disable SuspiciousTypeConversion.Global

namespace App.Metrics.MetricAutoFactory
{
    public class MetricInterceptor : IInterceptor
    {
        private readonly IMeasureMetrics _measureMetrics;

        public MetricInterceptor(IMeasureMetrics measureMetrics)
        {
            _measureMetrics = measureMetrics;
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
                invocation.ReturnValue = new Counter(_measureMetrics.Counter, counterOptions, metricTags);
            }
        }
    }
}
