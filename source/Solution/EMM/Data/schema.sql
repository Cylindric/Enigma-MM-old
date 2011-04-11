CREATE TABLE Config
(
    [Config_ID] int IDENTITY PRIMARY KEY NOT NULL,
    [Key] nvarchar(50) NOT NULL,
    [Value] nvarchar(50) NOT NULL
)
GO

INSERT INTO Config ([Key], [Value]) VALUES ('db_version', '1.0.0')
GO

CREATE TABLE MessageTypes
(
    [Message_Type_ID] int IDENTITY PRIMARY KEY NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Expression] nvarchar(50) NOT NULL
)
GO


CREATE TABLE Ranks
(
    [Rank_ID] int PRIMARY KEY NOT NULL,
    [Name] nvarchar(50) NOT NULL
)
GO


CREATE TABLE Items
(
    [Item_ID] int IDENTITY PRIMARY KEY NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Stack_Size] int NOT NULL DEFAULT 1,
    [Max] int NOT NULL DEFAULT 64,
    [Min_Rank_ID] int NOT NULL DEFAULT 1,

    CONSTRAINT [fk_Rank] FOREIGN KEY (Min_Rank_ID) REFERENCES Ranks (Rank_ID) ON DELETE CASCADE ON UPDATE CASCADE
)
GO


CREATE TABLE Users
(
    [User_ID] int IDENTITY PRIMARY KEY NOT NULL,
    [Rank_ID] int NOT NULL,
    [Username] nvarchar(50) NOT NULL,

    CONSTRAINT [fk_Rank] FOREIGN KEY (Rank_ID) REFERENCES Ranks (Rank_ID) ON DELETE CASCADE ON UPDATE CASCADE
)
GO
