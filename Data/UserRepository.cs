using System.Data;
using Microsoft.Data.SqlClient;

namespace BT5_1_LoginForm.Data;

internal sealed class UserRepository
{
    internal Task InitializeAsync() => Task.Run(() =>
    {
        Database.EnsureCreated();
        using var connection = Database.OpenConnection();
        using var transaction = connection.BeginTransaction();
        using var schema = connection.CreateCommand();
        schema.Transaction = transaction;
        schema.CommandText = Database.ReadSchema();
        schema.ExecuteNonQuery();

        // Account for this exercise only; an existing password is never overwritten.
        var password = PasswordHasher.Create("Admin@123");
        using var seed = connection.CreateCommand();
        seed.Transaction = transaction;
        seed.CommandText = """
            IF NOT EXISTS (SELECT 1 FROM dbo.Users WITH (UPDLOCK, HOLDLOCK) WHERE Username = @username)
                INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, PasswordIterations)
                VALUES (@username, @hash, @salt, @iterations);
            """;
        seed.Parameters.Add("@username", SqlDbType.NVarChar, 100).Value = "admin";
        seed.Parameters.Add("@hash", SqlDbType.VarBinary, 32).Value = password.Hash;
        seed.Parameters.Add("@salt", SqlDbType.VarBinary, 16).Value = password.Salt;
        seed.Parameters.Add("@iterations", SqlDbType.Int).Value = PasswordHasher.Iterations;
        seed.ExecuteNonQuery();
        transaction.Commit();
    });

    internal Task<bool> AuthenticateAsync(string username, string password) => Task.Run(() =>
    {
        using var connection = Database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT PasswordHash, PasswordSalt, PasswordIterations
            FROM Users WHERE Username = @username;
            """;
        command.Parameters.Add("@username", SqlDbType.NVarChar, 100).Value = username.Trim();
        using var reader = command.ExecuteReader();
        return reader.Read() && PasswordHasher.Verify(password,
            (byte[])reader[0], (byte[])reader[1], reader.GetInt32(2));
    });
}
