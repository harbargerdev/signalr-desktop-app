using FluentAssertions;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalR.Event.Handler.Core.Tests.Utilities
{
    [TestFixture]
    public class RetryPolicyTests
    {
        private RetryPolicy _retryPolicy;

        [SetUp]
        public void SetUp()
        {
            _retryPolicy = new RetryPolicy
            {
                MaxRetries = 3,
                InitialDelayMs = 100,
                BackoffMultiplier = 2.0,
                MaxDelayMs = 1000
            };
        }

        [Test]
        public void Execute_SuccessOnFirstAttempt_ReturnsResult()
        {
            // Arrange
            var expectedResult = "Success";
            Func<string> action = () => expectedResult;

            // Act
            var result = _retryPolicy.Execute(action);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Test]
        public void Execute_SuccessOnSecondAttempt_ReturnsResult()
        {
            // Arrange
            var attemptCount = 0;
            Func<string> action = () =>
            {
                attemptCount++;
                if (attemptCount == 1)
                    throw new InvalidOperationException("First attempt failed");
                return "Success";
            };

            // Act
            var result = _retryPolicy.Execute(action);

            // Assert
            result.Should().Be("Success");
            attemptCount.Should().Be(2);
        }

        [Test]
        public void Execute_AllAttemptsFail_ThrowsAggregateException()
        {
            // Arrange
            var attemptCount = 0;
            Func<string> action = () =>
            {
                attemptCount++;
                throw new InvalidOperationException($"Attempt {attemptCount} failed");
            };

            // Act
            Action act = () => _retryPolicy.Execute(action);

            // Assert
            act.Should().Throw<AggregateException>()
                .Which.InnerExceptions.Should().HaveCount(4); // Initial + 3 retries
            attemptCount.Should().Be(4);
        }

        [Test]
        public void Execute_AggregateExceptionMessage_ContainsRetryCount()
        {
            // Arrange
            Func<string> action = () => throw new Exception("Test failure");

            // Act
            Action act = () => _retryPolicy.Execute(action);

            // Assert
            act.Should().Throw<AggregateException>()
                .WithMessage("*3 retry attempts*");
        }

        [Test]
        public async Task ExecuteAsync_SuccessOnFirstAttempt_ReturnsResult()
        {
            // Arrange
            var expectedResult = "Success";
            Func<Task<string>> action = () => Task.FromResult(expectedResult);

            // Act
            var result = await _retryPolicy.ExecuteAsync(action);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Test]
        public async Task ExecuteAsync_SuccessOnThirdAttempt_ReturnsResult()
        {
            // Arrange
            var attemptCount = 0;
            Func<Task<string>> action = () =>
            {
                attemptCount++;
                if (attemptCount < 3)
                    throw new InvalidOperationException($"Attempt {attemptCount} failed");
                return Task.FromResult("Success");
            };

            // Act
            var result = await _retryPolicy.ExecuteAsync(action);

            // Assert
            result.Should().Be("Success");
            attemptCount.Should().Be(3);
        }

        [Test]
        public void ExecuteAsync_AllAttemptsFail_ThrowsAggregateException()
        {
            // Arrange
            var attemptCount = 0;
            Func<Task<string>> action = () =>
            {
                attemptCount++;
                throw new InvalidOperationException($"Attempt {attemptCount} failed");
            };

            // Act
            Func<Task> act = async () => await _retryPolicy.ExecuteAsync(action);

            // Assert
            act.Should().ThrowAsync<AggregateException>()
                .Result.Which.InnerExceptions.Should().HaveCount(4);
            attemptCount.Should().Be(4);
        }

        [Test]
        public void Execute_WithZeroRetries_FailsImmediately()
        {
            // Arrange
            _retryPolicy.MaxRetries = 0;
            var attemptCount = 0;
            Func<string> action = () =>
            {
                attemptCount++;
                throw new Exception("Failed");
            };

            // Act
            Action act = () => _retryPolicy.Execute(action);

            // Assert
            act.Should().Throw<AggregateException>();
            attemptCount.Should().Be(1); // Only initial attempt, no retries
        }

        [Test]
        public void Execute_ExponentialBackoff_CalculatesDelaysCorrectly()
        {
            // Arrange
            _retryPolicy.InitialDelayMs = 100;
            _retryPolicy.BackoffMultiplier = 2.0;
            _retryPolicy.MaxDelayMs = 10000;

            var attemptTimes = new List<DateTime>();
            Func<string> action = () =>
            {
                attemptTimes.Add(DateTime.UtcNow);
                throw new Exception("Failed");
            };

            // Act
            Action act = () => _retryPolicy.Execute(action);

            // Assert
            act.Should().Throw<AggregateException>();
            attemptTimes.Should().HaveCount(4);

            // Verify delays: 0ms, ~100ms, ~200ms, ~400ms
            if (attemptTimes.Count >= 2)
            {
                var firstDelay = (attemptTimes[1] - attemptTimes[0]).TotalMilliseconds;
                firstDelay.Should().BeGreaterOrEqualTo(90); // Allow 10ms tolerance
            }
        }

        [Test]
        public void Execute_MaxDelayRespected_DoesNotExceedLimit()
        {
            // Arrange
            _retryPolicy.InitialDelayMs = 500;
            _retryPolicy.BackoffMultiplier = 10.0;
            _retryPolicy.MaxDelayMs = 600;

            var attemptTimes = new List<DateTime>();
            Func<string> action = () =>
            {
                attemptTimes.Add(DateTime.UtcNow);
                throw new Exception("Failed");
            };

            // Act
            Action act = () => _retryPolicy.Execute(action);

            // Assert
            act.Should().Throw<AggregateException>();

            // Verify that delays don't exceed MaxDelayMs
            for (int i = 1; i < attemptTimes.Count; i++)
            {
                var delay = (attemptTimes[i] - attemptTimes[i - 1]).TotalMilliseconds;
                delay.Should().BeLessThan(_retryPolicy.MaxDelayMs + 50); // Allow small tolerance
            }
        }
    }
}
