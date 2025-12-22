CREATE OR ALTER PROCEDURE USP_CreateCategory
    @CategoryName NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();

    -- Insert into Category table
    INSERT INTO Category (CategoryName)
    VALUES (@CategoryName);

    -- Return the newly created Id (assuming Id is an INT identity column)
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END