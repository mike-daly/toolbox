USE [dupe_files]
GO

/****** Object:  Table [dbo].[checksum_file]    Script Date: 2/11/2023 9:45:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[checksum_file]') AND type in (N'U'))
DROP TABLE [dbo].[checksum_file]
GO

/****** Object:  Table [dbo].[checksum_file]    Script Date: 2/11/2023 9:45:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[checksum_file_base](
	[sha1_checksum] [char](40) NOT NULL,
	[file_path] [nvarchar](512) NOT NULL
) ON [PRIMARY]
GO


