using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception indicating that step or scenario thrown an exception.
    /// It's purpose is to allow LightBDD engine to process exception and eventually report them back to the underlying test frameworks without exposing LightBDD internal stack frames.
    /// 
    /// The inner exception represents original one that has been thrown by step/scenario.
    /// </summary>
    public class ScenarioExecutionException : Exception
    {
        public ScenarioExecutionException(Exception inner) : base(string.Empty, inner) { }

        public ExceptionDispatchInfo GetOriginal()
        {
            return ExceptionDispatchInfo.Capture(InnerException);
        }
    }

    [DebuggerStepThrough]
    public class ScenarioExecutionFlow
    {
        public static ScenarioExceptionWrappingAwaitable WrapScenarioExceptions(Task targetTask)
        {
            return new ScenarioExceptionWrappingAwaitable(targetTask);
        }

        public static ScenarioExceptionWrappingAwaitable<T> WrapScenarioExceptions<T>(Task<T> targetTask)
        {
            return new ScenarioExceptionWrappingAwaitable<T>(targetTask);
        }
    }
    [DebuggerStepThrough]
    public struct ScenarioExceptionWrappingAwaitable : ICriticalNotifyCompletion
    {
        private Task _task;
        private TaskAwaiter _awaiter;

        internal ScenarioExceptionWrappingAwaitable(Task task)
        {
            _task = task;
            _awaiter = _task.GetAwaiter();
        }

        public ScenarioExceptionWrappingAwaitable GetAwaiter() => this;

        public bool IsCompleted => _awaiter.IsCompleted;
        [SecuritySafeCritical]
        public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);
        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

        public void GetResult()
        {
            if (!_task.IsCompleted)
                _task.Wait();
            if (_task.Exception != null)
                throw new ScenarioExecutionException(_task.Exception.InnerExceptions[0]);
        }
    }

    [DebuggerStepThrough]
    public struct ScenarioExceptionWrappingAwaitable<T> : ICriticalNotifyCompletion
    {
        private Task<T> _task;
        private TaskAwaiter<T> _awaiter;

        internal ScenarioExceptionWrappingAwaitable(Task<T> task)
        {
            _task = task;
            _awaiter = _task.GetAwaiter();
        }

        public ScenarioExceptionWrappingAwaitable<T> GetAwaiter() => this;

        public bool IsCompleted => _awaiter.IsCompleted;
        [SecuritySafeCritical]
        public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);
        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

        public T GetResult()
        {
            if (!_task.IsCompleted)
                _task.Wait();

            if (_task.Exception != null)
                throw new ScenarioExecutionException(_task.Exception.InnerExceptions[0]);

            return _awaiter.GetResult();
        }
    }
}