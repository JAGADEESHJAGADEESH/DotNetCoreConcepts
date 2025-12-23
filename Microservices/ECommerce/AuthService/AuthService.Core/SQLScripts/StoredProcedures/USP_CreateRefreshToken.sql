CREATE OR ALTER PROCEDURE USP_CreateRefreshToken
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @TokenHash NVARCHAR(256),
    @ExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO RefreshTokens
    (
        Id,
        UserId,
        TokenHash,
        ExpiresAt,
        IsRevoked
    )
    VALUES
    (
        @Id,
        @UserId,
        @TokenHash,
        @ExpiresAt,
        0
    );
END;
GO
