namespace App.Metrics.MetricAutoFactory
{
    public interface ICounter
    {
        void Increment(long increment = 1);
        void Decrement(long decrement = 1);
    }
}
