CREATE OR ALTER PROCEDURE dbo.USP_SaveUser
    @UserName     NVARCHAR(100),
    @Email        NVARCHAR(256),
    @Password     NVARCHAR(256),
    @PasswordSalt NVARCHAR(256),
    @FirstName    NVARCHAR(100) = NULL,
    @LastName     NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Prevent duplicate username/email (acquire update lock to prevent race)
        IF EXISTS (
            SELECT 1
            FROM dbo.Users WITH (UPDLOCK, HOLDLOCK)
            WHERE UserName = @UserName OR Email = @Email
        )
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('A user with the specified username or email already exists.', 16, 1);
            RETURN;
        END

        INSERT INTO dbo.Users (UserName, Email, Password, PasswordSalt, FirstName, LastName)
        VALUES (@UserName, @Email, @Password, @PasswordSalt, @FirstName, @LastName);

        DECLARE @NewId INT = CONVERT(INT, SCOPE_IDENTITY());

        COMMIT TRANSACTION;

        -- Return the newly created id
        SELECT @NewId AS NewUserId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();

        -- Re-raise the error with original severity/state
        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH
END
