using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Ast
{
    public partial class AwaitExpression
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
        public AwaitExpression()
        {
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
        public AwaitExpression(LexicalInfo lexicalInfo)
            : base(lexicalInfo)
        {
        }

        public AwaitExpression(Expression baseExpression)
            : base(baseExpression.LexicalInfo)
        {
            _baseExpression = baseExpression;
            _baseExpression.InitializeParent(this);
        }
    }
}
