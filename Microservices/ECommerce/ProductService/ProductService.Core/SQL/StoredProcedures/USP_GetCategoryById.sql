CREATE OR ALTER PROCEDURE USP_GetCategoryById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CategoryName
    FROM Category WITH(NOLOCK)
    WHERE Id = @CategoryId;
END