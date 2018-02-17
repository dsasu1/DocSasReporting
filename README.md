# DocSasReporting
SqlServer Reporting - Netcore 2.0 mvc

Change connection string in the appsettings.json to your database connection

# SQL Scripts to run on database:

# AdvanceReportViews

   CREATE TABLE [dbo].[AdvanceReportViews](
	[ReportViewId] [int] IDENTITY(1,1) NOT NULL,
	[ViewName] [nvarchar](250) NOT NULL,
	[ViewFriendlyName] [nvarchar](250) NOT NULL,
	[IsValid] [bit] NOT NULL,
  CONSTRAINT [PK_Advance_ReportViews] PRIMARY KEY CLUSTERED 
   (
	[ReportViewId] ASC
   )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
   )

   GO

# AdvanceSavedReports

  CREATE TABLE [dbo].[AdvanceSavedReports](
	[ReportId] [int] IDENTITY(1,1) NOT NULL,
	[ReportName] [nvarchar](250) NOT NULL,
	[ReportDesc] [nvarchar](250) NULL,
	[ReportType] [nvarchar](50) NULL,
	[ReportData] [nvarchar](max) NOT NULL,
 	[SavedDateUTC] [datetime] NOT NULL,
   CONSTRAINT [PK_Advance_SavedReports] PRIMARY KEY CLUSTERED 
   (
	[ReportId] ASC
   )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
   )

  GO

# Store Procedure

 Create PROCEDURE [dbo].[GetTableSchema]
	@p1 NVARCHAR(256) 
 AS
 BEGIN
	SET NOCOUNT ON;

	SELECT 
		COLUMN_NAME,
		DATA_TYPE
	FROM 
		INFORMATION_SCHEMA.COLUMNS
	WHERE 
		TABLE_NAME  = @p1
 END
 GO
