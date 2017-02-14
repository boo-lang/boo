using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps.AsyncAwait
{
    /// <summary>
    /// Analyzes method body for labels.
    /// 
    /// Adapted from Microsoft.CodeAnalysis.CSharp.IteratorMethodToStateMachineRewriter.LabelCollector 
    /// in the Roslyn codebase
    /// </summary>
    internal abstract class LabelCollector : FastDepthFirstVisitor
    {
        // transient accumulator.
        protected HashSet<InternalLabel> _currentLabels;

        public override void OnLabelStatement(LabelStatement node)
        {
            if (node != null)
            {
                var cl = _currentLabels;
                if (cl == null)
                {
                    _currentLabels = cl = new HashSet<InternalLabel>();
                }
                cl.Add((InternalLabel) node.Entity);
            }
        }

    }
}
