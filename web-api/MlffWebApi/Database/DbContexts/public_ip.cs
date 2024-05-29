using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MlffWebApi.Database.DbContexts
{
    [Table("public_ip")]
    public partial class public_ip
    {
        [Key]
        public Guid uid { get; set; }
        [Required]
        public string ip_address { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime date_update { get; set; }
        public string site_id { get; set; }
    }
}
