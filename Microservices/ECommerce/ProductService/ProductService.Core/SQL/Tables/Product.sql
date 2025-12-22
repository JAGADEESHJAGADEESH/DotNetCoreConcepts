-- Product Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Product')
BEGIN
    CREATE TABLE Product (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        ProductName NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        InStock BIT NOT NULL,
        CreatedDate DATETIME NOT NULL,
        ModifiedDate DATETIME NULL,
        CategoryId INT NULL,
        CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryId) REFERENCES Category(Id)
    );
END;
