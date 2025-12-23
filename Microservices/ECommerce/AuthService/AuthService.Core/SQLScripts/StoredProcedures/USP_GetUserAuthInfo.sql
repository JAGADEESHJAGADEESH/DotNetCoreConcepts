CREATE OR ALTER PROCEDURE dbo.USP_GetUserAuthInfo
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Email,
        u.PasswordHash,
        u.PasswordSalt,
        r.RoleName
    FROM dbo.Users u WITH (NOLOCK)
    INNER JOIN dbo.Roles r WITH (NOLOCK)
        ON u.RoleId = r.Id
    WHERE u.Email = @Email
      AND u.IsActive = 1;
END;
GO
