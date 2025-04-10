IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241125055554_InitialCreateWithIdentity', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserTokens]') AND [c].[name] = N'Name');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AspNetUserTokens] ALTER COLUMN [Name] nvarchar(450) NOT NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserTokens]') AND [c].[name] = N'LoginProvider');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUserTokens] ALTER COLUMN [LoginProvider] nvarchar(450) NOT NULL;
GO

ALTER TABLE [AspNetUsers] ADD [Address] nvarchar(150) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [ProfilePicture] varbinary(max) NOT NULL DEFAULT 0x;
GO

ALTER TABLE [AspNetUsers] ADD [Resturant_Id] int NOT NULL DEFAULT 0;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserLogins]') AND [c].[name] = N'ProviderKey');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [AspNetUserLogins] ALTER COLUMN [ProviderKey] nvarchar(450) NOT NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserLogins]') AND [c].[name] = N'LoginProvider');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [AspNetUserLogins] ALTER COLUMN [LoginProvider] nvarchar(450) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241125060835_UpdateIdentity', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'ProfilePicture');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [ProfilePicture] varbinary(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241125071012_UpdateIdentityProfilePictureBeNull', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ApplicationModelVM] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [ConfirmPassword] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(11) NOT NULL,
    CONSTRAINT [PK_ApplicationModelVM] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241125213538_UploadApplicationModelVM', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [LoginVM] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [RememberMe] bit NOT NULL,
    CONSTRAINT [PK_LoginVM] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241125225052_UploadLoginVMModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [FirstName] nvarchar(15) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [Gender] nvarchar(6) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [LastName] nvarchar(15) NULL;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApplicationModelVM]') AND [c].[name] = N'Address');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ApplicationModelVM] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [ApplicationModelVM] ALTER COLUMN [Address] nvarchar(100) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241126133853_UpdateApplicationUserTocontailFNameandLNameAndGender', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Gender');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Gender] int NULL;
GO

ALTER TABLE [AspNetUsers] ADD [BirthDate] date NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241126134848_UpdateApplicationUserProperties', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NULL;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FirstName');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [FirstName] nvarchar(max) NULL;
GO

CREATE TABLE [ProfileVM] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(15) NULL,
    [LastName] nvarchar(15) NULL,
    [ProfilePicture] varbinary(max) NULL,
    [Gender] int NULL,
    [BirthDate] date NULL,
    CONSTRAINT [PK_ProfileVM] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241126140501_AddProfileVM', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(15) NULL;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FirstName');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [FirstName] nvarchar(15) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241126140636_AlterApplicatioUser', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ProfileVM] ADD [Email] nvarchar(100) NOT NULL DEFAULT N'';
GO

ALTER TABLE [ProfileVM] ADD [Password] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241126141023_AlterProfileVM', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProfileVM]') AND [c].[name] = N'Password');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ProfileVM] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [ProfileVM] ALTER COLUMN [Password] nvarchar(max) NULL;
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProfileVM]') AND [c].[name] = N'Email');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ProfileVM] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [ProfileVM] ALTER COLUMN [Email] nvarchar(100) NULL;
GO

ALTER TABLE [ProfileVM] ADD [PhoneNumber] nvarchar(11) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241126184017_AddPhoneNumberToProfileVm', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Addresses] (
    [Id] int NOT NULL IDENTITY,
    [CustomerAddres] nvarchar(max) NOT NULL,
    [ApplicationUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Addresses_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ContactUs] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ContactUs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Points] (
    [Id] int NOT NULL IDENTITY,
    [Points] int NOT NULL,
    [ExpireDate] datetime2 NOT NULL,
    [ApplicationUserId] nvarchar(450) NULL,
    [UserId] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Points] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Points_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Restaurants] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Descriptions] nvarchar(max) NOT NULL,
    [Rate] decimal(18,2) NOT NULL,
    [Logo] nvarchar(max) NOT NULL,
    [DeliveryFee] decimal(18,2) NOT NULL,
    [DeliveryDuration] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Restaurants] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Deliveries] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [RestaurantId] int NOT NULL,
    CONSTRAINT [PK_Deliveries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Deliveries_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Meals] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [IsAvailable] bit NOT NULL,
    [IsNewMeal] bit NOT NULL,
    [IsTrendingMeal] bit NOT NULL,
    [NumberOfServings] int NOT NULL,
    [Category] int NOT NULL,
    [RestaurantId] int NOT NULL,
    CONSTRAINT [PK_Meals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Meals_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [ApplicationUserId] nvarchar(450) NOT NULL,
    [MealId] int NOT NULL,
    [Quantity] int NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [Method] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_Meals_MealId] FOREIGN KEY ([MealId]) REFERENCES [Meals] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CustomerConcerns] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar(max) NOT NULL,
    [OrderId] int NOT NULL,
    [Status] int NOT NULL,
    [DateTime] datetime2 NOT NULL,
    CONSTRAINT [PK_CustomerConcerns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomerConcerns_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Addresses_ApplicationUserId] ON [Addresses] ([ApplicationUserId]);
