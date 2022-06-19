using HR.BrightspaceConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class RecoverySettingsTests
    {
        [TestMethod]
        public void NewInstance_HasExpectedDefaultValues()
        {
            RecoverySettings recoverySettings = new();

            Assert.AreEqual(4, recoverySettings.RetryAttempts);
            Assert.AreEqual(new TimeSpan(hours: 0, minutes: 0, seconds: 15), recoverySettings.RetryDelay);
            Assert.AreEqual(1.0, recoverySettings.BackOffRate);
        }

        [TestMethod]
        public void CalculateRetryDelay_GivenAttemptZero_ReturnsDelayOfZero()
        {
            var recoverySettings = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffRate = 1.2
            };

            var delay0 = recoverySettings.CalculateRetryDelay(0);

            Assert.AreEqual(TimeSpan.Zero, delay0);
        }

        [TestMethod]
        public void CalculateRetryDelay_GivenAttemptGreaterThanMaxAttempt_ReturnsMaxDelay()
        {
            var recoverySettings = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffRate = 1.2
            };

            var delay4 = recoverySettings.CalculateRetryDelay(4);
            var delay5 = recoverySettings.CalculateRetryDelay(5); // Max attempt.
            var delay6 = recoverySettings.CalculateRetryDelay(6);

            Assert.IsTrue(delay4 < delay5);
            Assert.AreEqual(delay5, delay6);
        }

        [TestMethod]
        public void CalculateRetryDelay_WithBackOffRateOfOne_ReturnsSameDelay()
        {
            var recoverySettings = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffRate = 1.0
            };

            var delay1 = recoverySettings.CalculateRetryDelay(1);
            var delay2 = recoverySettings.CalculateRetryDelay(2);
            var delay3 = recoverySettings.CalculateRetryDelay(3);
            var delay4 = recoverySettings.CalculateRetryDelay(4);
            var delay5 = recoverySettings.CalculateRetryDelay(5);

            Assert.AreEqual(delay1, delay2);
            Assert.AreEqual(delay2, delay3);
            Assert.AreEqual(delay3, delay4);
            Assert.AreEqual(delay4, delay5);
        }

        [TestMethod]
        public void CalculateRetryDelay_WithBackOffRateOfGreaterThanOne_ReturnsGraduallyLongerDelays()
        {
            var recoverySettings = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffRate = 1.2
            };

            var delay1 = recoverySettings.CalculateRetryDelay(1);
            var delay2 = recoverySettings.CalculateRetryDelay(2);
            var delay3 = recoverySettings.CalculateRetryDelay(3);
            var delay4 = recoverySettings.CalculateRetryDelay(4);
            var delay5 = recoverySettings.CalculateRetryDelay(5);

            Assert.IsTrue(delay1 < delay2);
            Assert.IsTrue(delay2 < delay3);
            Assert.IsTrue(delay3 < delay4);
            Assert.IsTrue(delay4 < delay5);
        }
    }
}
