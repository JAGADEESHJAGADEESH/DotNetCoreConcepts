CREATE OR ALTER PROCEDURE dbo.USP_RegisterUser
    @Email NVARCHAR(256),
    @Username NVARCHAR(100),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX),
    @IsActive BIT = 1,
    @CreatedAt DATETIME2 
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.Users
    (
        Id,
        Email,
        Username,
        PasswordHash,
        PasswordSalt,
        RoleId,
        IsActive,
        CreatedAt
    )
    VALUES
    (
        @UserId,
        @Email,
        @Username,
        @PasswordHash,
        @PasswordSalt,
        2,  -- Default RoleId for regular users
        @IsActive,  
        @CreatedAt
    );

    -- Return success indicator
    SELECT @UserId as UserId;
END;
GO
