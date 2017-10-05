using System;
using System.Collections.Generic;
using Jbta.SearchEngine.Events.Args;

namespace Jbta.SearchEngine.Events
{
    /// <summary>
    /// Event mediator between engine object and internal components
    /// </summary>
    internal class EventReactor : IEventReactor
    {
        private readonly IDictionary<EngineEvent, SearchEngineEventHandler> _eventHandlers;

        public EventReactor()
        {
            _eventHandlers = new Dictionary<EngineEvent, SearchEngineEventHandler>();
        }

        public void Register(EngineEvent e, SearchEngineEventHandler handler)
        {
            _eventHandlers.Add(e, handler);
        }

        public void React(EngineEvent e, params object[] args)
        {
            if (!_eventHandlers.TryGetValue(e, out var handler))
            {
                return;
            }

            var eventArgs = CastParams(e, args);
            handler.Invoke(eventArgs);
        }

        private SearchEngineEventArgs CastParams(EngineEvent e, object[] args)
        {
            switch (e)
            {
                case EngineEvent.FileIndexingStarted:
                case EngineEvent.FileRemovingStarted:
                case EngineEvent.FileUpdateInitiated:
                    if (args.Length != 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(args), args, "Invalid args count");
                    }
                    return new SearchEngineEventArgs((string)args[0]);

                case EngineEvent.FileIndexingEnded:
                case EngineEvent.FileRemovingEnded:
                    if (args.Length < 1 && args.Length > 2)
                    {
                        throw new ArgumentOutOfRangeException(nameof(args), args, "Invalid args count");
                    }
                    if (args.Length == 1)
                    {
                        return new SearchEngineEventArgs((string)args[0]);
                    }
                    return new SearchEngineEventArgs((string)args[0], (Exception)args[1]);

                case EngineEvent.FileUpdateFailed:
                    if (args.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException(nameof(args), args, "Invalid args count");
                    }
                    return new SearchEngineEventArgs((string)args[0], (Exception)args[1]);

                case EngineEvent.FilePathChanged:
                    if (args.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException(nameof(args), args, "Invalid args count");
                    }
                    return new FilePathChangedEventArgs((string)args[0], (string)args[1]);

                case EngineEvent.IndexCleanUpFailed:
                    if (args.Length != 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(args), args, "Invalid args count");
                    }
                    return new SearchEngineEventArgs((Exception)args[0]);

                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, "Invalid event");
            }
        }
    }
}