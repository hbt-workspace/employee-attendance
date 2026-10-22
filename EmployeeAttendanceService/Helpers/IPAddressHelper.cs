namespace EmployeeAttendanceService.Helpers;

// IPAddressHelper class for IP address helper methods.
public static class IPAddressHelper
{
    #region Get Local IP Address

    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(
            System.Net.Dns.GetHostName());

        var ipAddress = host.AddressList.FirstOrDefault(ip =>
            ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
            !System.Net.IPAddress.IsLoopback(ip));

        return ipAddress?.ToString() ?? string.Empty;
    }

    #endregion

    #region Get Public IP Address

    public static async Task<string> GetPublicIPAddressAsync()
    {
        using var httpClient = new HttpClient();

        string? ipAddress = await httpClient.GetStringAsync("https://api.ipify.org");

        return ipAddress.Trim();
    }

    #endregion
}