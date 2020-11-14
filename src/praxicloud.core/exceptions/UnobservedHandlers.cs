// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.exceptions
{
    #region Using Clauses
    using System;
    using System.Threading.Tasks;
    using security;
    #endregion

    /// <summary>
    /// A simple method that handles unobserved task or app domain originating exceptions
    /// </summary>
    public class UnobservedHandlers
    {
        #region Delegates
        /// <summary>
        /// The signature for an unbserved or unhandled exception handler
        /// </summary>
        /// <param name="sender">The source of the exception handler</param>
        /// <param name="exception">The exception object that was raised</param>
        /// <param name="isTerminating">True if the app domain is terminating</param>
        /// <param name="sourceType">The source of the invocation (app domain or task scheduler)</param>
        /// <returns>True if the exception should be concerned handled</returns>
        public delegate bool HandleException(object sender, Exception exception, bool isTerminating, UnobservedType sourceType);
        #endregion
        #region Variables
        /// <summary>
        /// A handler that is invoked when an app domain or task scheduler exception is caught
        /// </summary>
        private readonly HandleException _handler;
        #endregion
        #region Constructors
        /// <summary>
        /// A type that is used to handle unobserved app domain or task scheduler exceptions
        /// </summary>
        /// <param name="handler">A handler that is invoked when an app domain or task scheduler exception is caught</param>
        public UnobservedHandlers(HandleException handler)
        {
            Guard.NotNull(nameof(handler), handler);

            _handler = handler;

            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }
        #endregion
        #region Internal Handlers
        /// <summary>
        /// Triggers the domain unhandled exceptions
        /// </summary>
        /// <param name="sender">The caller of the instance</param>
        /// <param name="isTerminating">True if the the application is terminating</param>
        public void TestDomainUnhandled(object sender, bool isTerminating)
        {
            CurrentDomainUnhandledException(sender, new UnhandledExceptionEventArgs(new ApplicationException("Artificial triggered"), isTerminating));
        }

        /// <summary>
        /// Triggers the Task unhandled exceptions
        /// </summary>
        /// <param name="sender">The caller of the instance</param>
        public void TestTaskUnhandled(object sender)
        {
            UnobservedTaskExceptionHandler(sender, new UnobservedTaskExceptionEventArgs(new AggregateException(new ApplicationException("Artificial triggered"))));
        }

        /// <summary>
        /// Handles the app domain unhandled exceptions
        /// </summary>
        /// <param name="sender">The object that invoked the handler</param>
        /// <param name="eventArgs">The parameters associated with the unhandled exceptions</param>
        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            _handler(sender, eventArgs.ExceptionObject as Exception, eventArgs.IsTerminating, UnobservedType.AppDomain);
        }

        /// <summary>
        /// Handles the task schduler unobserved exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs eventArgs)
        {
            if (_handler(sender, eventArgs.Exception, false, UnobservedType.TaskScheduler))
            {
                eventArgs.SetObserved();
            }
        }
        #endregion
    }
}