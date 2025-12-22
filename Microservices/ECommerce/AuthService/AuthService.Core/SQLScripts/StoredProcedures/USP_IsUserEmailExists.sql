CREATE OR ALTER PROCEDURE dbo.USP_IsUserEmailExists
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM dbo.Users WITH(NOLOCK)
                WHERE Email = @Email
            )
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS IsEmailExists;
END;
GO
