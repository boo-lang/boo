using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class SequencePoint
    {
        public int Offset { get; }
        public LexicalInfo Position { get; }

        public SequencePoint(int offset, LexicalInfo position)
        {
            Offset = offset;
            Position = position;
        }
    }
}
