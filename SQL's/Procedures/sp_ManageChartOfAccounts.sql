USE MiniAccountingDB;
GO

CREATE PROCEDURE sp_ManageChartOfAccounts
  @Action VARCHAR(10),  -- 'INSERT', 'UPDATE', 'DELETE'
  @AccountId INT = NULL,
  @AccountCode VARCHAR(20) = NULL,
  @AccountName NVARCHAR(100) = NULL,
  @ParentAccountId INT = NULL,
  @AccountType NVARCHAR(50) = NULL 
AS
BEGIN
  -- INSERT: Validate parent type matches new account type
  IF @Action = 'INSERT'
  BEGIN

   IF @AccountCode IS NULL OR @AccountName IS NULL OR  @AccountType IS NULL
	BEGIN
      RAISERROR('Provide all necessary values for inserting a new account.', 16, 1);
      RETURN;
    END

    IF @ParentAccountId IS NOT NULL
      AND (SELECT AccountType FROM Accounts WHERE AccountId = @ParentAccountId)  != @AccountType
    BEGIN
      RAISERROR('Parent account type mismatch', 16, 1);
      RETURN;
    END

	

    IF
	  EXISTS (SELECT 1 FROM Accounts WHERE AccountCode = @AccountCode) 
	BEGIN
      RAISERROR('This AccountCode is taken, Choose another', 16, 1);
      RETURN;
    END

	

    INSERT INTO Accounts (AccountCode, AccountName, ParentAccountId, AccountType)
    VALUES (@AccountCode, @AccountName, @ParentAccountId, @AccountType);
  END

  -- UPDATE: Prevent type change
  ELSE IF @Action = 'UPDATE'
  BEGIN
    

	

	IF NOT EXISTS (SELECT AccountId FROM Accounts WHERE AccountId = @AccountId)
	BEGIN
      RAISERROR('No account exists with the Id', 16, 1);
      RETURN;
    END


    UPDATE Accounts
    SET 
      AccountName = @AccountName

    WHERE AccountId = @AccountId;
  END

-- DELETE: Soft-delete only if no children/vouchers
  ELSE IF @Action = 'DELETE'
  BEGIN

   IF NOT EXISTS (SELECT AccountId FROM Accounts WHERE AccountId = @AccountId)
	BEGIN
      RAISERROR('No account exists with the Id', 16, 1);
      RETURN;
    END

    IF EXISTS (SELECT 1 FROM Accounts WHERE ParentAccountId = @AccountId)
      OR EXISTS (SELECT 1 FROM VoucherDetails WHERE AccountIdValue = @AccountId)
    BEGIN
      RAISERROR('Account has dependencies', 16, 1);
      RETURN;
    END



    DELETE FROM Accounts WHERE AccountId = @AccountId;
  END

END



