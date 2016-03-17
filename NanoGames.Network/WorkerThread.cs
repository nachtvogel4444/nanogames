// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Threading;

namespace NanoGames.Network
{
    /// <summary>
    /// Helper class to create worker threads. Supports cancellation and waiting for successful cancellation in the Dispose method.
    /// </summary>
    internal sealed class WorkerThread : IDisposable
    {
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerThread"/> class.
        /// </summary>
        /// <param name="action">The action to invoke in the worker thread.</param>
        public WorkerThread(Action<CancellationToken> action)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _thread = new Thread(
                () =>
                {
                    try
                    {
                        action(_cancellationTokenSource.Token);
                    }
                    catch
                    {
                    }
                });
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Marks the thread for cancellation.
        /// </summary>
        public void Cancel()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Waits for the thread to finish.
        /// </summary>
        public void Join()
        {
            _thread.Join();
        }

        /// <summary>
        /// Marks the thread for cancellation, and waits until the thread has finished.
        /// </summary>
        public void Dispose()
        {
            Cancel();
            Join();
            _cancellationTokenSource.Dispose();
        }
    }
}
