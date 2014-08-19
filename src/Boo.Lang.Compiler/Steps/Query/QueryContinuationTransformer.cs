using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   /// <summary>
   /// Implements transformation step #1 of the LINQ expression reduction process
   /// as described in section 7.16.2.1 (Select and groupby clauses with continuations)
   /// of the C# Language Standard
   /// </summary>
   public class QueryContinuationTransformer: QueryTransformerStep
   {      
      override public void OnQueryExpression(QueryExpression node)
      {
         if (node.Cont != null)
         {
            QueryContinuationExpression cont = node.Cont;
            node.Cont = null;
            QueryExpression outer = cont.Body.CloneNode();
            outer.LexicalInfo = node.LexicalInfo;
            var newFrom = new FromClauseExpression(FromClause(node).LexicalInfo);
            newFrom.Container = node;
            outer.Clauses.Insert(0, newFrom);
            ReplaceCurrentNode(outer);
         }
         base.OnQueryExpression(node);         
      }
   }
}
