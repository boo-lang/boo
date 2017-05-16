using System;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.Query
{
   /// <summary>
   /// Implements transformation step #4 of the LINQ expression reduction process
   /// as described in section 7.16.2.4 (From, let, where, join and orderby clauses)
   /// of the C# Language Standard
   /// 
   /// According to section 7.16.2.7: "An implementation ... is permitted to use a
   /// different mechanism than anonymous types to group together multiple range variables."
   /// This implementation uses generic Tuples instead, to avoid the need to implement
   /// anonymous types in Boo.
   /// </summary>
   public class QuerySimplifier: QueryTransformerStep
   {
      Declaration _range;
      
      override public void OnQueryExpression(QueryExpression node)
      {
         _range = FromClause(node).Identifier;
         for (var i = 1; i < node.Clauses.Count; ++i)
         {
            var expr = node.Clauses[i];
            if (expr is FromClauseExpression)
               HandleFromClause(node, (FromClauseExpression) expr, i);
            else if (expr is WhereClauseExpression)
               HandleWhereClause((WhereClauseExpression) expr);
            else if (expr is OrderByClauseExpression)
               HandleOrderbyClause((OrderByClauseExpression) expr);
            else if (expr is LetClauseExpression)
               HandleLetClause(node, (LetClauseExpression) expr, i);
            else if (expr is JoinClauseExpression)
            {
               var join = (JoinClauseExpression)expr;
               if (join.Into == null)
                  HandleJoinClause(node, join, i);
               else HandleJoinIntoClause(node, join, i);
            }
            
         }
         //if (node.Clauses.Count = 2 && node.Clauses[1] is FromClauseExpression)
         base.OnQueryExpression(node);
      }
      
      private void HandleFromClause(QueryExpression node, FromClauseExpression fromExpr, int i)
      {
         var e2 = fromExpr.Container;
         BlockExpression l1 = MakeLambda(_range, e2);
         if (i == node.Clauses.Count - 1 && node.Ending is SelectClauseExpression)
         {
            var l2 = MakeLambda(_range, fromExpr.Identifier, node.Ending.BaseExpr);
            BlockExpression[] lambdas = {l1, l2};
            var sm = MakeMethodCall("SelectMany", lambdas);
            Simplify(fromExpr, sm);
            node.Ending = null;
         } else {            
            var constructor = MakeTupleConstructor(_range, fromExpr.Identifier, TupleDepth(fromExpr));
            var l2 = MakeLambda(_range, fromExpr.Identifier, constructor);
            BlockExpression[] lambdas = {l1, l2};
            var sm = MakeMethodCall("SelectMany", lambdas);
            Simplify(fromExpr, sm);
            string name;
            InjectTransparentIdentifier(node, i, TupleDepth(fromExpr), _range, fromExpr.Identifier, out name);
            _range = new Declaration(name, null);
            FromClause(node).Identifier = _range;
         }
      }
      
      private void HandleJoinClause(QueryExpression node, JoinClauseExpression JoinExpr, int i)
      {
         var l1 = MakeLambda(_range, JoinExpr.Left);
         var l2 = MakeLambda(JoinExpr.Identifier, JoinExpr.Right);
         BlockExpression[] lambdas = {l1, l2, null};
         if (i == node.Clauses.Count - 1 && node.Ending is SelectClauseExpression)
         {
            lambdas[2] = MakeLambda(_range, JoinExpr.Identifier, node.Ending.BaseExpr);
            var join = MakeMethodCall("Join", JoinExpr.Container, lambdas);
            Simplify(JoinExpr, join);
            node.Ending = null;
         } else {
            var constructor = MakeTupleConstructor(_range, JoinExpr.Identifier, TupleDepth(JoinExpr));
            lambdas[2] = MakeLambda(_range, JoinExpr.Identifier, constructor);
            var join = MakeMethodCall("Join", JoinExpr.Container, lambdas);
            Simplify(JoinExpr, join);
            string name;
            InjectTransparentIdentifier(node, i, TupleDepth(JoinExpr), _range, JoinExpr.Identifier, out name);
            _range = new Declaration(name, null);
            FromClause(node).Identifier = _range;            
         }
      }
      
      private void HandleJoinIntoClause(QueryExpression node, JoinClauseExpression JoinExpr, int i)
      {
         var l1 = MakeLambda(_range, JoinExpr.Left);
         var l2 = MakeLambda(JoinExpr.Identifier, JoinExpr.Right);
         BlockExpression[] lambdas = {l1, l2, null};
         var into = new Declaration(JoinExpr.Into.Name, TypeReference.Lift(JoinExpr.Into));
         if (i == node.Clauses.Count - 1 && node.Ending is SelectClauseExpression)
         {
            lambdas[2] = MakeLambda(_range, into, node.Ending.BaseExpr);
            var gJoin = MakeMethodCall("GroupJoin", JoinExpr.Container, lambdas);
            Simplify(JoinExpr, gJoin);
            node.Ending = null;
         } else {
            var constructor = MakeTupleConstructor(_range, JoinExpr.Identifier, TupleDepth(JoinExpr));
            lambdas[2] = MakeLambda(_range, into, constructor);
            var gJoin = MakeMethodCall("GroupJoin", JoinExpr.Container, lambdas);
            Simplify(JoinExpr, gJoin);
            string name;
            InjectTransparentIdentifier(node, i, TupleDepth(JoinExpr), _range, JoinExpr.Identifier, out name);
            _range = new Declaration(name, null);
            FromClause(node).Identifier = _range;            
         }         
      }

      private void HandleLetClause(QueryExpression query, LetClauseExpression node, int i)
      {
         var constructor = MakeTupleConstructor(_range, node.Value, TupleDepth(node));
         var lambda = MakeLambda(_range, constructor);
         var sm = MakeMethodCall("Select", lambda);
         Simplify(node, sm);
         string name;
         var decl = new Declaration((node.Identifier as ReferenceExpression).Name, null);
         InjectTransparentIdentifier(query, i, TupleDepth(node), _range, decl, out name);
         _range = new Declaration(name, null);
         FromClause(query).Identifier = _range;
      }

      private void HandleWhereClause(WhereClauseExpression node)
      {
         var lambda = MakeLambda(_range, node.Cond);
         Simplify(node, MakeMethodCall("Where", lambda));
      }
      
      private void HandleOrderbyClause(OrderByClauseExpression node)
      {
         var order = node.Orderings.First;         
         var lambda = MakeLambda(_range, order.BaseExpr);
         string methodName = order.Descending ? "OrderByDescending" : "OrderBy";
         var chain = MakeMethodCall(methodName, lambda);
         for (var i = 1; i < node.Orderings.Count; ++i)
         {
            order = node.Orderings[i];
            lambda = MakeLambda(_range, order.BaseExpr);
            methodName = order.Descending ? "ThenByDescending" : "ThenBy";
            chain = MakeMethodCall(methodName, lambda);            
         }
         Simplify(node, chain);
      }
      
   }
}