GO

CREATE INDEX [IX_CustomerConcerns_OrderId] ON [CustomerConcerns] ([OrderId]);
GO

CREATE INDEX [IX_Deliveries_RestaurantId] ON [Deliveries] ([RestaurantId]);
GO

CREATE INDEX [IX_Meals_RestaurantId] ON [Meals] ([RestaurantId]);
GO

CREATE INDEX [IX_Orders_ApplicationUserId] ON [Orders] ([ApplicationUserId]);
GO

CREATE INDEX [IX_Orders_MealId] ON [Orders] ([MealId]);
GO

CREATE INDEX [IX_Points_ApplicationUserId] ON [Points] ([ApplicationUserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241128230839_Add Models With New Edit', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Restaurants].[Descriptions]', N'Description', N'COLUMN';
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Logo');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [Logo] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241128235359_Edit Resturant Model logo can be null', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[ContactUs].[Phone]', N'ResUserName', N'COLUMN';
GO

EXEC sp_rename N'[ContactUs].[Name]', N'ResPhone', N'COLUMN';
GO

EXEC sp_rename N'[ContactUs].[Email]', N'ResName', N'COLUMN';
GO

EXEC sp_rename N'[ContactUs].[Address]', N'ResEmail', N'COLUMN';
GO

ALTER TABLE [Restaurants] ADD [Accecpted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [ContactUs] ADD [ConfirmPassword] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [ContactUs] ADD [Description] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [ContactUs] ADD [Password] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [ContactUs] ADD [ResAddress] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241201123711_Add property Accecpted to Resturant model and update contactus model', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [ContactUs];
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'Quantity');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [Orders] DROP COLUMN [Quantity];
GO

CREATE TABLE [Cart] (
    [Id] int NOT NULL IDENTITY,
    [ResturantId] int NOT NULL,
    [restaurantId] int NOT NULL,
    CONSTRAINT [PK_Cart] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cart_Restaurants_restaurantId] FOREIGN KEY ([restaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderedMeals] (
    [Id] int NOT NULL IDENTITY,
    [ResturantId] int NOT NULL,
    [MealId] int NOT NULL,
    [MealName] nvarchar(max) NOT NULL,
    [MealDescription] nvarchar(max) NOT NULL,
    [Quantity] int NOT NULL,
    [CartId] int NOT NULL,
    [OrderId] int NULL,
    CONSTRAINT [PK_OrderedMeals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderedMeals_Cart_CartId] FOREIGN KEY ([CartId]) REFERENCES [Cart] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderedMeals_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id])
);
GO

CREATE INDEX [IX_Cart_restaurantId] ON [Cart] ([restaurantId]);
GO

CREATE INDEX [IX_OrderedMeals_CartId] ON [OrderedMeals] ([CartId]);
GO

CREATE INDEX [IX_OrderedMeals_OrderId] ON [OrderedMeals] ([OrderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241205104855_Add Cart and OrderedMeals Models', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Cart] DROP CONSTRAINT [FK_Cart_Restaurants_restaurantId];
GO

ALTER TABLE [OrderedMeals] DROP CONSTRAINT [FK_OrderedMeals_Cart_CartId];
GO

EXEC sp_rename N'[Cart].[restaurantId]', N'RestaurantId', N'COLUMN';
GO

EXEC sp_rename N'[Cart].[IX_Cart_restaurantId]', N'IX_Cart_RestaurantId', N'INDEX';
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'CartId');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [OrderedMeals] ALTER COLUMN [CartId] int NULL;
GO

ALTER TABLE [Cart] ADD CONSTRAINT [FK_Cart_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [OrderedMeals] ADD CONSTRAINT [FK_OrderedMeals_Cart_CartId] FOREIGN KEY ([CartId]) REFERENCES [Cart] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241205110942_Edit Cart Model', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Cart] DROP CONSTRAINT [FK_Cart_Restaurants_RestaurantId];
GO

ALTER TABLE [OrderedMeals] DROP CONSTRAINT [FK_OrderedMeals_Orders_OrderId];
GO

DROP INDEX [IX_OrderedMeals_OrderId] ON [OrderedMeals];
GO

DROP INDEX [IX_Cart_RestaurantId] ON [Cart];
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'OrderId');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [OrderedMeals] DROP COLUMN [OrderId];
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cart]') AND [c].[name] = N'RestaurantId');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Cart] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [Cart] DROP COLUMN [RestaurantId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241205111538_Edit Order Model', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderedMeals] DROP CONSTRAINT [FK_OrderedMeals_Cart_CartId];
GO

