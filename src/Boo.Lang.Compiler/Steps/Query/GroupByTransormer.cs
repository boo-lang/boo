using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   /// <summary>
   /// Implements transformation step #6 of the LINQ expression reduction process
   /// as described in section 7.16.2.6 (Groupby clauses) of the C# Language Standard
   /// </summary>
   public class GroupByTransformer: QueryTransformerStep
   {
      Declaration _range;
      QueryExpression _query;

      public GroupByTransformer(Declaration range, QueryExpression query)
      {
         _range = range;
         _query = query;
      }
      
      override public void OnGroupClauseExpression(GroupClauseExpression node)
      {
         base.OnGroupClauseExpression(node);
         var v = node.BaseExpr;
         var lambda = MakeLambda(_range, node.Criterion);
         if (v is ReferenceExpression && ((ReferenceExpression)v).Name == _range.Name)
            _query.Clauses.Add(MakeMethodCall("GroupBy", lambda));
         else {
            var lambda2 = MakeLambda(_range, v);
            BlockExpression[] lambdas = {lambda, lambda2};
            _query.Clauses.Add(MakeMethodCall("GroupBy", lambdas));
         }
         _query.Ending = null;
      }
      
   }
}
