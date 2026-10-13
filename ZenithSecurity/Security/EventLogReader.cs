using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ZenithSecurity.Security
{
    public class EventLogReader
    {
        public async Task<object> GetSecurityEventsAsync(int count = 50) => await Task.Run(() =>
            GetEvents("Security", count));

        public async Task<object> GetSystemEventsAsync(int count = 50) => await Task.Run(() =>
            GetEvents("System", count));

        private static object GetEvents(string logName, int count)
        {
            var events = new List<object>();
            try
            {
                using var log = new EventLog(logName);
                var entries = log.Entries;
                int total   = entries.Count;
                int start   = Math.Max(0, total - count);

                for (int i = total - 1; i >= start && events.Count < count; i--)
                {
                    try
                    {
                        var e = entries[i];
                        events.Add(new
                        {
                            id        = e.InstanceId,
                            time      = e.TimeGenerated.ToString("yyyy-MM-dd HH:mm:ss"),
                            level     = e.EntryType.ToString(),
                            source    = e.Source,
                            message   = e.Message?.Length > 300
                                        ? e.Message[..300] + "…" : e.Message ?? ""
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                return new { error = ex.Message, events };
            }
            return new { log = logName, count = events.Count, events };
        }
    }
}
