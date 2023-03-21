using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGHarker.Orleans.Indexing.Core
{
    public interface IValueObject<T>
    {
        T Value { get; }
    }
}
