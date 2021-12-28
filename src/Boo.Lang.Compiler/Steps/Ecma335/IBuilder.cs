using System.Reflection.Metadata;

using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal interface IBuilder
    {
        IEntity Entity { get; }
        EntityHandle Handle { get; }
        void Build();
    }
}