DROP INDEX [IX_OrderedMeals_CartId] ON [OrderedMeals];
DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'CartId');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var18 + '];');
UPDATE [OrderedMeals] SET [CartId] = 0 WHERE [CartId] IS NULL;
ALTER TABLE [OrderedMeals] ALTER COLUMN [CartId] int NOT NULL;
ALTER TABLE [OrderedMeals] ADD DEFAULT 0 FOR [CartId];
CREATE INDEX [IX_OrderedMeals_CartId] ON [OrderedMeals] ([CartId]);
GO

ALTER TABLE [OrderedMeals] ADD CONSTRAINT [FK_OrderedMeals_Cart_CartId] FOREIGN KEY ([CartId]) REFERENCES [Cart] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241205115454_EditOrderMealsModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'MealDescription');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [OrderedMeals] ALTER COLUMN [MealDescription] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241205120725_EditOrderMealsModelTwo', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Cart] ADD [UserId] nvarchar(450) NOT NULL DEFAULT N'';
GO

CREATE INDEX [IX_Cart_UserId] ON [Cart] ([UserId]);
GO

ALTER TABLE [Cart] ADD CONSTRAINT [FK_Cart_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241205143444_EditcartModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [CustomerConcerns] DROP CONSTRAINT [FK_CustomerConcerns_Orders_OrderId];
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Meals_MealId];
GO

DROP INDEX [IX_Orders_MealId] ON [Orders];
GO

DROP INDEX [IX_CustomerConcerns_OrderId] ON [CustomerConcerns];
GO

EXEC sp_rename N'[Orders].[MealId]', N'ResturantId', N'COLUMN';
GO

EXEC sp_rename N'[CustomerConcerns].[OrderId]', N'ResturantId', N'COLUMN';
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'Notes');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [Orders] ALTER COLUMN [Notes] nvarchar(max) NULL;
GO

ALTER TABLE [Orders] ADD [CartId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Orders] ADD [OrderPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Orders] ADD [RestaurantId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [CustomerConcerns] ADD [RestaurantId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [CustomerConcerns] ADD [UserId] nvarchar(450) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Cart] ADD [RestaurantId] int NULL;
GO

CREATE INDEX [IX_Orders_CartId] ON [Orders] ([CartId]);
GO

CREATE INDEX [IX_Orders_RestaurantId] ON [Orders] ([RestaurantId]);
GO

CREATE INDEX [IX_OrderedMeals_MealId] ON [OrderedMeals] ([MealId]);
GO

CREATE INDEX [IX_CustomerConcerns_RestaurantId] ON [CustomerConcerns] ([RestaurantId]);
GO

CREATE INDEX [IX_CustomerConcerns_UserId] ON [CustomerConcerns] ([UserId]);
GO

CREATE INDEX [IX_Cart_RestaurantId] ON [Cart] ([RestaurantId]);
GO

ALTER TABLE [Cart] ADD CONSTRAINT [FK_Cart_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]);
GO

