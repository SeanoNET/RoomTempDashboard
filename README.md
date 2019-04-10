# RoomTempDashboard
Web dashboard for displaying temperature and humidity data from [RoomTempDevice-IoT](https://github.com/SeanoNET/RoomTempDevice-IoT)

### Dashboard
![](Docs/dashboard.gif)

### Timeline
![](Docs/timeline.gif)

## Workflow

![](Docs/RoomTempDashboard.png)

## Getting Started

Install [.NET Core](https://dotnet.microsoft.com/download) version 2.2 or above

- `git clone https://github.com/SeanoNET/RoomTempDashboard.git`
- `cd RoomTempDashboard/RoomTempDashboard`
- `dotnet restore && dotnet run`

### Config
Add `HelloIotDatabase` connection string to your `appsettings.json` see [Creating the SQL Database](#Creating-the-SQL-Database)

```JSON
"ConnectionStrings": {
    "HelloIotDatabase": "<ConnectionString>"
}
```
## Configuring the MXChip and IoT Hub

For configuring the MXChip/IoT Hub and for uploading device code see [RoomTempDevice-IoT Getting Started](https://github.com/SeanoNET/RoomTempDevice-IoT#getting-started)

## Stream Analytics

The [Stream Analytics](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-introduction) job will process the events from the [payload](https://github.com/SeanoNET/RoomTempDevice-IoT#payload) sent to [IoT Hub](https://azure.microsoft.com/en-au/services/iot-hub/) and output the data into the [MSSQL data source](#Creating-the-SQL-Database) so the RoomTempDashboard can retrieve the data.

### Creating the Stream Analytics job <a name="Creating-the-Stream-Analytics-job"></a>

- [Create a Stream Analytics job](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-quick-create-portal#create-a-stream-analytics-job)
- [Configure job input](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-quick-create-portal#configure-job-input) name it `iptSensorData`
- [Configure job output](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-quick-create-portal#configure-job-output) name it `optSensorData` *Select SQL Database instead of Blob storage and select `SensorData` table*


### [Transformation Query](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-quick-create-portal#define-the-transformation-query)
```SQL
SELECT 
    CAST(TEMPERATURE AS float) AS Temperature,
    CAST(HUMIDITY AS float) AS Humidity,
    CAST(EVENTPROCESSEDUTCTIME AS datetime) as Measured_At
INTO
    optSensorData
FROM
    iptSensorData
```

## Creating the SQL Database <a name="Creating-the-SQL-Database"></a>

Create a table called `SensorData` - The output from the [Stream Analytics Job](#Creating-the-Stream-Analytics-job)

```SQL
CREATE TABLE [dbo].[SensorData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Temperature] [decimal](18, 13) NULL,
	[Humidity] [decimal](18, 13) NULL,
	[Measured_At] [datetime] NULL,
 CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
```