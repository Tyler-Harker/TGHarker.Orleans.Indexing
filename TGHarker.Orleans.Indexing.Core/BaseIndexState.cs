using System;
using System.Collections.Generic;
using System.Text;

namespace TGHarker.Orleans.Indexing.Core
{
    public abstract class BaseIndexState<T>
    where T : new()
    {
        public abstract void Map(T obj);
    }
}