ALTER TABLE [CustomerConcerns] ADD CONSTRAINT [FK_CustomerConcerns_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [CustomerConcerns] ADD CONSTRAINT [FK_CustomerConcerns_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [OrderedMeals] ADD CONSTRAINT [FK_OrderedMeals_Meals_MealId] FOREIGN KEY ([MealId]) REFERENCES [Meals] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241206174222_Alter Models', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [UserComplaints] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [DateTime] datetime2 NOT NULL,
    [RestaurantId] int NOT NULL,
    CONSTRAINT [PK_UserComplaints] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserComplaints_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_UserComplaints_UserId] ON [UserComplaints] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241206213324_addUserComplaintModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] ADD [CustomerAddres] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241207110318_Alter Order Model', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ResturantId');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [Orders] DROP COLUMN [ResturantId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241207122348_Alter Resturant Model', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Orders].[CartId]', N'CartInOrderId', N'COLUMN';
GO

EXEC sp_rename N'[Orders].[IX_Orders_CartId]', N'IX_Orders_CartInOrderId', N'INDEX';
GO

EXEC sp_rename N'[OrderedMeals].[ResturantId]', N'RestaurantId', N'COLUMN';
GO

ALTER TABLE [OrderedMeals] ADD [CartInOrderId] int NULL;
GO

CREATE TABLE [CartInOrder] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ResturantId] int NOT NULL,
    CONSTRAINT [PK_CartInOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartInOrder_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MealsInOrder] (
    [Id] int NOT NULL IDENTITY,
    [RestaurantId] int NOT NULL,
    [MealId] int NOT NULL,
    [MealName] nvarchar(max) NOT NULL,
    [MealDescription] nvarchar(max) NULL,
    [Quantity] int NOT NULL,
    [CartInOrderId] int NOT NULL,
    CONSTRAINT [PK_MealsInOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MealsInOrder_CartInOrder_CartInOrderId] FOREIGN KEY ([CartInOrderId]) REFERENCES [CartInOrder] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MealsInOrder_Meals_MealId] FOREIGN KEY ([MealId]) REFERENCES [Meals] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_OrderedMeals_CartInOrderId] ON [OrderedMeals] ([CartInOrderId]);
GO

CREATE INDEX [IX_OrderedMeals_RestaurantId] ON [OrderedMeals] ([RestaurantId]);
GO

CREATE INDEX [IX_CartInOrder_UserId] ON [CartInOrder] ([UserId]);
GO

CREATE INDEX [IX_MealsInOrder_CartInOrderId] ON [MealsInOrder] ([CartInOrderId]);
GO

CREATE INDEX [IX_MealsInOrder_MealId] ON [MealsInOrder] ([MealId]);
GO

ALTER TABLE [OrderedMeals] ADD CONSTRAINT [FK_OrderedMeals_CartInOrder_CartInOrderId] FOREIGN KEY ([CartInOrderId]) REFERENCES [CartInOrder] ([Id]);
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_CartInOrder_CartInOrderId] FOREIGN KEY ([CartInOrderId]) REFERENCES [CartInOrder] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241208115618_Add CartInOrder & MealsInOrder Models', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderedMeals] ADD CONSTRAINT [FK_OrderedMeals_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241208120430_alterOrderMealsModels', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_OrderedMeals_CartInOrderId] ON [OrderedMeals];
GO

ALTER TABLE [OrderedMeals] DROP CONSTRAINT [FK_OrderedMeals_CartInOrder_CartInOrderId];
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'CartInOrderId');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [OrderedMeals] DROP COLUMN [CartInOrderId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241208121311_alterOrderMealsDropColumn', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderedMeals] DROP CONSTRAINT [FK_OrderedMeals_Restaurants_RestaurantId];
GO

DROP INDEX [IX_OrderedMeals_RestaurantId] ON [OrderedMeals];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241208134020_DropFKInOrderedMealsModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Accecpted');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Restaurants] DROP COLUMN [Accecpted];
GO

ALTER TABLE [Restaurants] ADD [AcctiveStatus] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241210121049_AddAcctiveStatusEnumToResturantModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Cart].[ResturantId]', N'RestaurantId', N'COLUMN';
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Rate');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [Rate] decimal(18,4) NOT NULL;
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'DeliveryFee');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [DeliveryFee] decimal(18,4) NOT NULL;
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'DeliveryDuration');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [DeliveryDuration] decimal(18,4) NOT NULL;
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Meals]') AND [c].[name] = N'Price');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Meals] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Meals] ALTER COLUMN [Price] decimal(18,4) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250214134218_Edit_Cart_Model', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE INDEX [IX_Cart_RestaurantId] ON [Cart] ([RestaurantId]);
GO

