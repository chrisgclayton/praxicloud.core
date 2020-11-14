// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// Utilities to monitor and event on Container life cycle events.
    /// </summary>
    public static class ContainerLifecycle
    {
        #region Variables
        /// <summary>
        /// The cancellation token source used to trigger cancellation of the monitoring token
        /// </summary>
        private static readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();

        /// <summary>
        /// A task that completes when the cancellation occurs
        /// </summary>
        private static readonly TaskCompletionSource<bool> _taskCompletion = new TaskCompletionSource<bool>();
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes the Type
        /// </summary>
        static ContainerLifecycle()
        {
            AssemblyLoadContext.Default.Unloading += DefaultUnloading;
            Console.CancelKeyPress += CancelKeyPress;
        }
        #endregion
        #region Properties
        /// <summary>
        /// A Cancellation Token that triggers when the container is shutdown
        /// </summary>
        public static CancellationToken CancellationToken => _cancellationSource.Token;

        /// <summary>
        /// A task that completes when the container is shutdown
        /// </summary>
        public static Task Task => _taskCompletion.Task;
        #endregion
        #region Methods
        /// <summary>
        /// A method that can be called to artificially trigger a shutdown
        /// </summary>
        public static void End()
        {
            DefaultUnloading(null);
        }
        #endregion
        #region Handlers
        /// <summary>
        /// A handler that is triggered when the entry assembly is unloaded
        /// </summary>
        /// <param name="context">The assembly context</param>
        private static void DefaultUnloading(AssemblyLoadContext context)
        {
            if (!_cancellationSource.IsCancellationRequested) _cancellationSource.Cancel();
            _taskCompletion.TrySetResult(true);
        }

        /// <summary>
        /// A handler that is triggered when the container receives a cancel key press
        /// </summary>
        /// <param name="sender">The object that the invocation is associated with</param>
        /// <param name="arguments">The arguments associated with the event</param>
        [ExcludeFromCodeCoverage]
        private static void CancelKeyPress(object sender, ConsoleCancelEventArgs arguments)
        {
            arguments.Cancel = true;

            _cancellationSource.Cancel();
            _taskCompletion.TrySetResult(true);
        }
        #endregion
    }
}
