SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensorData](
	[SensorDataID] [int] IDENTITY(1,1) NOT NULL,
	[SensorName] [varchar](50) NOT NULL,
	[SensorType] [char](5) NOT NULL,
	[SensorValue] [decimal](8, 3) NOT NULL,
	[EventTime] [datetime] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SensorData] ADD PRIMARY KEY CLUSTERED 
(
	[SensorDataID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO