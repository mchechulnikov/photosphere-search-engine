using System.Timers;
using Photosphere.SearchEngine.FileIndexing;

namespace Photosphere.SearchEngine.Scheduling
{
    internal class Scheduler : IScheduler
    {
        private readonly ICleaner _cleaner;
        private readonly Timer _timer;

        public Scheduler(ICleaner cleaner, SearchEngineSettings settings)
        {
            _cleaner = cleaner;
            _timer = new Timer
            {
                Enabled = false,
                Interval = settings.CleaUpIntervalMs
            };
            _timer.Elapsed += OnElapsed;
        }

        public void Start() => _timer.Start();

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_cleaner.IsBusy)
            {
                _cleaner.CleanUp();
            }
        }

        public void Dispose()
        {
            _timer.Elapsed -= OnElapsed;
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}