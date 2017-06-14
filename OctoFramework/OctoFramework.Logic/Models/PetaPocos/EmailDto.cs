using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace OctoFramework.Logic.Models.PetaPocos
{
    [TableName("Email")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class EmailDto
    {
        [Column("id")]
        [PrimaryKeyColumn()]
        public int Id { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("fromName")]
        public string FromName { get; set; }

        [Column("fromEmail")]
        public string FromEmail { get; set; }

        [Column("toEmail")]
        public string ToEmail { get; set; }

        [Column("ccEmail")]
        public string CCEmail { get; set; }

        [Column("bccEmail")]
        public string BCCEmail { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("subject")]
        public string Subject { get; set; }

        [Column("message")]
        public string Message { get; set; }
    }
}