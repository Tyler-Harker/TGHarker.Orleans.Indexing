using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.Console
{
    public record TestValueObject(string Value) : IValueObject<string>
    {
    }
}
