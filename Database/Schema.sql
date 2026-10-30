IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Username nvarchar(100) COLLATE Latin1_General_100_CI_AS NOT NULL UNIQUE
            CHECK (LEN(LTRIM(RTRIM(Username))) BETWEEN 1 AND 100),
        PasswordHash varbinary(32) NOT NULL CHECK (DATALENGTH(PasswordHash) = 32),
        PasswordSalt varbinary(16) NOT NULL CHECK (DATALENGTH(PasswordSalt) = 16),
        PasswordIterations int NOT NULL CHECK (PasswordIterations BETWEEN 100000 AND 2000000)
    );
END;
