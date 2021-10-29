using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SignatureTestApp
{
    class ThreadQueue : IDisposable
     {
        private readonly object _locker = new();
        private readonly int _maxThreads;
        private readonly Queue<Action> _tasks = new();
        private readonly ManualResetEvent _taskDequedResetEvent = new(true);
        private readonly List<Thread> _threads;
        private int _waitingThreads;

        public ThreadQueue(int maxThreads)
        {
            _maxThreads = maxThreads;
            _threads = new List<Thread>(_maxThreads);
        }

        public void Enqueue(Action task)
        {
            _taskDequedResetEvent.WaitOne();

            lock (_locker)
            {
                //Limit queue size
                if (_tasks.Count >= _maxThreads * 2 - 1)
                {
                    _taskDequedResetEvent.Reset();
                }

                //Spawn new thread if maximum count not reached
                if (_waitingThreads == 0 && _threads.Count() < _maxThreads)
                {
                    CreateThread();
                }

                _tasks.Enqueue(task);
                Monitor.PulseAll(_locker);
            }
        }
        
        private void CreateThread()
        {
            Thread thread = new(Worker);
            _threads.Add(thread);
            thread.Start();
        }

        private void Worker()
        {
            while (true)
            {
                Action task;

                lock (_locker)
                {
                    //wait for task in queue
                    while (_tasks.Count == 0)
                    {
                        _waitingThreads++;
                        Monitor.Wait(_locker);
                        _waitingThreads--;
                    }

                    task = _tasks.Dequeue();
                    _taskDequedResetEvent.Set();
                    
                    //Finish task if null dequeued
                    if(task == null)
                    {
                        return;
                    }
                }

                task();
            }
        }

        public void Dispose()
        {
            //Finish all tasks
            for (var i = 0; i < _threads.Count(); i++)
            {
                Enqueue(null);
            }

            //Wait unfinished threads
            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }
    }
}