ALTER TABLE [Cart] ADD CONSTRAINT [FK_Cart_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250214201012_LinkCartToRestaurantModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Logo');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [Restaurants] DROP COLUMN [Logo];
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Meals]') AND [c].[name] = N'ImageUrl');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Meals] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [Meals] DROP COLUMN [ImageUrl];
GO

EXEC sp_rename N'[AspNetUsers].[ProfilePicture]', N'Image', N'COLUMN';
GO

ALTER TABLE [Restaurants] ADD [Image] varbinary(max) NULL;
GO

ALTER TABLE [Meals] ADD [Image] varbinary(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250217115438_RefactorImageProperty', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Cart] DROP CONSTRAINT [FK_Cart_AspNetUsers_UserId];
GO

ALTER TABLE [MealsInOrder] DROP CONSTRAINT [FK_MealsInOrder_CartInOrder_CartInOrderId];
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_AspNetUsers_ApplicationUserId];
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_CartInOrder_CartInOrderId];
GO

DROP TABLE [CartInOrder];
GO

DROP INDEX [IX_MealsInOrder_CartInOrderId] ON [MealsInOrder];
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'OrderPrice');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [Orders] DROP COLUMN [OrderPrice];
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'MealDescription');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [OrderedMeals] DROP COLUMN [MealDescription];
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'MealName');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [OrderedMeals] DROP COLUMN [MealName];
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'RestaurantId');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [OrderedMeals] DROP COLUMN [RestaurantId];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsInOrder]') AND [c].[name] = N'CartInOrderId');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [MealsInOrder] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [MealsInOrder] DROP COLUMN [CartInOrderId];
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsInOrder]') AND [c].[name] = N'MealDescription');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [MealsInOrder] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [MealsInOrder] DROP COLUMN [MealDescription];
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsInOrder]') AND [c].[name] = N'MealName');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [MealsInOrder] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [MealsInOrder] DROP COLUMN [MealName];
GO

EXEC sp_rename N'[Orders].[CustomerAddres]', N'AddressId', N'COLUMN';
GO

EXEC sp_rename N'[Orders].[CartInOrderId]', N'AddressId1', N'COLUMN';
GO

EXEC sp_rename N'[Orders].[IX_Orders_CartInOrderId]', N'IX_Orders_AddressId1', N'INDEX';
GO

EXEC sp_rename N'[MealsInOrder].[RestaurantId]', N'OrderId', N'COLUMN';
GO

EXEC sp_rename N'[Cart].[UserId]', N'ApplicationUserId', N'COLUMN';
GO

EXEC sp_rename N'[Cart].[IX_Cart_UserId]', N'IX_Cart_ApplicationUserId', N'INDEX';
GO

EXEC sp_rename N'[AspNetUsers].[Resturant_Id]', N'RestaurantId', N'COLUMN';
GO

ALTER TABLE [Restaurants] ADD [Category] int NOT NULL DEFAULT 0;
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ApplicationUserId');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [Orders] ALTER COLUMN [ApplicationUserId] nvarchar(450) NULL;
GO

ALTER TABLE [OrderedMeals] ADD [Price] decimal(18,4) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [MealsInOrder] ADD [Price] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NULL;
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FirstName');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [FirstName] nvarchar(max) NULL;
GO

CREATE TABLE [MealsPriceHistories] (
    [Id] int NOT NULL IDENTITY,
    [MealId] int NOT NULL,
    [Price] decimal(18,4) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    CONSTRAINT [PK_MealsPriceHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MealsPriceHistories_Meals_MealId] FOREIGN KEY ([MealId]) REFERENCES [Meals] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_MealsInOrder_OrderId] ON [MealsInOrder] ([OrderId]);
GO

CREATE INDEX [IX_AspNetUsers_RestaurantId] ON [AspNetUsers] ([RestaurantId]);
GO

CREATE INDEX [IX_MealsPriceHistories_MealId] ON [MealsPriceHistories] ([MealId]);
GO

CREATE INDEX [IX_MealsPriceHistories_StartDate] ON [MealsPriceHistories] ([StartDate]);
GO

