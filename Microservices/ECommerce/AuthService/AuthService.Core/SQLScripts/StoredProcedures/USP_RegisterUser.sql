CREATE OR ALTER PROCEDURE dbo.USP_RegisterUser
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(256),
    @Username NVARCHAR(100),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.Users
    (
        Id,
        FirstName,
        LastName,
        Email,
        Username,
        PasswordHash,
        PasswordSalt,
        IsActive,
        CreatedAt
    )
    VALUES
    (
        @UserId,
        @FirstName,
        @LastName,
        @Email,
        @Username,
        @PasswordHash,
        @PasswordSalt,
        1,
        SYSUTCDATETIME()
    );

    -- Return success indicator
    SELECT 1;
END;
GO
