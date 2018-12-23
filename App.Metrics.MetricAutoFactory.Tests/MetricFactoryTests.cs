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
            _measureCounterMetricsMock
                .Setup(m => m.Increment(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>(), It.IsAny<long>()));
            _measureMetricsMock.Setup(m => m.Counter).Returns(_measureCounterMetricsMock.Object);

            ITestMetric1 testMetric1 = _metricFactory.CreateMetric<ITestMetric1>();

            // Act
            testMetric1.RequestCount("endoint1", 200).Increment(5);

            // Assert
            _measureCounterMetricsMock.VerifyAll();
        }

        public interface ITestMetric1
        {
            ICounter RequestCount(string endpoint, int httpStatus);
        }
    }
}
