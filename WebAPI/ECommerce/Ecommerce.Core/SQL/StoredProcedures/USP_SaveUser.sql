CREATE OR ALTER PROCEDURE [dbo].[USP_SaveUser]
    @UserName     NVARCHAR(100),
    @Email        NVARCHAR(256),
    @PasswordSalt NVARCHAR(256),
    @FirstName    NVARCHAR(100) = NULL,
    @LastName     NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
        INSERT INTO dbo.Users (UserName, Email, PasswordSalt, FirstName, LastName)
        VALUES (@UserName, @Email, @PasswordSalt, @FirstName, @LastName);

        DECLARE @NewId INT = CONVERT(INT, SCOPE_IDENTITY());

        -- Return the newly created id
        SELECT @NewId AS NewUserId;
END
GO