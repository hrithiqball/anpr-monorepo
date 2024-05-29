using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("watchlist")]
    public partial class watchlist
    {
        [Key]
        public Guid uid { get; set; }
        public int monitor_option { get; set; }
        [Required]
        public string value { get; set; }
        public string created_by { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_created { get; set; }
        public string modified_by { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_modified { get; set; }
        public string remarks { get; set; }
        public string tag_color { get; set; }
    }
}
