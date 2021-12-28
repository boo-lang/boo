using System;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class EcmaBuildException : Exception
    {
        public EcmaBuildException(string message) : base(message) { }
    }
}
