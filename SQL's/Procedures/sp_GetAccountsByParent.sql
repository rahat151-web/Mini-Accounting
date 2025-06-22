USE MiniAccountingDB;
GO

CREATE PROCEDURE sp_GetAccountsByParent
    @ParentAccountId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ParentAccountId IS NULL
    BEGIN
        SELECT AccountId, AccountCode, AccountName, ParentAccountId, AccountType,
		-- Check if account is leaf
            CASE 
                WHEN EXISTS (SELECT 1 FROM Accounts C WHERE C.ParentAccountId = Accounts.AccountId) THEN 0
                ELSE 1
            END AS IsLeaf,

            -- Check if account has vouchers
            CASE 
                WHEN EXISTS (SELECT 1 FROM VoucherDetails V WHERE V.AccountIdValue = Accounts.AccountId) THEN 1
                ELSE 0
            END AS HasVoucher
        FROM Accounts
        WHERE ParentAccountId IS NULL
        ORDER BY AccountCode
    END
    ELSE
    BEGIN
        SELECT AccountId, AccountCode, AccountName, ParentAccountId, AccountType,
		 -- Check if account is leaf
            CASE 
                WHEN EXISTS (SELECT 1 FROM Accounts C WHERE C.ParentAccountId = Accounts.AccountId) THEN 0
                ELSE 1
            END AS IsLeaf,

            -- Check if account has vouchers
            CASE 
                WHEN EXISTS (SELECT 1 FROM VoucherDetails V WHERE V.AccountIdValue = Accounts.AccountId) THEN 1
                ELSE 0
            END AS HasVoucher
        FROM Accounts
        WHERE ParentAccountId = @ParentAccountId
        ORDER BY AccountCode
    END
END