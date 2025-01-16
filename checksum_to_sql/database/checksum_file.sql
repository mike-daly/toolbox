CREATE TABLE [dbo].[FileInfo]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[FileHash] [char](64) NOT NULL,
	[FilePath] [nvarchar](512) NOT NULL
)
