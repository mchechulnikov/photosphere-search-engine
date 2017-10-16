using System;
using System.Collections.Concurrent;
using System.Threading;
using Jbta.SearchEngine.Vendor.VsCodeFilewatcher;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching
{
    internal class FileSystemEventsProcessor : IDisposable
    {
        private readonly EventProcessor _internalProcessor;
        private readonly ConcurrentQueue<FileSystemEvent> _eventQueue;
        private readonly Thread _eventLoopThread;

        public FileSystemEventsProcessor(FileSystemEventHandler eventHandler)
        {
            _eventQueue = new ConcurrentQueue<FileSystemEvent>();
            _internalProcessor = new EventProcessor(eventHandler.OnEvent, delegate {});
            _eventLoopThread = new Thread(EventLoop) { IsBackground = true };
            _eventLoopThread.Start();
        }

        public void Add(FileSystemEvent e) => _eventQueue.Enqueue(e);

        public void Dispose() => _eventLoopThread.Abort();

        private void EventLoop()
        {
            while (true)
            {
                if (_eventQueue.TryDequeue(out var e))
                {
                    _internalProcessor.ProcessEvent(e);
                }
                Thread.Sleep(30);
            }
        }
    }
}