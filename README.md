# MetricAutoFactory

This is a library that simplifies defining and recording metrics for [App.Metrics](https://www.app-metrics.io/) library.

## Without `MetricAutoFactory`

```c#
IMetrics metrics = AppMetrics.CreateDefaultBuilder().Build();


public static class MyMetricsRegistry
{
    public static CounterOptions RequestCounter => new CounterOptions
    {
        Name = "RequestCount",
        MeasurementUnit = Unit.Calls,
    };
}


public class BooksController : Controller
{
    private readonly IMetrics _metrics;
    
    public BooksController(IMetrics metrics)
    {
        _metrics = metrics;
    }
    
    public void CreateBook(BookModel book)
    {
        // Some application code

        MetricTags tags = new MetricTags(new[] { "endpoint", "statusCode" }, new[] { "CreateBook", this.Response.StatusCode.ToString() });
        _metrics.Measure.Counter.Increment(MyMetricsRegistry.RequestCounter, tags);
    }
}
```

## With `MetricAutoFactory`

```c#
IMetrics metrics = AppMetrics.CreateDefaultBuilder().Build();
IMetricsAutoFactory metricsAutoFactory = new MetricsAutoFactory(metrics.Measure);

// MetricsAutoFactory automaticaly generates an implementation of IMyMetrics  
IMyMetrics myMetrics = metricsAutoFactory.CreateMetric<IMyMetrics>();


public interface IMyMetrics
{
    ICounter RequestCount(string endpoint, int httpStatus);
}


public class BooksController : Controller
{
    private readonly IMyMetrics _myMetrics;
    
    public BooksController(IMyMetrics myMetrics)
    {
        _myMetrics = myMetrics;
    }
    
    public void CreateBook(BookModel book)
    {
        // Some application code

        // Autocomplete shows tag names and types: ICounter RequestCount(string endpoint, int httpCode) 
        _myMetrics.RequestCount("CreateBook", this.Response.StatusCode).Increment();
    }
}
```