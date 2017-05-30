CREATE TABLE [dbo].[Students] (
    [Id]       INT             NOT NULL,
    [Age]      INT             NULL,
    [Name]     NVARCHAR (30)   NULL,
    [JoinDate] DATETIME      NULL,
    [Money]    DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

