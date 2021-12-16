CREATE TABLE [dbo].[MessageThreadParticipants] (
    [MessageThreadId] INT NOT NULL,
    [UserId]   INT NOT NULL,
    [Created] DATETIMEOFFSET NOT NULL, 
    [CreatedBy] NVARCHAR(4000) NOT NULL, 
    [LastModified] DATETIMEOFFSET NOT NULL, 
    [LastModifiedBy] NVARCHAR(4000) NOT NULL, 
    CONSTRAINT [FK_MessageThreadParticipants_MessageThreads] FOREIGN KEY ([MessageThreadId]) REFERENCES [dbo].[MessageThreads] ([Id]),
    CONSTRAINT [FK_ThreadParticipants_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]), 
    CONSTRAINT [PK_MessageThreadParticipants] PRIMARY KEY ([MessageThreadId],[UserId])
);

