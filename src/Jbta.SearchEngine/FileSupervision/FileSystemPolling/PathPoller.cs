using System;
using System.Linq;
using System.Timers;
using Jbta.SearchEngine.FileSupervision.FileSystemEventWatching;
using Timer = System.Timers.Timer;

namespace Jbta.SearchEngine.FileSupervision.FileSystemPolling
{
    internal class PathPoller : IDisposable
    {
        private const int PoolingIntervalMs = 1000;
        private readonly DeadPathDetector _deadPathCleaner;
        private readonly PathWatchersCollection _watchers;
        private readonly Timer _timer;
        private bool _isBusy;

        public PathPoller(
            DeadPathDetector deadPathCleaner,
            PathWatchersCollection watchers)
        {
            _deadPathCleaner = deadPathCleaner;
            _watchers = watchers;
            _timer = new Timer
            {
                Enabled = false,
                Interval = PoolingIntervalMs
            };
            _timer.Elapsed += Check;
        }

        public void TryStart()
        {
            if (_timer.Enabled)
            {
                return;
            }
            _watchers.ReadPathes(pathes =>
            {
                if (pathes.Any())
                {
                    _timer.Start();
                }
            });
        }

        public void TryStop()
        {
            if (!_timer.Enabled)
            {
                return;
            }
            _watchers.ReadPathes(pathes =>
            {
                if (!pathes.Any())
                {
                    _timer.Stop();
                }
            });
        }

        private void Check(object sender, ElapsedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }
            _isBusy = true;

            _deadPathCleaner.Clean();
            TryStop();

            _isBusy = false;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Elapsed -= Check;
            _timer.Dispose();
        }
    }
}