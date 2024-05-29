using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("rfid_detection")]
    [Index("tag_id", Name = "rfid_detection_tag_id")]
    public partial class rfid_detection
    {
        [Key]
        public Guid uid { get; set; }
        [Required]
        public string tag_id { get; set; }
        [Required]
        public string site_id { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_detection { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_created { get; set; }
    }
}
