-- ProductImage Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ProductImage')
BEGIN
    CREATE TABLE ProductImage (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ImageUrl NVARCHAR(500) NULL,
        CreatedDate DATETIME NOT NULL,
        ModifiedDate DATETIME NULL,
        Title NVARCHAR(255) NULL,
        PhysicalPath NVARCHAR(500) NULL,
        CONSTRAINT FK_ProductImage_Product FOREIGN KEY (ProductId) REFERENCES Product(Id)
    );
END;