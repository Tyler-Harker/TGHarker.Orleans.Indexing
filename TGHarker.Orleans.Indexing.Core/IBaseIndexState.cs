using System;
using System.Collections.Generic;
using System.Text;

namespace TGHarker.Orleans.Indexing.Core
{
    public interface IBaseIndexState
    {
        void Map(object o);
        //Type Type { get; }
    }
}
