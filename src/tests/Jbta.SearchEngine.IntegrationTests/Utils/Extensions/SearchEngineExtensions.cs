using System;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.Events.Args;

namespace Jbta.SearchEngine.IntegrationTests.Utils.Extensions
{
    internal static class SearchEngineExtensions
    {
        public static void AddHandler(this ISearchEngine engine, string eventName, Action<SearchEngineEventArgs> action)
        {
            var eventInfo = engine.GetType().GetEvent(eventName);
            eventInfo.AddEventHandler(engine, (SearchEngineEventHandler)(args =>
            {
                action(args);
            }));
        }
    }
}