# RoomTempDashboard
![Build Docker CLI](https://github.com/SeanoNET/RoomTempDashboard/workflows/Build%20Docker%20CLI/badge.svg)

Web dashboard for displaying temperature and humidity data from [RoomTempDevice-IoT](https://github.com/SeanoNET/RoomTempDevice-IoT) or local stack [RoomTempMQTTConsumer](https://github.com/SeanoNET/RoomTempMQTTConsumer)

![](Docs/dashboard.gif)

![](Docs/timeline.gif)

## Getting Started

Install [.NET Core](https://dotnet.microsoft.com/download) version 3.1 or above

- `git clone https://github.com/SeanoNET/RoomTempDashboard.git`
- `cd RoomTempDashboard/src`
- `dotnet restore && dotnet run`

## Local Stack

See [RoomTempMQTTConsumer](https://github.com/SeanoNET/RoomTempMQTTConsumer) for running the MQTT consumer - this replaces all the cloud services with local alternatives. The local version uses postgres as the db engine due to mssql not supported being supported on arm processors yet. Using postgres will allow you to run this stack on a Raspberry Pi or other arm32 devices.

### Config

Configure the [Postgres](https://www.postgresql.org/) `DataSource` connection string in `appsettings.json`

> Setup and start the [RoomTempMQTTConsumer](https://github.com/SeanoNET/RoomTempMQTTConsumer) first, it will generate the database and tables for you.

```JSON
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Hangfire": "Information"
    }
  },
  "AllowedHosts": "*",
  "DataSource": "Host=localhost;Database=MQTT;Username=postgres;Password=St0ngPassword1!"
}
```

### Running locally in Docker

Install [Docker](https://docs.docker.com/get-docker/)

Build the image:

`docker build -t roomtempdashboard:dev .`

Run the image:

`docker container run -p 5000:5000 -e DataSource="Host=localhost;Database=MQTT;Username=postgres;Password=St0ngPassword1!" -e "ASPNETCORE_URLS: http://0.0.0.0:5000" --name roomtempdash roomtempdashboard:dev`

Install [Docker Compose](https://docs.docker.com/compose/install/) and start the other services with the `docker-compose.yml`.

```
version: '3'

services:

  consumer:
    image: seanonet/roomtempmqttconsumer:latest
    environment:
      DataSource: Host=dbdata;Database=MQTT;Username=postgres;Password=St0ngPassword1!;
      ClientId: metric-consumer
      MqttServerIp: mqtt
      MqttServerPort: 1883
      MqttSubscribeTopic: home/room/temp-mon/data
    depends_on:
        - mqtt
        - dbdata
  mqtt:
    image: eclipse-mosquitto:1.6
    ports:
        - 1883:1883
        - 9001:9001
    volumes:
        - mosquitto:/mosquitto/data
        - mosquitto:/mosquitto/log eclipse-mosquitto
  dbdata:
    image: postgres:12
    ports:
    - 5432:5432
    environment:
        POSTGRES_PASSWORD: St0ngPassword1!
    volumes:
        - dbsql:/var/lib/postgresql/data
  dashboard:
    build: .
    image: roomtempdashboard:dev
    ports:
        - 5000:5000
    environment:
        DataSource: Server=dbdata;Database=MQTT;User Id=sa;Password=St0ngPassword1!;
        ASPNETCORE_URLS: http://0.0.0.0:5000

volumes:
    mosquitto:
    dbsql:
```
run `docker-compose up --build`.

## Cloud Stack
- <img src="Docs/icons/AzureAppService.png" width="25"> [App Service](https://azure.microsoft.com/en-au/services/app-service/)
- <img src="Docs/icons/AzureSQLDatabase.png" width="25"> [SQL Database](https://azure.microsoft.com/en-au/services/sql-database/)
- <img src="Docs/icons/AzureStreamAnalytics.png" width="25"> [Azure Stream Analytics](https://azure.microsoft.com/en-au/services/stream-analytics/)
- <img src="Docs/icons/AzureIoTHub.png" width="25"> [Azure IoT Hub](https://azure.microsoft.com/en-au/services/iot-hub/)

![](Docs/RoomTempDashboard.png)

## Setup MSSQL Entity Framework Provider

Install the [Microsoft.EntityFrameworkCore.SqlServer NuGet package.](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/)

`dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 3.1.1`

Replace `options.UseNpgsql()` with `options.UseSqlServer()` in `ConfigureServices`.  

`Startup.cs`

```
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    });


    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
    services.AddSignalR();
    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetSection("DataSource").Value));
}
```

### Config

Configure the [MSSQL](https://www.microsoft.com/en-us/sql-server/sql-server-2019) `DataSource` connection string in `appsettings.json`.

> You will need to manually create the table and database see [Creating the SQL Database](#Creating-the-SQL-Database)

```JSON
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Hangfire": "Information"
    }
  },
  "AllowedHosts": "*",
  "DataSource": "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
}
```

## Configuring the MXChip and IoT Hub

For configuring the MXChip/IoT Hub and for uploading device code refer to [RoomTempDevice-IoT Getting Started](https://github.com/SeanoNET/RoomTempDevice-IoT#getting-started)

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
    CAST(EVENTPROCESSEDUTCTIME AS datetime) as MeasuredAt
INTO
    optSensorData
FROM
    iptSensorData
```

## Creating the SQL Database

Create a table called `SensorData` - The output from the [Stream Analytics Job](#Creating-the-Stream-Analytics-job)

```SQL
CREATE TABLE [dbo].[SensorData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Temperature] [decimal](18, 13) NULL,
	[Humidity] [decimal](18, 13) NULL,
	[MeasuredAt] [datetime] NULL,
 CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
```

## Icons
[Microsoft Azure, Cloud and Enterprise Symbol / Icon Set](https://www.microsoft.com/en-au/download/details.aspx?id=41937)