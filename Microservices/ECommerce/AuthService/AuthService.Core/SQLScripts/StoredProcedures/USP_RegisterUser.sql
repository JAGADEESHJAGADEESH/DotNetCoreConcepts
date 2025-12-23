CREATE OR ALTER PROCEDURE dbo.USP_RegisterUser
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(256),
    @Username NVARCHAR(100),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX),
    @RoleId INT,
    @IsActive BIT = 1,
    @CreatedAt DATETIME2 
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
        RoleId,
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
        @RoleId,
        @IsActive,  
        @CreatedAt
    );

    -- Return success indicator
    SELECT @UserId as UserId;
END;
GO
