CREATE TABLE [dbo].[MessageThreads] (
    [Id] INT NOT NULL IDENTITY,
    [Created] DATETIMEOFFSET NOT NULL, 
    [CreatedBy] NVARCHAR(4000) NOT NULL, 
    [LastModified] DATETIMEOFFSET NOT NULL, 
    [LastModifiedBy] NVARCHAR(4000) NOT NULL,
    CONSTRAINT [PK_MessageThreads] PRIMARY KEY CLUSTERED ([Id] ASC)
);

