IF OBJECT_ID(N'dbo.Accounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Accounts (
        Id INT NOT NULL PRIMARY KEY,
        AccountType INT NOT NULL,
        Login NVARCHAR(100) NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        EMail NVARCHAR(255) NOT NULL,
        Phone NVARCHAR(50) NOT NULL,
        Surname NVARCHAR(100) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        MiddleName NVARCHAR(100) NOT NULL,
        Address NVARCHAR(255) NULL
    );
END;

IF OBJECT_ID(N'dbo.Sketches', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sketches (
        Id INT NOT NULL PRIMARY KEY,
        Type INT NOT NULL,
        MasterId INT NULL,
        Image VARBINARY(MAX) NULL,
        Placement INT NOT NULL CONSTRAINT DF_Sketches_Placement DEFAULT (10),
        [Size] INT NOT NULL CONSTRAINT DF_Sketches_Size DEFAULT (4),
        EstimatedHours INT NOT NULL CONSTRAINT DF_Sketches_EstimatedHours DEFAULT (1),
        Complexity INT NOT NULL CONSTRAINT DF_Sketches_Complexity DEFAULT (3)
    );
END;

IF COL_LENGTH(N'dbo.Sketches', N'MasterId') IS NULL
BEGIN
    ALTER TABLE dbo.Sketches
    ADD MasterId INT NULL;
END;

IF COL_LENGTH(N'dbo.Sketches', N'Placement') IS NULL
BEGIN
    ALTER TABLE dbo.Sketches
    ADD Placement INT NOT NULL CONSTRAINT DF_Sketches_Placement_Legacy DEFAULT (10);
END;

IF COL_LENGTH(N'dbo.Sketches', N'Size') IS NULL
BEGIN
    ALTER TABLE dbo.Sketches
    ADD [Size] INT NOT NULL CONSTRAINT DF_Sketches_Size_Legacy DEFAULT (4);
END;

IF COL_LENGTH(N'dbo.Sketches', N'EstimatedHours') IS NULL
BEGIN
    ALTER TABLE dbo.Sketches
    ADD EstimatedHours INT NOT NULL CONSTRAINT DF_Sketches_EstimatedHours_Legacy DEFAULT (1);
END;

IF COL_LENGTH(N'dbo.Sketches', N'Complexity') IS NULL
BEGIN
    ALTER TABLE dbo.Sketches
    ADD Complexity INT NOT NULL CONSTRAINT DF_Sketches_Complexity_Legacy DEFAULT (3);
END;

IF OBJECT_ID(N'dbo.Masters', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Masters (
        Id INT NOT NULL PRIMARY KEY,
        Image VARBINARY(MAX) NULL,
        Surname NVARCHAR(100) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        MiddleName NVARCHAR(100) NOT NULL,
        Type INT NOT NULL,
        Experience INT NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Sketches_Masters')
BEGIN
    ALTER TABLE dbo.Sketches
    ADD CONSTRAINT FK_Sketches_Masters FOREIGN KEY (MasterId) REFERENCES dbo.Masters(Id);
END;

IF OBJECT_ID(N'dbo.Reservations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reservations (
        Id INT NOT NULL PRIMARY KEY,
        UserId INT NOT NULL,
        MasterId INT NOT NULL,
        [Date] DATETIME2 NOT NULL,
        [Time] INT NOT NULL,
        [Status] INT NOT NULL CONSTRAINT DF_Reservations_Status DEFAULT (0),
        CONSTRAINT FK_Reservations_Users FOREIGN KEY (UserId) REFERENCES dbo.Accounts(Id),
        CONSTRAINT FK_Reservations_Masters FOREIGN KEY (MasterId) REFERENCES dbo.Masters(Id)
    );
END;

IF COL_LENGTH(N'dbo.Reservations', N'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Reservations
    ADD [Status] INT NOT NULL CONSTRAINT DF_Reservations_Status_Legacy DEFAULT (0);
END;

IF OBJECT_ID(N'dbo.Feedbacks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Feedbacks (
        Id INT NOT NULL PRIMARY KEY,
        UserId INT NOT NULL,
        MasterId INT NOT NULL,
        Rating INT NOT NULL CONSTRAINT DF_Feedbacks_Rating DEFAULT (5),
        Comment NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Feedbacks_CreatedAt DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_Feedbacks_Users FOREIGN KEY (UserId) REFERENCES dbo.Accounts(Id),
        CONSTRAINT FK_Feedbacks_Masters FOREIGN KEY (MasterId) REFERENCES dbo.Masters(Id)
    );
END;

IF COL_LENGTH(N'dbo.Feedbacks', N'Rating') IS NULL
BEGIN
    ALTER TABLE dbo.Feedbacks
    ADD Rating INT NOT NULL CONSTRAINT DF_Feedbacks_Rating_Legacy DEFAULT (5);
END;

IF COL_LENGTH(N'dbo.Feedbacks', N'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Feedbacks
    ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Feedbacks_CreatedAt_Legacy DEFAULT (SYSDATETIME());
END;

IF OBJECT_ID(N'dbo.Favourites', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Favourites (
        Id INT NOT NULL PRIMARY KEY,
        UserId INT NOT NULL,
        SketchId INT NOT NULL,
        CONSTRAINT FK_Favourites_Users FOREIGN KEY (UserId) REFERENCES dbo.Accounts(Id),
        CONSTRAINT FK_Favourites_Sketches FOREIGN KEY (SketchId) REFERENCES dbo.Sketches(Id)
    );
END;

IF OBJECT_ID(N'dbo.SupportRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupportRequests (
        Id INT NOT NULL PRIMARY KEY,
        UserId INT NOT NULL,
        Subject NVARCHAR(255) NOT NULL,
        Message NVARCHAR(MAX) NOT NULL,
        AdminReply NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL,
        ProcessedAt DATETIME2 NULL,
        IsProcessed BIT NOT NULL CONSTRAINT DF_SupportRequests_IsProcessed DEFAULT (0),
        CONSTRAINT FK_SupportRequests_Users FOREIGN KEY (UserId) REFERENCES dbo.Accounts(Id)
    );
END;

IF OBJECT_ID(N'dbo.InternalNotifications', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InternalNotifications (
        Id INT NOT NULL PRIMARY KEY,
        UserId INT NOT NULL,
        Title NVARCHAR(255) NOT NULL,
        Message NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        IsRead BIT NOT NULL CONSTRAINT DF_InternalNotifications_IsRead DEFAULT (0),
        CONSTRAINT FK_InternalNotifications_Users FOREIGN KEY (UserId) REFERENCES dbo.Accounts(Id)
    );
END;
