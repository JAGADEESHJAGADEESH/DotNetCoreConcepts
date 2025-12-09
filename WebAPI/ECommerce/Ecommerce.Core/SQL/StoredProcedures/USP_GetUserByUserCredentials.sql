CREATE OR ALTER PROCEDURE [dbo].[USP_GetUserByUserCredentials]
	-- Add the parameters for the stored procedure here
	@Email [nvarchar](100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 *
	FROM dbo.[Users]
	WHERE Email = @Email
END
GO
