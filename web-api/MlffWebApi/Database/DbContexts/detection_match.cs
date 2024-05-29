using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("detection_match")]
    [Index("correctness", Name = "detection_match_correctness")]
    [Index("site_id", Name = "detection_match_site_id")]
    [Index("speed", Name = "detection_match_speed_id")]
    [Index("tag_id", Name = "detection_match_tag_id")]
    [Index("verified", Name = "detection_match_verified")]
    public partial class detection_match
    {
        [Key]
        public Guid uid { get; set; }
        [Required]
        public string site_id { get; set; }
        public string tag_id { get; set; }
        public string plate_number { get; set; }
        public int? speed { get; set; }
        public bool verified { get; set; }
        public bool? correctness { get; set; }
        public string vehicle_image_path { get; set; }
        public string plate_image_path { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_matched { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_created { get; set; }
    }
}
