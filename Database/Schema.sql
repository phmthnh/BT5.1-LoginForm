CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL COLLATE NOCASE UNIQUE
        CHECK (length(trim(Username)) BETWEEN 1 AND 100),
    PasswordHash BLOB NOT NULL CHECK (length(PasswordHash) = 32),
    PasswordSalt BLOB NOT NULL CHECK (length(PasswordSalt) = 16),
    PasswordIterations INTEGER NOT NULL CHECK (PasswordIterations BETWEEN 100000 AND 2000000)
);
