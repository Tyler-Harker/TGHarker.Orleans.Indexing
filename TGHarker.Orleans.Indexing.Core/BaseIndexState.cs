using System;
using System.Collections.Generic;
using System.Text;

namespace TGHarker.Orleans.Indexing.Core
{
    public abstract class BaseIndexState<T> : IBaseIndexState
    where T : new()
    {
        public abstract void Map(T obj);
        public void Map(object o)
        {
            Map((T)o);
        }

        //public abstract Type Type { get; }
    }
}
