using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerOfLove.Helpers
{
    public class SemaphoreAsync
    {
        uint _count;
        uint _maximumCount;

        Queue<TaskCompletionSource<bool>> _waiting;

        public SemaphoreAsync(uint initialCount, uint maximumCount)
        {
            if (initialCount > maximumCount)
                throw new ArgumentException();

            if (maximumCount < 1)
                throw new ArgumentOutOfRangeException();

            _count = initialCount;
            _maximumCount = maximumCount;
            _waiting = new Queue<TaskCompletionSource<bool>>();
        }

        public Task WaitOneAsync()
        {
            lock (this)
            {
                var tcs = new TaskCompletionSource<bool>();
                if (_count > 0)
                {
                    _count--;
                    tcs.SetResult(true);
                }
                else
                    _waiting.Enqueue(tcs);

                return tcs.Task;
            }
        }

        public void Release()
        {
            Release(1);
        }

        public void Release(uint releaseCount)
        {
            List<TaskCompletionSource<bool>> toRelase = new List<TaskCompletionSource<bool>>();

            lock (this)
            {
                if (_count + releaseCount - _waiting.Count > _maximumCount)
                    throw new InvalidOperationException();

                while (releaseCount > 0 && _waiting.Count > 0)
                {
                    toRelase.Add(_waiting.Dequeue());
                    releaseCount--;
                }
                _count += releaseCount;
            }

            foreach (var tcs in toRelase)
                tcs.SetResult(true);
        }
    }
}
