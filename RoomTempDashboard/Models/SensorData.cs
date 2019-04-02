using System;
using System.Collections.Generic;

namespace RoomTempDashboard.Models
{
    public partial class SensorData
    {
        public int Id { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public DateTime? MeasuredAt { get; set; }
    }
}
