CREATE OR ALTER PROCEDURE USP_GetRoleByRoleId
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        RoleName
    FROM Roles
    WHERE Id = @RoleId;
END;
GO