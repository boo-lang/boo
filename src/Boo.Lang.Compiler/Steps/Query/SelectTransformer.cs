using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   /// <summary>
   /// Implements transformation step #5 of the LINQ expression reduction process
   /// as described in section 7.16.2.5 (Select clauses) of the C# Language Standard
   /// </summary>
   public class SelectTransformer: QueryTransformerStep
   {
      Declaration _range;
      QueryExpression _query;

      public SelectTransformer(Declaration range, QueryExpression query)
      {
         _range = range;
         _query = query;
      }
      
      override public void OnSelectClauseExpression(SelectClauseExpression node)
      {
         base.OnSelectClauseExpression(node);
         var v = node.BaseExpr;
         if (v is ReferenceExpression && ((ReferenceExpression)v).Name == _range.Name)
            RemoveCurrentNode();
         else {
            var lambda = MakeLambda(_range, node.BaseExpr);
            _query.Clauses.Add(MakeMethodCall("Select", lambda));
            _query.Ending = null;
         }
      }
      
   }
}
