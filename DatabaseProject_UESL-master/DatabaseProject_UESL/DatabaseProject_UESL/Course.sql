CREATE TABLE [dbo].[Course]
(
	[ClassRecordID] INT IDENTITY (1, 1) NOT NULL,
    [CatalogNumber] NVARCHAR (50) NULL,
    [ClassName] NVARCHAR (50) NULL,
    [SectionNumber] NCHAR(10) NULL,
	[Instructor] NVARCHAR (50) NULL,
	[MeetTime] DATETIME NULL,
    [room] NVARCHAR(50) NULL, 
    [course Name] NVARCHAR(50) NULL, 
    PRIMARY KEY CLUSTERED ([ClassRecordID] ASC)
)