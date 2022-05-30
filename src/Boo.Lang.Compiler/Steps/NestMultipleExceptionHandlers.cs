using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
    public class NestMultipleExceptionHandlers : AbstractFastVisitorCompilerStep
    {
        public override void OnTryStatement(TryStatement node)
        {
            var count = (node.ExceptionHandlers?.Count > 0 ? 1 : 0) +
                (node.EnsureBlock != null ? 1 : 0) +
                (node.FailureBlock != null ? 1 : 0);
            if (count > 1)
            {
                NestTryStatement(node);
            }
            base.OnTryStatement(node);
        }

        private static void NestTryStatement(TryStatement node)
        {
            var firstEHLine = node.ExceptionHandlers?.First?.LexicalInfo?.Line ?? int.MinValue;
            var faultLine = node.FailureBlock?.LexicalInfo?.Line ?? int.MinValue;
            var ensureLine = node.EnsureBlock?.LexicalInfo?.Line ?? int.MinValue;
            if (firstEHLine > faultLine && firstEHLine > ensureLine)
            {
                NestTryStatementInEH(node);
            }
            else if (faultLine > ensureLine && faultLine > firstEHLine)
            {
                NestTryStatementInFault(node);
            }
            else
            {
                NestTryStatementInEnsure(node);
            }
        }

        private static void NestTryStatementInEH(TryStatement node)
        {
            var newTry = new TryStatement(node.LexicalInfo)
            {
                ProtectedBlock = node.ProtectedBlock,
                EnsureBlock = node.EnsureBlock,
                FailureBlock = node.FailureBlock
            };
            if (node.ExceptionHandlers?.Count > 1 && node.ExceptionHandlers.Any(eh => eh.FilterCondition != null))
            {
                DivideExceptionHandlers(node, newTry);
            }
            node.ProtectedBlock = new Block(node.ProtectedBlock.LexicalInfo);
            node.ProtectedBlock.Statements.Add(newTry);
            node.EnsureBlock = null;
            node.FailureBlock = null;
            newTry.InitializeParent(node); ;
            newTry.ProtectedBlock.InitializeParent(newTry);
            newTry.EnsureBlock?.InitializeParent(newTry);
            newTry.FailureBlock?.InitializeParent(newTry);
        }

        private static void DivideExceptionHandlers(TryStatement oldTry, TryStatement newTry)
        {
            newTry.ExceptionHandlers = new(newTry);
            var oldEH = new ExceptionHandlerCollection(oldTry);
            if (oldTry.ExceptionHandlers.First.FilterCondition != null)
            {
                oldEH.AddRange(oldTry.ExceptionHandlers.Skip(1));
                newTry.ExceptionHandlers.Add(oldTry.ExceptionHandlers.First);
            }
            else
            {
                newTry.ExceptionHandlers.AddRange(oldTry.ExceptionHandlers.TakeWhile(e => e.FilterCondition == null));
                oldEH.AddRange(oldTry.ExceptionHandlers.SkipWhile(e => e.FilterCondition == null));
            }
            oldTry.ExceptionHandlers = oldEH;
        }

        private static void NestTryStatementInFault(TryStatement node)
        {
            var newTry = new TryStatement(node.LexicalInfo)
            {
                ProtectedBlock = node.ProtectedBlock,
                EnsureBlock = node.EnsureBlock,
                ExceptionHandlers = node.ExceptionHandlers
            };
            node.ProtectedBlock = new Block(node.ProtectedBlock.LexicalInfo);
            node.ProtectedBlock.Statements.Add(newTry);
            node.EnsureBlock = null;
            node.ExceptionHandlers = null;
            newTry.InitializeParent(node); ;
            newTry.ProtectedBlock.InitializeParent(newTry);
            newTry.EnsureBlock?.InitializeParent(newTry);
            newTry.ExceptionHandlers?.InitializeParent(newTry);
        }

        private static void NestTryStatementInEnsure(TryStatement node)
        {
            var newTry = new TryStatement(node.LexicalInfo)
            {
                ProtectedBlock = node.ProtectedBlock,
                FailureBlock = node.FailureBlock,
                ExceptionHandlers = node.ExceptionHandlers
            };
            node.ProtectedBlock = new Block(node.ProtectedBlock.LexicalInfo);
            node.ProtectedBlock.Statements.Add(newTry);
            node.FailureBlock = null;
            node.ExceptionHandlers = null;
            newTry.InitializeParent(node); ;
            newTry.ProtectedBlock.InitializeParent(newTry);
            newTry.FailureBlock?.InitializeParent(newTry);
            newTry.ExceptionHandlers?.InitializeParent(newTry);
        }
    }
}
