using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace OctoFramework.Logic.Models.PetaPocos
{
    [TableName("PageViews")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class PageView
    {
        public int Id { get; set; }

        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Url { get; set; }

        public Int64 Count { get; set; }
        public DateTime PublishDate { get; set; }
    }
}