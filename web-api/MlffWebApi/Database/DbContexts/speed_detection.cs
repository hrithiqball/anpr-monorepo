using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("speed_detection")]
    [Index("speed_kmh", Name = "speed_detection_speed_kmh")]
    public partial class speed_detection
    {
        [Key]
        public Guid uid { get; set; }
        [Required]
        public string site_id { get; set; }
        public decimal speed_kmh { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_detection { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_created { get; set; }
    }
}
