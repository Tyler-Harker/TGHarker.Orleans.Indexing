using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGHarker.Orleans.Indexing.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexablePropertyAttribute : Attribute
    {
        public bool IsId { get; set; }
        public bool IsSortable { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsFacetable { get; set; }
        public bool IsHidden { get; set; }
    }
}
