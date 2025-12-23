CREATE OR ALTER PROCEDURE USP_GetRefreshTokenByTokenHash
    @TokenHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        TokenHash,
        ExpiresAt,
        IsRevoked
    FROM RefreshTokens
    WHERE TokenHash = @TokenHash;
END;
GO
