using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps.AsyncAwait
{
    public class LocalReferenceCollector : FastDepthFirstVisitor
    {
        private readonly HashSet<InternalLocal> _locals;
        private readonly HashSet<InternalLocal> _results = new HashSet<InternalLocal>();

        public LocalReferenceCollector(IEnumerable<InternalLocal> locals)
        {
            _locals = new HashSet<InternalLocal>(locals);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            CheckReference(node);
        }

        public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            CheckReference(node);
            base.OnMemberReferenceExpression(node);
        }

        private void CheckReference(ReferenceExpression node)
        {
            var local = node.Entity as InternalLocal;
            if (local != null && _locals.Contains(local))
                _results.Add(local);
        }

        public IEnumerable<InternalLocal> Result
        {
            get { return _results; }
        }
    }
}