ALTER TABLE [Cart] ADD CONSTRAINT [FK_Cart_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [MealsInOrder] ADD CONSTRAINT [FK_MealsInOrder_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Addresses_AddressId1] FOREIGN KEY ([AddressId1]) REFERENCES [Addresses] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250219235647_RefactorDataBase', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] ADD [OrderPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250220221649_AddOrderPricePropToOrderModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Addresses_AddressId1];
GO

DROP INDEX [IX_Orders_AddressId1] ON [Orders];
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'AddressId1');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [Orders] DROP COLUMN [AddressId1];
GO

EXEC sp_rename N'[OrderedMeals].[Price]', N'PricePerMeal', N'COLUMN';
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'AddressId');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [Orders] ALTER COLUMN [AddressId] int NOT NULL;
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsInOrder]') AND [c].[name] = N'Price');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [MealsInOrder] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [MealsInOrder] ALTER COLUMN [Price] decimal(18,4) NOT NULL;
GO

CREATE INDEX [IX_Orders_AddressId] ON [Orders] ([AddressId]);
GO

CREATE INDEX [IX_Orders_OrderDate] ON [Orders] ([OrderDate]);
GO

CREATE INDEX [IX_Orders_OrderPrice] ON [Orders] ([OrderPrice]);
GO

CREATE INDEX [IX_Orders_Status] ON [Orders] ([Status]);
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Addresses_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250220235451_ChangePricePropNameToPricePerMealfromOrderedMealsModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_AspNetUsers_ApplicationUserId];
GO

ALTER TABLE [UserComplaints] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Restaurants] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Points] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

DROP INDEX [IX_Orders_ApplicationUserId] ON [Orders];
DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ApplicationUserId');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var43 + '];');
UPDATE [Orders] SET [ApplicationUserId] = N'' WHERE [ApplicationUserId] IS NULL;
ALTER TABLE [Orders] ALTER COLUMN [ApplicationUserId] nvarchar(450) NOT NULL;
ALTER TABLE [Orders] ADD DEFAULT N'' FOR [ApplicationUserId];
CREATE INDEX [IX_Orders_ApplicationUserId] ON [Orders] ([ApplicationUserId]);
GO

ALTER TABLE [Orders] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [MealsPriceHistories] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [MealsInOrder] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Meals] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Deliveries] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUserTokens] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUserRoles] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUserLogins] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUserClaims] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetRoles] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetRoleClaims] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Addresses] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250312222557_AddIsDeletedPropertyToModels', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderedMeals] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Cart] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250313114855_AddIsDeletedPropertyToCartModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Rate');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [Rate] decimal(5,2) NOT NULL;
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'DeliveryFee');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [DeliveryFee] decimal(5,2) NOT NULL;
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'DeliveryDuration');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [DeliveryDuration] decimal(5,2) NOT NULL;
GO

DROP INDEX [IX_Orders_OrderPrice] ON [Orders];
DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'OrderPrice');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [Orders] ALTER COLUMN [OrderPrice] decimal(5,2) NOT NULL;
CREATE INDEX [IX_Orders_OrderPrice] ON [Orders] ([OrderPrice]);
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'PricePerMeal');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [OrderedMeals] ALTER COLUMN [PricePerMeal] decimal(18,2) NOT NULL;
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsPriceHistories]') AND [c].[name] = N'Price');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [MealsPriceHistories] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [MealsPriceHistories] ALTER COLUMN [Price] decimal(5,2) NOT NULL;
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsInOrder]') AND [c].[name] = N'Price');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [MealsInOrder] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [MealsInOrder] ALTER COLUMN [Price] decimal(18,2) NOT NULL;
GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Meals]') AND [c].[name] = N'Price');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [Meals] DROP CONSTRAINT [' + @var51 + '];');
ALTER TABLE [Meals] ALTER COLUMN [Price] decimal(18,2) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250313221637_ChangePriceDecimalFormating', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Orders_OrderPrice] ON [Orders];
GO

EXEC sp_rename N'[Orders].[OrderPrice]', N'TotalTaxPrice', N'COLUMN';
GO

EXEC sp_rename N'[MealsInOrder].[Quantity]', N'MealQuantity', N'COLUMN';
GO

EXEC sp_rename N'[MealsInOrder].[Price]', N'MealPrice', N'COLUMN';
GO

