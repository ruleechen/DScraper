using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
                throw new ArgumentNullException(nameof(process));
            }

            Process = process;
            Timeout = timeout;
        }

        public Process Process { get; set; }

        public TimeSpan Timeout { get; set; }

        private class WatchTaskModel
        {
            public Process Process { get; set; }
            public TimeSpan Timeout { get; set; }
            public CancellationToken Token { get; set; }
        }

        private CancellationTokenSource _source;
        private void StartWatch()
        {
            if (Timeout > TimeSpan.MinValue)
            {
                _source = new CancellationTokenSource();
                var task = Task.Factory.StartNew((state) =>
                {
                    var param = (WatchTaskModel)state;
                    Thread.Sleep(param.Timeout);
                    if (!param.Token.IsCancellationRequested)
                    {
                        param.Process.Close();
                        param.Process.Dispose();
                    }
                },
                new WatchTaskModel
                {
                    Process = Process,
                    Timeout = Timeout,
                    Token = _source.Token
                },
                _source.Token,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
            }
        }

        public void Dispose()
        {
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
            }

            Process = null;
        }
    }
}
