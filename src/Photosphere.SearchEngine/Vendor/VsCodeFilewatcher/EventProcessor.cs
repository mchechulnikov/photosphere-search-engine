using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photosphere.SearchEngine.Vendor.VsCodeFilewatcher
{
    internal class EventProcessor
    {
        private const int EventDelay = 50; // aggregate and only emit events when changes have stopped for this duration (in ms)
        private const int EventSpamWarningThreshold = 60 * 1000 * 10000; // warn after certain time span of event spam (in ticks)

        private readonly object _lock = new object();
        private Task _delayTask;

        private readonly List<FileSystemEvent> _events = new List<FileSystemEvent>();
        private readonly Action<FileSystemEvent> _handleEvent;

        private readonly Action<string> _logger;

        private long _lastEventTime;
        private long _delayStarted;

        private long _spamCheckStartTime;
        private bool _spamWarningLogged;

        public EventProcessor(Action<FileSystemEvent> onEvent, Action<string> onLogging)
        {
            _handleEvent = onEvent;
            _logger = onLogging;
        }

        public void ProcessEvent(FileSystemEvent fileEvent)
        {
            lock (_lock)
            {
                var now = DateTime.Now.Ticks;

                // Check for spam
                if (_events.Count == 0)
                {
                    _spamWarningLogged = false;
                    _spamCheckStartTime = now;
                }
                else if (!_spamWarningLogged && _spamCheckStartTime + EventSpamWarningThreshold < now)
                {
                    _spamWarningLogged = true;
                    _logger(string.Format("Warning: Watcher is busy catching up wit {0} file changes in 60 seconds. Latest path is '{1}'", _events.Count, fileEvent.Path));
                }

                // Add into our queue
                _events.Add(fileEvent);
                _lastEventTime = now;

                // Process queue after delay
                if (_delayTask == null)
                {
                    // Create function to buffer events
                    Action<Task> func = null;
                    func = (Task value) => {
                        lock (_lock)
                        {
                            // Check if another event has been received in the meantime
                            if (_delayStarted == _lastEventTime)
                            {
                                // Normalize and handle
                                var normalized = NormalizeEvents(_events.ToArray());
                                foreach (var e in normalized)
                                {
                                    _handleEvent(e);
                                }

                                // Reset
                                _events.Clear();
                                _delayTask = null;
                            }

                            // Otherwise we have received a new event while this task was
                            // delayed and we reschedule it.
                            else
                            {
                                _delayStarted = _lastEventTime;
                                _delayTask = Task.Delay(EventDelay).ContinueWith(func);
                            }
                        }
                    };

                    // Start function after delay
                    _delayStarted = _lastEventTime;
                    _delayTask = Task.Delay(EventDelay).ContinueWith(func);
                }
            }
        }

        private static IEnumerable<FileSystemEvent> NormalizeEvents(FileSystemEvent[] events)
        {
            var mapPathToEvents = new Dictionary<string, FileSystemEvent>();
            var eventsWithoutDuplicates = new List<FileSystemEvent>();

            // Normalize Duplicates
            foreach (var e in events)
            {

                // Existing event
                if (mapPathToEvents.ContainsKey(e.Path))
                {
                    var existingEvent = mapPathToEvents[e.Path];
                    var currentChangeType = existingEvent.ChangeType;
                    var newChangeType = e.ChangeType;

                    // ignore CREATE followed by DELETE in one go
                    if (currentChangeType == ChangeType.Created && newChangeType == ChangeType.Deleted)
                    {
                        mapPathToEvents.Remove(existingEvent.Path);
                        eventsWithoutDuplicates.Remove(existingEvent);
                    }

                    // flatten DELETE followed by CREATE into CHANGE
                    else if (currentChangeType == ChangeType.Deleted && newChangeType == ChangeType.Created)
                    {
                        existingEvent.ChangeType = ChangeType.Changed;
                    }

                    // Do nothing. Keep the created event
                    else if (currentChangeType == ChangeType.Created && newChangeType == ChangeType.Changed)
                    {
                    }

                    // Otherwise apply change type
                    else
                    {
                        existingEvent.ChangeType = newChangeType;
                    }
                }

                // New event
                else
                {
                    mapPathToEvents.Add(e.Path, e);
                    eventsWithoutDuplicates.Add(e);
                }
            }

            // Handle deletes
            var addedChangeEvents = new List<FileSystemEvent>();
            var deletedPaths = new List<string>();

            // This algorithm will remove all DELETE events up to the root folder
            // that got deleted if any. This ensures that we are not producing
            // DELETE events for each file inside a folder that gets deleted.
            //
            // 1.) split ADD/CHANGE and DELETED events
            // 2.) sort short deleted paths to the top
            // 3.) for each DELETE, check if there is a deleted parent and ignore the event in that case

            return eventsWithoutDuplicates
                .Where((e) =>
                {
                    if (e.ChangeType != ChangeType.Deleted)
                    {
                        addedChangeEvents.Add(e);
                        return false; // remove ADD / CHANGE
                    }

                    return true;
                })
                .OrderBy((e) => e.Path.Length) // shortest path first
                .Where((e) =>
                {
                    if (deletedPaths.Any(d => IsParent(e.Path, d)))
                    {
                        return false; // DELETE is ignored if parent is deleted already
                    }

                    // otherwise mark as deleted
                    deletedPaths.Add(e.Path);

                    return true;
                })
                .Concat(addedChangeEvents);
        }

        private static bool IsParent(string p, string candidate)
        {
            return p.IndexOf(candidate + '\\') == 0;
        }
    }
}