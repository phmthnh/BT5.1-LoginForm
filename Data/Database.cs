using System.Data;
using Microsoft.Data.SqlClient;

namespace BT5_1_LoginForm.Data;

internal static class Database
{
    internal static string ConnectionString =>
        Environment.GetEnvironmentVariable("BT51_CONNECTION_STRING") is { Length: > 0 } custom
            ? custom
            : @"Server=(localdb)\LTNET_BT5_SQL;Database=BT51_LoginForm;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;Connect Timeout=15";

    internal static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        try { connection.Open(); return connection; }
        catch { connection.Dispose(); throw; }
    }

    internal static void EnsureCreated()
    {
        var settings = new SqlConnectionStringBuilder(ConnectionString);
        var name = settings.InitialCatalog;
        if (string.IsNullOrWhiteSpace(name) || name.Length > 128 ||
            new[] { "master", "model", "msdb", "tempdb" }.Contains(name, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Chuỗi kết nối phải chỉ định một CSDL riêng cho bài tập.");

        settings.InitialCatalog = "master";
        using var connection = new SqlConnection(settings.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            IF DB_ID(@name) IS NULL
            BEGIN
                DECLARE @sql nvarchar(max) = N'CREATE DATABASE ' + QUOTENAME(@name);
                EXEC sys.sp_executesql @sql;
            END;
            """;
        command.Parameters.Add("@name", SqlDbType.NVarChar, 128).Value = name;
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) when (ex.Number == 1801) { /* Another instance created the database. */ }
    }

    internal static string ReadSchema()
    {
        using var stream = typeof(Database).Assembly.GetManifestResourceStream("DatabaseSchema.sql")
            ?? throw new InvalidOperationException("Không tìm thấy SQL khởi tạo cơ sở dữ liệu.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
