using Microsoft.Data.SqlClient;

namespace TaskFlow.Helpers
{
    public static class DatabaseAccessHelper
    {
        public static bool IsLikelyDatabaseAccessFailure(Exception ex)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                if (e is SqlException)
                    return true;
                if (e is TimeoutException)
                    return true;
                if (e is InvalidOperationException io && io.Message.Contains("Connection", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            var msg = ex.Message;
            if (msg.Contains("network", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("transport", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("timeout", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("could not open", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("estabelecer conex", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("SQL Server", StringComparison.OrdinalIgnoreCase) && msg.Contains("acess", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}