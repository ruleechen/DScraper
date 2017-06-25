using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DScraper
{
    public class ProcessWatcher : IDisposable
    {
        public ProcessWatcher(Process process, TimeSpan timeout)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException("timeout");
            }

            Process = process;
            Timeout = timeout;
            Cancellation = new CancellationTokenSource();
        }

        public Process Process { get; private set; }
        public TimeSpan Timeout { get; private set; }
        public CancellationTokenSource Cancellation { get; private set; }
        public event EventHandler OnTimeout;

        public void StartWatch()
        {
            if (Cancellation == null || Cancellation.IsCancellationRequested)
            {
                throw new Exception("Watcher has been disposed.");
            }

            Task.Delay(Timeout, Cancellation.Token).ContinueWith((task) =>
            {
                if (Cancellation != null && !Cancellation.Token.IsCancellationRequested)
                {
                    try
                    {
                        Process.CloseMainWindow();
                        Process.Close();
                        Process.Dispose();

                        if (!Process.HasExited)
                        {
                            Process.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        var source = typeof(ProcessWatcher).Name;
                        LogFactory.GetLogger().Error(source, ex);
                    }
                    finally
                    {
                        if (OnTimeout != null)
                        {
                            OnTimeout.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            if (Cancellation != null)
            {
                Cancellation.Cancel();
                Cancellation.Dispose();
                Cancellation = null;
            }

            Process = null;
        }
    }
}
