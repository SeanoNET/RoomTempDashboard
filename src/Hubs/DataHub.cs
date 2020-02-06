using Microsoft.AspNetCore.SignalR;
using RoomTempDashboard.Models;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using Microsoft.EntityFrameworkCore;

namespace RoomTempDashboard.Hubs
{
    public class DataHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public DataHub(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task GetCurrentValues()
        {
            var sensorData = _context.SensorData.OrderByDescending(m => m.MeasuredAt).First();

            // For testing differences with no device active
            //Random r = new Random();  
            //int range = 3;
            //decimal rTemp = Convert.ToDecimal(r.NextDouble() * range);
            //decimal rHum = Convert.ToDecimal(r.NextDouble() * (range/2));

            //sensorData.Humidity = sensorData.Humidity + rHum;
            //sensorData.Temperature = sensorData.Temperature + rTemp;

            string json = JsonConvert.SerializeObject(sensorData); //@"{""temp"":""24"",""humidity"":""20""}";
            await Clients.All.SendAsync("GetCurrentValues", json);
        }

        public async Task GetLatestReading()
        {
            var sensorData = _context.SensorData.OrderByDescending(m => m.MeasuredAt).First();

            // For testing differences with no device active
            //Random r = new Random();
            //int range = 3;
            //decimal rTemp = Convert.ToDecimal(r.NextDouble() * range);
            //decimal rHum = Convert.ToDecimal(r.NextDouble() * (range / 2));

            //sensorData.Humidity = sensorData.Humidity + rHum;
            //sensorData.Temperature = sensorData.Temperature + rTemp;

            string json = JsonConvert.SerializeObject(sensorData);
            await Clients.All.SendAsync("GetLatestReading", json);
        }

    }
}
