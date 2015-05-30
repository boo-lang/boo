using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   /// <summary>
   /// Implements transformation step #3 of the LINQ expression reduction process
   /// as described in section 7.16.2.3 (Degenerate query expressions)
   /// of the C# Language Standard
   /// </summary>
   public class DegenerateQueryProtector: QueryTransformerStep
   {      
      override public void OnQueryExpression(Boo.Lang.Compiler.Ast.QueryExpression node)
      {        
         if (IsDegenerateQuery(node))
         {
            var lambda = MakeLambda(FromClause(node).Identifier, node.Ending.BaseExpr);
            var select = MakeMethodCall("Select", lambda);
            ReplaceCurrentNode(select);
         }
         base.OnQueryExpression(node);
      }
   }
}
