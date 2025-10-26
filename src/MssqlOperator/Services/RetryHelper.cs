namespace MssqlOperator.Services;

public static class RetryHelper
{
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        int delayMs = 1000)
    {
        var retries = maxRetries;
        
        while (true)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                if (!DefaultShouldRetry(ex) || --retries == 0)
                    throw;
                
                await Task.Delay(delayMs);
            }
        }
    }

    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int maxRetries = 3,
        int delayMs = 1000)
    {
        var retries = maxRetries;
        
        while (true)
        {
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                if (!DefaultShouldRetry(ex) || --retries == 0)
                    throw;
                
                Thread.Sleep(delayMs);
            }
        }
    }

    private static bool DefaultShouldRetry(Exception ex)
    {
        var message = ex.Message.ToLowerInvariant();
        
        return message.Contains("timeout") ||
               message.Contains("connection") ||
               message.Contains("network") ||
               message.Contains("deadlock") ||
               message.Contains("temporary") ||
               message.Contains("busy");
    }
}
