namespace Internet_1.Controllers;
public static class ConnectionHelper
{
    private static readonly string _connectionString = "Server=OKE;Database=oguzproject;Trusted_Connection=True;";

    public static string GetConnectionString()
    {
        return _connectionString;
    }
}
