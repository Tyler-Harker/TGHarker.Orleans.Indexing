using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.Console
{
    [IndexableState]
    public class TestState
    {
        [IndexableProperty(IsId = true)]
        public TestValueObject Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByImpersonatorId { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByImpersonatorId { get; set; }

        [IndexableProperty()]
        public string IndexedNonValueType { get; set; }
        public string NonIndexedNonValueType { get; set; }
    }
}
