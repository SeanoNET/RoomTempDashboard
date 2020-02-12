using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomTempDashboard.Models
{
    public partial class SensorData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public decimal Temperature { get; set; }
        [Required]
        public decimal Humidity { get; set; }
        [Required]
        public DateTime MeasuredAt { get; set; }
    }
}
