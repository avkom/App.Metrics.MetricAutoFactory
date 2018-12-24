using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using App.Metrics.Counter;

namespace App.Metrics.MetricAutoFactory.Tests
{
    [TestClass]
    public class MetricFactoryTests
    {
        private Mock<ICounter> _counterMock;
        private Mock<IProvideCounterMetrics> _provideCounterMetricsMock;
        private Mock<IProvideMetrics> _metricsProviderMock;
        private Mock<IMetrics> _metricsMock;
        private IMetricFactory _metricFactory;

        [TestInitialize]
        public void InitializeTest()
        {
            _counterMock = new Mock<ICounter>();
            _provideCounterMetricsMock = new Mock<IProvideCounterMetrics>();
            _metricsProviderMock = new Mock<IProvideMetrics>();
            _metricsMock = new Mock<IMetrics>();
        }

        [TestMethod]
        public void TestCounterIncrement()
        {
            // Arrange
            _counterMock
                .Setup(m => m.Increment(It.IsAny<long>()));

            _provideCounterMetricsMock
                .Setup(m => m.Instance(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>()))
                .Returns(_counterMock.Object);

            _metricsProviderMock
                .SetupGet(m => m.Counter)
                .Returns(_provideCounterMetricsMock.Object);

            _metricsMock
                .SetupGet(m => m.Provider)
                .Returns(_metricsProviderMock.Object);

            _metricFactory = new MetricFactory(_metricsMock.Object);

            ITestMetric1 testMetric1 = _metricFactory.CreateMetric<ITestMetric1>();

            // Act
            testMetric1.RequestCount("endpoint1", 200).Increment(5);

            // Assert
            _counterMock.VerifyAll();
            _provideCounterMetricsMock.VerifyAll();
            _metricsProviderMock.VerifyAll();
            _metricsMock.VerifyAll();

            Assert.AreEqual(1, _provideCounterMetricsMock.Invocations.Count);
            CounterOptions counterOptions = (CounterOptions) _provideCounterMetricsMock.Invocations[0].Arguments[0];
            Assert.AreEqual("RequestCount", counterOptions.Name);

            MetricTags metricTags = (MetricTags) _provideCounterMetricsMock.Invocations[0].Arguments[1];
            Assert.AreEqual(2, metricTags.Keys.Length);
            Assert.AreEqual("endpoint", metricTags.Keys[0]);
            Assert.AreEqual("httpStatus", metricTags.Keys[1]);

            Assert.AreEqual(2, metricTags.Values.Length);
            Assert.AreEqual("endpoint1", metricTags.Values[0]);
            Assert.AreEqual("200", metricTags.Values[1]);

            Assert.AreEqual(5L, _counterMock.Invocations[0].Arguments[0]);
        }

        public interface ITestMetric1
        {
            ICounter RequestCount(string endpoint, int httpStatus);
        }
    }
}
