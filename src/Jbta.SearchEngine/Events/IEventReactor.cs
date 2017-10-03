namespace Jbta.SearchEngine.Events
{
    /// <summary>
    /// Event mediator between engine object and internal components
    /// </summary>
    internal interface IEventReactor
    {
        void React(EngineEvent e, params object[] args);
    }
}