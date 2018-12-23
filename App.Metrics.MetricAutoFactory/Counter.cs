using App.Metrics.Counter;

namespace App.Metrics.MetricAutoFactory
{
    public class Counter : ICounter
    {
        private readonly IMeasureCounterMetrics _counter;
        private CounterOptions _counterOptions;
        private MetricTags _metricTags;

        public Counter(IMeasureCounterMetrics counter, CounterOptions counterOptions, MetricTags metricTags)
        {
            _counter = counter;
            _counterOptions = counterOptions;
            _metricTags = metricTags;
        }

        public void Decrement(long decrement = 1)
        {
            _counter.Decrement(_counterOptions, _metricTags, decrement);
        }

        public void Increment(long increment = 1)
        {
            _counter.Increment(_counterOptions, _metricTags, increment);
        }
    }
}
