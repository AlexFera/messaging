CREATE TABLE [dbo].[Messages] (
    [Id]                   INT           NOT NULL IDENTITY,
    [MessageThreadId]      INT           NOT NULL,
    [SendingUserId]        INT           NOT NULL,
    [ToAddress] NVARCHAR(250) NOT NULL, 
    [FromAddress] NVARCHAR(250) NOT NULL, 
    [Content]              NVARCHAR(4000) NOT NULL,
    [ReadDate]             DATETIMEOFFSET NULL,
    [ResponseReceivedDate] DATETIMEOFFSET NULL,
    [CompletedDate]        DATETIMEOFFSET NULL,
    [Created]              DATETIMEOFFSET NOT NULL,
    [CreatedBy] NVARCHAR(4000) NOT NULL, 
    [LastModified] DATETIMEOFFSET NOT NULL, 
    [LastModifiedBy] NVARCHAR(4000) NOT NULL, 
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Messages_Users] FOREIGN KEY ([SendingUserId]) REFERENCES [dbo].[Users] ([Id]), 
    CONSTRAINT [FK_Messages_MessageThreads] FOREIGN KEY ([MessageThreadId]) REFERENCES [MessageThreads]([Id])
);


GO


CREATE INDEX [IX_Messages_CreatedBy] ON [dbo].[Messages] ([CreatedBy])
