using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("site")]
    public partial class site
    {
        [Key]
        public string id { get; set; }
        public string location_name { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public decimal? kilometer_marker { get; set; }
        public string created_by { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_created { get; set; }
        public string modified_by { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_modified { get; set; }
    }
}
