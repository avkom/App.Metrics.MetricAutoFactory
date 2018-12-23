namespace App.Metrics.MetricRecorderFactory
{
    public interface IGauge
    {
        void Increment(double increment = 1.0);
        void Decrement(double decrement = 1.0);
        void SetValue(double value);
    }
}
