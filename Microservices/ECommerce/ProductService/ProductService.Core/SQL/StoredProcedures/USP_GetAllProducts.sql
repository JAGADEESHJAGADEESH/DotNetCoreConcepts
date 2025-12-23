CREATE OR ALTER PROCEDURE USP_GetAllProducts
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT 
            p.Id,
            p.ProductName,
            p.Description,
            p.Price,
            p.Quantity,
            p.InStock,
            p.CreatedDate,
            p.ModifiedDate,
            p.CategoryId,
            c.CategoryName,
            Pi.Id AS ImageId,
            pi.ProductId,
            pi.ImageUrl,
            pi.CreatedDate AS ImageCreatedDate,
            pi.ModifiedDate AS ImageModifiedDate,
            pi.Title,
            pi.PhysicalPath
        FROM Product p
        LEFT JOIN Category c ON p.CategoryId = c.Id
        LEFT JOIN ProductImage pi ON p.Id = pi.ProductId;
    END