ALTER TABLE [Orders] ADD [TotalMealsPrice] decimal(5,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Orders] ADD [TotalOrderPrice] AS [TotalMealsPrice] + [TotalTaxPrice];
GO

ALTER TABLE [MealsInOrder] ADD [TotalPrice] AS [MealPrice] * [MealQuantity];
GO

CREATE INDEX [IX_Orders_TotalOrderPrice] ON [Orders] ([TotalOrderPrice]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250315213048_AlterOrdersAndMealsInOrderModels', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Addresses_AddressId];
GO

DROP INDEX [IX_Orders_AddressId] ON [Orders];
GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'AddressId');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [Orders] DROP COLUMN [AddressId];
GO

ALTER TABLE [Orders] ADD [UserAddress] nvarchar(max) NOT NULL DEFAULT N'';
GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsInOrder]') AND [c].[name] = N'TotalPrice');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [MealsInOrder] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [MealsInOrder] DROP COLUMN [TotalPrice];
ALTER TABLE [MealsInOrder] ADD [TotalPrice] AS [MealPrice] * [MealQuantity];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250321234141_DropAddressIdColumnInOrderModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [MealsInOrder];
GO

CREATE TABLE [OrderDetails] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [MealId] int NOT NULL,
    [MealPrice] decimal(18,2) NOT NULL,
    [MealQuantity] int NOT NULL,
    [TotalPrice] AS [MealPrice] * [MealQuantity],
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderDetails_Meals_MealId] FOREIGN KEY ([MealId]) REFERENCES [Meals] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_OrderDetails_MealId] ON [OrderDetails] ([MealId]);
GO

CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250327235614_ChangeMealsInOrderModelNameToOrderDetails', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Orders_TotalOrderPrice] ON [Orders];
GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'TotalOrderPrice');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [Orders] DROP COLUMN [TotalOrderPrice];
GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Rate');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [Rate] decimal(7,2) NOT NULL;
GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'TotalMealsPrice');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [Orders] ALTER COLUMN [TotalMealsPrice] decimal(8,2) NOT NULL;
GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MealsPriceHistories]') AND [c].[name] = N'Price');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [MealsPriceHistories] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [MealsPriceHistories] ALTER COLUMN [Price] decimal(8,2) NOT NULL;
GO

ALTER TABLE [Orders] ADD [TotalOrderPrice] AS [TotalMealsPrice] + [TotalTaxPrice];
GO

CREATE INDEX [IX_Orders_TotalOrderPrice] ON [Orders] ([TotalOrderPrice]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250328124531_EditTheDecimalRangeValues', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TempOrders] (
    [Id] nvarchar(450) NOT NULL,
    [CartData] nvarchar(max) NOT NULL,
    [OrderData] nvarchar(max) NOT NULL,
    [Expiry] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_TempOrders] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250402201443_AddTempOrderTable', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Orders_Status] ON [Orders];
DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'Status');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var58 + '];');
ALTER TABLE [Orders] ALTER COLUMN [Status] nvarchar(450) NOT NULL;
CREATE INDEX [IX_Orders_Status] ON [Orders] ([Status]);
GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'Method');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var59 + '];');
ALTER TABLE [Orders] ALTER COLUMN [Method] nvarchar(max) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250403174028_ValueConversionInOrderModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderedMeals]') AND [c].[name] = N'PricePerMeal');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [OrderedMeals] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [OrderedMeals] ALTER COLUMN [PricePerMeal] decimal(8,2) NOT NULL;
GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Meals]') AND [c].[name] = N'Price');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [Meals] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [Meals] ALTER COLUMN [Price] decimal(8,2) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250403221015_ChangeDecimalValuesRange', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'Category');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var62 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [Category] VARCHAR(20) NOT NULL;
GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Restaurants]') AND [c].[name] = N'AcctiveStatus');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [Restaurants] DROP CONSTRAINT [' + @var63 + '];');
ALTER TABLE [Restaurants] ALTER COLUMN [AcctiveStatus] VARCHAR(10) NOT NULL;
GO

DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Meals]') AND [c].[name] = N'Category');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [Meals] DROP CONSTRAINT [' + @var64 + '];');
ALTER TABLE [Meals] ALTER COLUMN [Category] VARCHAR(20) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250403223425_ChangeValueConversion', N'8.0.11');
GO

COMMIT;
GO

