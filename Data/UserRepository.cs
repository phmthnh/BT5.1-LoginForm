using Microsoft.Data.Sqlite;

namespace BT5_1_LoginForm.Data;

internal sealed class UserRepository
{
    internal Task InitializeAsync() => Task.Run(() =>
    {
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
            INSERT INTO Users (Username, PasswordHash, PasswordSalt, PasswordIterations)
            VALUES ($username, $hash, $salt, $iterations)
            ON CONFLICT(Username) DO NOTHING;
            """;
        seed.Parameters.Add("$username", SqliteType.Text).Value = "admin";
        seed.Parameters.Add("$hash", SqliteType.Blob).Value = password.Hash;
        seed.Parameters.Add("$salt", SqliteType.Blob).Value = password.Salt;
        seed.Parameters.Add("$iterations", SqliteType.Integer).Value = PasswordHasher.Iterations;
        seed.ExecuteNonQuery();
        transaction.Commit();
    });

    internal Task<bool> AuthenticateAsync(string username, string password) => Task.Run(() =>
    {
        using var connection = Database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT PasswordHash, PasswordSalt, PasswordIterations
            FROM Users WHERE Username = $username;
            """;
        command.Parameters.Add("$username", SqliteType.Text).Value = username.Trim();
        using var reader = command.ExecuteReader();
        return reader.Read() && PasswordHasher.Verify(password,
            (byte[])reader[0], (byte[])reader[1], reader.GetInt32(2));
    });
}
