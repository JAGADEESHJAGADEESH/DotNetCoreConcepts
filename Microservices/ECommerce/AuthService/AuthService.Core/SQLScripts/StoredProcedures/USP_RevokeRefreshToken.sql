CREATE OR ALTER PROCEDURE USP_RevokeRefreshToken
    @TokenId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET IsRevoked = 1
    WHERE Id = @TokenId;
END;
GO
