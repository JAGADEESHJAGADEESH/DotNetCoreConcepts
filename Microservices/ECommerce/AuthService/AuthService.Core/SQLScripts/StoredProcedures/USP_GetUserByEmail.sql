CREATE OR ALTER PROCEDURE dbo.USP_GetUserByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FirstName,
        LastName,
        Email,
        Username,
        PasswordHash,
        PasswordSalt,
        IsActive,
        CreatedAt
    FROM dbo.Users WITH(NOLOCK)
    WHERE Email = @Email
      AND IsActive = 1;
END;
GO
