USE MiniAccountingDB;
GO

CREATE PROCEDURE sp_SaveVoucher
    @VoucherType VARCHAR(10),
    @VoucherDate DATE,
    @ReferenceNo VARCHAR(50),
    @CreatedBy NVARCHAR(450),
    @VoucherDetails VoucherDetailsType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY

        -- Validate that debit = credit
        DECLARE @TotalDebit DECIMAL(18,2) = 0;
        DECLARE @TotalCredit DECIMAL(18,2) = 0;

        SELECT 
            @TotalDebit = SUM(Debit),
            @TotalCredit = SUM(Credit)
        FROM @VoucherDetails;

        IF @TotalDebit <> @TotalCredit
        BEGIN
            RAISERROR('Total Debit and Credit must be equal.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Check all AccountIds are leaf accounts
        IF EXISTS (
            SELECT 1 
            FROM @VoucherDetails VD
            JOIN Accounts A ON VD.AccountIdNum = A.AccountId
            WHERE EXISTS (
                SELECT 1 FROM Accounts C WHERE C.ParentAccountId = A.AccountId
            )
        )
        BEGIN
            RAISERROR('Voucher can only be posted to leaf accounts.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insert into Vouchers table
        INSERT INTO Vouchers (VoucherType, VoucherDate, ReferenceNo, CreatedBy)
        VALUES (@VoucherType, @VoucherDate, @ReferenceNo, @CreatedBy);

        DECLARE @VoucherId INT = SCOPE_IDENTITY();

        -- Insert into VoucherDetails
        INSERT INTO VoucherDetails (VoucherId, AccountIdValue, Debit, Credit, Description)
        SELECT 
            @VoucherId, 
            AccountIdNum, 
            Debit, 
            Credit, 
            Description
        FROM @VoucherDetails;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrMsg NVARCHAR(4000), @ErrSeverity INT;
        SELECT @ErrMsg = ERROR_MESSAGE(), @ErrSeverity = ERROR_SEVERITY();
        RAISERROR (@ErrMsg, @ErrSeverity, 1);
    END CATCH
END 


