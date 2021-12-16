CREATE TABLE [dbo].[Users] (
    [Id]           INT            NOT NULL IDENTITY,
    [EmailAddress] NVARCHAR (254) NULL,
    [PhoneNumber]  NVARCHAR (50)  NULL,
    [Created] DATETIMEOFFSET NOT NULL, 
    [CreatedBy] NVARCHAR(4000) NOT NULL, 
    [LastModified] DATETIMEOFFSET NOT NULL, 
    [LastModifiedBy] NVARCHAR(4000) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

