using HR.BrightspaceConnector.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class AsyncReaderWriterLockSlimTests
    {
        [TestMethod]
        public void EnterReadLockAsync_WhenUnlocked_Locks()
        {
            var readerWriterLock = new AsyncReaderWriterLockSlim();

            var enterReadLockTask = readerWriterLock.EnterReadLockAsync();

            Assert.IsTrue(enterReadLockTask.IsCompleted);
            Assert.IsFalse(enterReadLockTask.IsFaulted);
            Assert.IsFalse(enterReadLockTask.IsCanceled);
        }

        [TestMethod]
        public async Task EnterReadLockAsync_WhenUnlocked_UpdatesCurrentReadCount()
        {
            var readerWriterLock = new AsyncReaderWriterLockSlim();

            var readerCountBeforeEnter = readerWriterLock.CurrentReadCount;
            await readerWriterLock.EnterReadLockAsync();
            var readerCountAfterEnter = readerWriterLock.CurrentReadCount;

            Assert.AreEqual(0, readerCountBeforeEnter);
            Assert.AreEqual(1, readerCountAfterEnter);
        }

        [TestMethod]
        public async Task EnterReadLockAsync_WhenReadLocked_AllowsLock()
        {
            var readerWriterLock = new AsyncReaderWriterLockSlim();

            var readerCountBeforeEnter = readerWriterLock.CurrentReadCount;
            await readerWriterLock.EnterReadLockAsync();
            var readerCountAfterEnter1 = readerWriterLock.CurrentReadCount;
            await readerWriterLock.EnterReadLockAsync();
            var readerCountAfterEnter2 = readerWriterLock.CurrentReadCount;

            Assert.AreEqual(0, readerCountBeforeEnter);
            Assert.AreEqual(1, readerCountAfterEnter1);
            Assert.AreEqual(2, readerCountAfterEnter2);
        }

        [TestMethod]
        public async Task ExitReadLock_WhenReadLocked_Unlocks()
        {
            var readerWriterLock = new AsyncReaderWriterLockSlim();
            await readerWriterLock.EnterReadLockAsync();

            var readerCountBeforeExit = readerWriterLock.CurrentReadCount;
            readerWriterLock.ExitReadLock();
            var readerCountAfterExit = readerWriterLock.CurrentReadCount;

            Assert.AreEqual(1, readerCountBeforeExit);
            Assert.AreEqual(0, readerCountAfterExit);
        }

        [TestMethod]
        public async Task EnterWriteLockAsync_ByDefault_DoesNotUpdateCurrentReadCount()
        {
            var readerWriterLock = new AsyncReaderWriterLockSlim();

            var readerCountBeforeEnter = readerWriterLock.CurrentReadCount;
            await readerWriterLock.EnterWriteLockAsync();
            var readerCountAfterEnter = readerWriterLock.CurrentReadCount;

            Assert.AreEqual(0, readerCountBeforeEnter);
            Assert.AreEqual(0, readerCountAfterEnter);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task EnterReadLockAsync_WhenWriteLocked_PreventsLock()
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var readerWriterLock = new AsyncReaderWriterLockSlim();

            await readerWriterLock.EnterWriteLockAsync();
            await readerWriterLock.EnterReadLockAsync(cancellationTokenSource.Token);
        }
    }
}
