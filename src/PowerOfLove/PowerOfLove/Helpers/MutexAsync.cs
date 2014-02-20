using Jv.Games.Xna;
using System;
using System.Threading.Tasks;

namespace PowerOfLove.Helpers
{
    public class MutexAsync
    {
        SemaphoreAsync _semaphore;

        public MutexAsync()
        {
            _semaphore = new SemaphoreAsync(1, 1);
        }

        public async Task<IDisposable> WaitAsync()
        {
            await _semaphore.WaitOneAsync();
            return Disposable.Create(_semaphore.Release);
        }
    }
}
