using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Utilities
{
    public static class AsyncReaderWriterLockSlimExtensions
    {
        public static async Task<IDisposable> EnterReadLockAndExitAsync(this AsyncReaderWriterLockSlim readerWriterLock, CancellationToken cancellationToken = default)
        {
            await readerWriterLock.EnterReadLockAsync(cancellationToken).WithoutCapturingContext();
            return new AsyncReaderWriterLockSlimExiter(readerWriterLock);
        }

        public static async Task<IDisposable> EnterWriteLockAndExitAsync(this AsyncReaderWriterLockSlim readerWriterLock, CancellationToken cancellationToken = default)
        {
            await readerWriterLock.EnterWriteLockAsync(cancellationToken).WithoutCapturingContext();
            return new AsyncReaderWriterLockSlimExiter(readerWriterLock, isWriteMode: true);
        }

        private struct AsyncReaderWriterLockSlimExiter : IDisposable
        {
            private AsyncReaderWriterLockSlim? readerWriterLock;
            private readonly bool isWriteMode;

            public AsyncReaderWriterLockSlimExiter(AsyncReaderWriterLockSlim readerWriterLock, bool isWriteMode = false)
            {
                this.readerWriterLock = readerWriterLock;
                this.isWriteMode = isWriteMode;
            }

            public void Dispose()
            {
                if (readerWriterLock is not null)
                {
                    if (isWriteMode)
                    {
                        readerWriterLock.ExitWriteLock();
                    }
                    else
                    {
                        readerWriterLock.ExitReadLock();
                    }
                    readerWriterLock = null;
                }
            }
        }
    }
}
