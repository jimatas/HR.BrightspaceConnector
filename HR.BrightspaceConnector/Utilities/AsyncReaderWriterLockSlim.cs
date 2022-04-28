using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Utilities
{
    /// <summary>
    /// Implements a reader/writer lock through two semaphores, more specifically <see cref="SemaphoreSlim"/> objects, which coordinate exclusive write and concurrent read access to a resource.
    /// Does not support recursion or upgradeable read mode.
    /// </summary>
    /// <seealso href="https://stackoverflow.com/a/64757462">ReaderWriterLockSlim and async\await</seealso>
    public sealed class AsyncReaderWriterLockSlim : IDisposable
    {
        private readonly SemaphoreSlim readMutex = new(initialCount: 1, maxCount: 1);
        private readonly SemaphoreSlim writeMutex = new(initialCount: 1, maxCount: 1);
        private int readerCount;

        /// <summary>
        /// The current number of readers.
        /// </summary>
        public int CurrentReadCount => readerCount;

        /// <summary>
        /// Tries to asynchronously enter the lock in read mode.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task EnterReadLockAsync(CancellationToken cancellationToken = default)
        {
            using (await writeMutex.WaitAndReleaseAsync(cancellationToken).WithoutCapturingContext())
            {
                if (Interlocked.Increment(ref readerCount) == 1)
                {
                    try
                    {
                        await readMutex.WaitAsync(cancellationToken).WithoutCapturingContext();
                    }
                    catch
                    {
                        Interlocked.Decrement(ref readerCount);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Reduces the current reader count and exits read mode if the resulting count is zero.
        /// </summary>
        public void ExitReadLock()
        {
            if (Interlocked.Decrement(ref readerCount) <= 0)
            {
                try
                {
                    readMutex.Release();
                }
                catch (SemaphoreFullException) // readerCount < 0
                {
                    Interlocked.Exchange(ref readerCount, 0);
                    throw;
                }
            }
        }

        /// <summary>
        /// Tries to asynchronously enter the lock in write mode.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task EnterWriteLockAsync(CancellationToken cancellationToken = default)
        {
            await writeMutex.WaitAsync(cancellationToken).WithoutCapturingContext();
            try
            {
                await readMutex.WaitAsync(cancellationToken).WithoutCapturingContext();
            }
            catch
            {
                writeMutex.Release();
                throw;
            }
        }

        /// <summary>
        /// Exits the write mode.
        /// </summary>
        public void ExitWriteLock()
        {
            readMutex.Release();
            writeMutex.Release();
        }

        public void Dispose()
        {
            readMutex.Dispose();
            writeMutex.Dispose();
        }
    }
}
