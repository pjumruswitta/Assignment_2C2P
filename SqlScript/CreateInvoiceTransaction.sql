CREATE DATABASE Assignment_2C2P
GO

USE [Assignment_2C2P]
GO

/****** Object:  Table [dbo].[Invoice_transaction]    Script Date: 1/12/2022 11:11:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Invoice_transaction](
	[transaction_id] [varchar](50) NOT NULL,
	[amount] [decimal](16, 2) NOT NULL,
	[currency] [varchar](3) NOT NULL,
	[transaction_date] [datetime] NOT NULL,
	[status] [varchar](20) NOT NULL,
	[input_type] [varchar](10) NOT NULL,
	[created_date] [datetime] NOT NULL,
	[is_active] [bit] NOT NULL,
 CONSTRAINT [PK_Invoice_transaction] PRIMARY KEY CLUSTERED 
(
	[transaction_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


