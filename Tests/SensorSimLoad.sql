DECLARE @MAX_TEMP INT 
DECLARE @MIN_TEMP INT 
DECLARE @MAX_HUMID INT 
DECLARE @MIN_HUMID INT 

--A script to simulate sensor data when developing the dashboard
SET @MAX_TEMP = 30--Max random temp 
SET @MIN_TEMP = 26--Min random temp 
SET @MAX_HUMID = 48 --Max random humidity 
SET @MIN_HUMID = 53--Min random humidity 
WHILE 1 = 1 
  BEGIN 
      INSERT INTO [dbo].[SensorData] 
                  ([temperature], 
                   [humidity], 
                   [measured_at]) 
      VALUES      ( CONVERT(DECIMAL(13, 4), 
					@MIN_TEMP + (@MAX_TEMP - @MIN_TEMP) * Rand(Checksum(Newid()))), 
                    CONVERT(DECIMAL(13, 4), 
                    @MIN_HUMID + (@MAX_HUMID - @MIN_HUMID) * Rand(Checksum(Newid()))),
					Getdate()) 

      WAITFOR delay '00:00:05' 
  END 