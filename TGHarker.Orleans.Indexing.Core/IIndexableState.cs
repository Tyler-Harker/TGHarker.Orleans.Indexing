using System;
using System.Collections.Generic;
using System.Text;

namespace TGHarker.Orleans.Indexing.Core
{
    public interface IIndexableState<TId, TUserId>
    {
        [IndexableProperty(IsId = true)]
        TId? Id { get; set; }
        [IndexableProperty]
        DateTime? CreatedAt { get; set; }
        [IndexableProperty]
        DateTime? UpdatedAt { get; set; }
        [IndexableProperty]
        TUserId? CreatedBy { get; set; }
        [IndexableProperty]
        TUserId? CreatedByImpersonatorId { get; set; }
        [IndexableProperty]
        TUserId? UpdatedBy { get; set; }
        [IndexableProperty]
        TUserId? UpdatedByImpersonatorId { get; set; }
    }
}
