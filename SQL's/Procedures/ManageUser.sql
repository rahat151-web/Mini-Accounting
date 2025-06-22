USE MiniAccountingDB;
GO

CREATE PROCEDURE ManageUser
    @Action NVARCHAR(10), -- INSERT, UPDATE, DELETE

    -- Common fields
    @Id NVARCHAR(450),
    @UserName NVARCHAR(256) = NULL,
    @NormalizedUserName NVARCHAR(256) = NULL,
    @Email NVARCHAR(256) = NULL,
    @NormalizedEmail NVARCHAR(256) = NULL,
    @EmailConfirmed BIT = NULL,
    @PasswordHash NVARCHAR(MAX) = NULL,
    @SecurityStamp NVARCHAR(MAX) = NULL,
    @ConcurrencyStamp NVARCHAR(MAX) = NULL,
    @PhoneNumber NVARCHAR(MAX) = NULL,
    @PhoneNumberConfirmed BIT = NULL,
    @TwoFactorEnabled BIT = NULL,
    @LockoutEnd DATETIMEOFFSET = NULL,
    @LockoutEnabled BIT = NULL,
    @AccessFailedCount INT = NULL,
    @RoleName NVARCHAR(256) = NULL,
	@RoleId NVARCHAR(450) = NULL -- This is used as a local variable
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Action = 'INSERT'
        BEGIN
            -- Insert into AspNetUsers
            INSERT INTO AspNetUsers (
                Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
                TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
            )
            VALUES (
                @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed,
                @PasswordHash, @SecurityStamp, @ConcurrencyStamp, @PhoneNumber, @PhoneNumberConfirmed,
                @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount
            );

            -- Get RoleId from RoleName
            SELECT @RoleId = Id FROM AspNetRoles WHERE NormalizedName = UPPER(@RoleName);

            IF @RoleId IS NULL
            BEGIN
                THROW 50000, 'Invalid Role Name', 1;
            END

            -- Insert into AspNetUserRoles
            INSERT INTO AspNetUserRoles (UserId, RoleId)
            VALUES (@Id, @RoleId);
        END

        ELSE IF @Action = 'UPDATE'
        BEGIN
            -- Update AspNetUsers
			IF @PasswordHash IS NULL
			BEGIN
            UPDATE AspNetUsers
            SET
                
                
                Email = @Email,
                NormalizedEmail = @NormalizedEmail,
                EmailConfirmed = @EmailConfirmed,
                SecurityStamp = @SecurityStamp,
                ConcurrencyStamp = @ConcurrencyStamp,
                PhoneNumber = @PhoneNumber,
                PhoneNumberConfirmed = @PhoneNumberConfirmed,
                TwoFactorEnabled = @TwoFactorEnabled,
                LockoutEnd = @LockoutEnd,
                LockoutEnabled = @LockoutEnabled,
                AccessFailedCount = @AccessFailedCount
            WHERE Id = @Id;
			END

			ELSE 
			BEGIN
            UPDATE AspNetUsers
            SET
                
                
                Email = @Email,
                NormalizedEmail = @NormalizedEmail,
                EmailConfirmed = @EmailConfirmed,
                PasswordHash = @PasswordHash,
                SecurityStamp = @SecurityStamp,
                ConcurrencyStamp = @ConcurrencyStamp,
                PhoneNumber = @PhoneNumber,
                PhoneNumberConfirmed = @PhoneNumberConfirmed,
                TwoFactorEnabled = @TwoFactorEnabled,
                LockoutEnd = @LockoutEnd,
                LockoutEnabled = @LockoutEnabled,
                AccessFailedCount = @AccessFailedCount
            WHERE Id = @Id;
			END

            -- Update Role if provided
            IF @RoleName IS NOT NULL
            BEGIN
     
                SELECT @RoleId = Id FROM AspNetRoles WHERE NormalizedName = UPPER(@RoleName);

                IF @RoleId IS NULL
                BEGIN
                    THROW 50001, 'Invalid Role Name', 1;
                END

                -- Delete previous role mapping
                DELETE FROM AspNetUserRoles WHERE UserId = @Id;

                -- Insert new role mapping
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                VALUES (@Id, @RoleId);
            END
        END

        ELSE IF @Action = 'DELETE'
        BEGIN
            -- Delete user roles first (FK constraint)
            DELETE FROM AspNetUserRoles WHERE UserId = @Id;

            -- Delete user
            DELETE FROM AspNetUsers WHERE Id = @Id;
        END

        ELSE
        BEGIN
            THROW 50002, 'Invalid Action', 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO






