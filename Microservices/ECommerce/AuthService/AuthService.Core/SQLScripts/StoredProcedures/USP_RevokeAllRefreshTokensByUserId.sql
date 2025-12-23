CREATE OR ALTER PROCEDURE USP_RevokeAllRefreshTokensByUserId
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET IsRevoked = 1
    WHERE UserId = @UserId;
END;
GO
