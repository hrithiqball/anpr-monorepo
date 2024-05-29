using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("license_plate_recognition")]
    [Index("plate_number", Name = "license_plate_recognition_plate_number")]
    public partial class license_plate_recognition
    {
        [Key]
        public Guid uid { get; set; }
        [Required]
        public string site_id { get; set; }
        public string vehicle_image_path { get; set; }
        public string plate_image_path { get; set; }
        [Required]
        public string plate_number { get; set; }
        public string camera_id { get; set; }
        public int? bbox_top { get; set; }
        public int? bbox_left { get; set; }
        public int? bbox_height { get; set; }
        public int? bbox_width { get; set; }
        public decimal? confidence_lpd { get; set; }
        public decimal? confidence_ocr { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_detection { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_created { get; set; }
    }
}
