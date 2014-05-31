using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using Hermes.Serialization;

namespace Hermes.Monitoring.DataAcess
{
    public class TableSchemaMonitorEventArgs
    {
        public IDictionary<string, TableSchema> TableSchemas { get; private set; }
        public TimeSpan MonitorPeriod { get; private set; }

        public TableSchemaMonitorEventArgs(IDictionary<string, TableSchema> tableSchemas, TimeSpan monitorPeriod)
        {
            TableSchemas = tableSchemas;
            MonitorPeriod = monitorPeriod;
        }
    }

    public delegate void TableSchemaMonitorEventHandler(object sender, TableSchemaMonitorEventArgs e);
    
    public class TableSchemaMonitor
    {
        private static BrokerDataAccess brokerDataAccess;
        private readonly Timer timer;
        private TimeSpan monitoringPeriod = TimeSpan.FromMinutes(5);
        private bool disposed;

        public event TableSchemaMonitorEventHandler OnTableSchemaMonitorComplete;

        public TableSchemaMonitor(ISerializeObjects serializeObjects)
        {
            brokerDataAccess = new BrokerDataAccess(serializeObjects);

            timer = new Timer
            {
                Interval = monitoringPeriod.TotalMilliseconds,
                AutoReset = true,
            };

            timer.Elapsed += Elapsed;
            timer.Start();
        }

        void Elapsed(object sender, ElapsedEventArgs e)
        {
            var tableSchemas = brokerDataAccess.GetTableSchemas();

            if (OnTableSchemaMonitorComplete != null)
            {
                OnTableSchemaMonitorComplete(this, new TableSchemaMonitorEventArgs(tableSchemas, monitoringPeriod));
            }
        }

        ~TableSchemaMonitor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                timer.Dispose();
            }

            disposed = true;
        }
    }
}
