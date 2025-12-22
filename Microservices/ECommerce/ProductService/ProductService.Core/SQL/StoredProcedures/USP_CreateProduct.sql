CREATE OR ALTER PROCEDURE USP_CreateProduct
    @ProductName NVARCHAR(255),
    @Description NVARCHAR(MAX) = NULL,
    @Price DECIMAL(18,2),
    @Quantity INT,
    @CategoryId INT = NULL,
    @ImageUrl NVARCHAR(500) = NULL,
    @Title NVARCHAR(255) = NULL,
    @PhysicalPath NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewProductId UNIQUEIDENTIFIER = NEWID();

    DECLARE @CreatedDate DATETIME = GETDATE();
    DECLARE @InStock BIT = CASE WHEN @Quantity > 0 THEN 1 ELSE 0 END;

    -- Insert Product
    INSERT INTO Product (Id, ProductName, Description, Price, Quantity, InStock, CreatedDate, CategoryId)
    VALUES (@NewProductId, @ProductName, @Description, @Price, @Quantity, @InStock, @CreatedDate, @CategoryId);

    -- Insert ProductImage if provided
    IF @ImageUrl IS NOT NULL OR @Title IS NOT NULL OR @PhysicalPath IS NOT NULL
    BEGIN
        INSERT INTO ProductImage (Id, ProductId, ImageUrl, CreatedDate, Title, PhysicalPath)
        VALUES (NEWID(), @NewProductId, @ImageUrl, GETDATE(), @Title, @PhysicalPath);
    END

    -- Return the newly created Product Id
    SELECT @NewProductId AS ProductId;

END