using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using App.Metrics.Counter;
using App.Metrics.Gauge;

namespace App.Metrics.MetricAutoFactory.Tests
{
    [TestClass]
    public class MetricFactoryTests
    {
        private Mock<IProvideMetrics> _metricsProviderMock;
        private Mock<IMetrics> _metricsMock;

        [TestInitialize]
        public void InitializeTest()
        {
            _metricsProviderMock = new Mock<IProvideMetrics>();
            _metricsMock = new Mock<IMetrics>();
        }

        [TestMethod]
        public void TestCounterIncrement()
        {
            // Arrange
            var counterMock = new Mock<ICounter>();
            counterMock
                .Setup(m => m.Increment(It.IsAny<long>()));

            var provideCounterMetricsMock = new Mock<IProvideCounterMetrics>();
            provideCounterMetricsMock
                .Setup(m => m.Instance(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>()))
                .Returns(counterMock.Object);

            _metricsProviderMock
                .SetupGet(m => m.Counter)
                .Returns(provideCounterMetricsMock.Object);

            _metricsMock
                .SetupGet(m => m.Provider)
                .Returns(_metricsProviderMock.Object);

            var metricFactory = new MetricFactory(_metricsMock.Object);

            ITestMetric1 testMetric1 = metricFactory.CreateMetric<ITestMetric1>();

            // Act
            testMetric1.RequestCount("endpoint1", 200).Increment(5);

            // Assert
            counterMock.VerifyAll();
            provideCounterMetricsMock.VerifyAll();
            _metricsProviderMock.VerifyAll();
            _metricsMock.VerifyAll();

            Assert.AreEqual(1, provideCounterMetricsMock.Invocations.Count);
            CounterOptions counterOptions = (CounterOptions) provideCounterMetricsMock.Invocations[0].Arguments[0];
            Assert.AreEqual("RequestCount", counterOptions.Name);

            MetricTags metricTags = (MetricTags) provideCounterMetricsMock.Invocations[0].Arguments[1];
            Assert.AreEqual(2, metricTags.Keys.Length);
            Assert.AreEqual("endpoint", metricTags.Keys[0]);
            Assert.AreEqual("httpStatus", metricTags.Keys[1]);

            Assert.AreEqual(2, metricTags.Values.Length);
            Assert.AreEqual("endpoint1", metricTags.Values[0]);
            Assert.AreEqual("200", metricTags.Values[1]);

            Assert.AreEqual(5L, counterMock.Invocations[0].Arguments[0]);
        }

        [TestMethod]
        public void TestGaugeSetValue()
        {
            // Arrange
            var gaugeMock = new Mock<IGauge>();
            gaugeMock
                .Setup(m => m.SetValue(It.IsAny<double>()));

            var provideGaugeMetricsMock = new Mock<IProvideGaugeMetrics>();
            provideGaugeMetricsMock
                .Setup(m => m.Instance(It.IsAny<GaugeOptions>(), It.IsAny<MetricTags>()))
                .Returns(gaugeMock.Object);

            _metricsProviderMock
                .SetupGet(m => m.Gauge)
                .Returns(provideGaugeMetricsMock.Object);

            _metricsMock
                .SetupGet(m => m.Provider)
                .Returns(_metricsProviderMock.Object);

            var metricFactory = new MetricFactory(_metricsMock.Object);

            ITestMetric1 testMetric1 = metricFactory.CreateMetric<ITestMetric1>();

            // Act
            testMetric1.QueueLength("queue1").SetValue(5.0);

            // Assert
            gaugeMock.VerifyAll();
            provideGaugeMetricsMock.VerifyAll();
            _metricsProviderMock.VerifyAll();
            _metricsMock.VerifyAll();

            Assert.AreEqual(1, provideGaugeMetricsMock.Invocations.Count);
            GaugeOptions gaugeOptions = (GaugeOptions) provideGaugeMetricsMock.Invocations[0].Arguments[0];
            Assert.AreEqual("QueueLength", gaugeOptions.Name);

            MetricTags metricTags = (MetricTags)provideGaugeMetricsMock.Invocations[0].Arguments[1];
            Assert.AreEqual(1, metricTags.Keys.Length);
            Assert.AreEqual("queue", metricTags.Keys[0]);

            Assert.AreEqual(1, metricTags.Values.Length);
            Assert.AreEqual("queue1", metricTags.Values[0]);

            Assert.AreEqual(5.0, gaugeMock.Invocations[0].Arguments[0]);
        }

        public interface ITestMetric1
        {
            ICounter RequestCount(string endpoint, int httpStatus);
            IGauge QueueLength(string queue);
        }
    }
}
