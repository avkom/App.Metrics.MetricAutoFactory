using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using App.Metrics.Counter;

namespace App.Metrics.MetricAutoFactory.Tests
{
    [TestClass]
    public class MetricFactoryTests
    {
        private Mock<IMeasureCounterMetrics> _measureCounterMetricsMock;
        private Mock<IMeasureMetrics> _measureMetricsMock;
        private IMetricFactory _metricFactory;

        [TestInitialize]
        public void InitializeTest()
        {
            _measureCounterMetricsMock = new Mock<IMeasureCounterMetrics>();
            _measureMetricsMock = new Mock<IMeasureMetrics>();
            _metricFactory = new MetricFactory(_measureMetricsMock.Object);
        }

        [TestMethod]
        public void TestCounterIncrement()
        {
            // Arrange
            _measureCounterMetricsMock.Setup(m => m.Increment(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>(), It.IsAny<long>()));
            _measureMetricsMock.Setup(m => m.Counter).Returns(_measureCounterMetricsMock.Object);

            ITestMetric1 testMetric1 = _metricFactory.CreateMetric<ITestMetric1>();

            // Act
            testMetric1.RequestCount("endpoint1", 200).Increment(5);

            // Assert
            _measureCounterMetricsMock.VerifyAll();

            Assert.AreEqual(1, _measureCounterMetricsMock.Invocations.Count);
            CounterOptions counterOptions = (CounterOptions) _measureCounterMetricsMock.Invocations[0].Arguments[0];
            Assert.AreEqual("RequestCount", counterOptions.Name);

            MetricTags metricTags = (MetricTags) _measureCounterMetricsMock.Invocations[0].Arguments[1];
            Assert.AreEqual(2, metricTags.Keys.Length);
            Assert.AreEqual("endpoint", metricTags.Keys[0]);
            Assert.AreEqual("httpStatus", metricTags.Keys[1]);

            Assert.AreEqual(2, metricTags.Values.Length);
            Assert.AreEqual("endpoint1", metricTags.Values[0]);
            Assert.AreEqual("200", metricTags.Values[1]);

            Assert.AreEqual(5L, _measureCounterMetricsMock.Invocations[0].Arguments[2]);
        }

        public interface ITestMetric1
        {
            ICounter RequestCount(string endpoint, int httpStatus);
        }
    }
}
