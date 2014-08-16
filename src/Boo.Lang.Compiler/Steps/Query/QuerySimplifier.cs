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
         BlockExpression l1 = MakeLambda(_range, e2, TypeReference.Lift(e2));
         if (i == node.Clauses.Count - 1 && node.Ending is SelectClauseExpression)
         {
            var l2 = MakeLambda(_range, fromExpr.Identifier, node.Ending.BaseExpr, TypeReference.Lift(node.Ending.BaseExpr));
            BlockExpression[] lambdas = {l1, l2};
            var sm = MakeMethodCall("SelectMany", lambdas, l2.ReturnType);
            Simplify(fromExpr, sm);
            node.Ending = null;
         } else {
            var constructor = MakeTupleConstructor(_range, fromExpr.Identifier);
            var l2 = MakeLambda(_range, fromExpr.Identifier, constructor, TypeReference.Lift(constructor));
            BlockExpression[] lambdas = {l1, l2};
            var sm = MakeMethodCall("SelectMany", lambdas, l2.ReturnType);
            Simplify(fromExpr, sm);
            string name;
            TypeReference type;
            InjectTransparentIdentifier(node, i, _range, fromExpr.Identifier, out name, out type);
            _range = new Declaration(name, type);
            FromClause(node).Identifier = _range;
         }
      }
      
      private void HandleJoinClause(QueryExpression node, JoinClauseExpression JoinExpr, int i)
      {
         var l1 = MakeLambda(_range, JoinExpr.Left, TypeReference.Lift(JoinExpr.Left));
         var l2 = MakeLambda(JoinExpr.Identifier, JoinExpr.Right, TypeReference.Lift(JoinExpr.Right));
         BlockExpression[] lambdas = {l1, l2, null};
         if (i == node.Clauses.Count - 1 && node.Ending is SelectClauseExpression)
         {
            lambdas[2] = MakeLambda(_range, JoinExpr.Identifier, node.Ending.BaseExpr, TypeReference.Lift(node.Ending.BaseExpr));
            var join = MakeMethodCall("Join", JoinExpr.Container, lambdas, CollectionItemType(JoinExpr.Container));
            Simplify(JoinExpr, join);
            node.Ending = null;
         } else {
            var constructor = MakeTupleConstructor(_range, JoinExpr.Identifier);
            lambdas[2] = MakeLambda(_range, JoinExpr.Identifier, constructor, TypeReference.Lift(constructor));
            var join = MakeMethodCall("Join", JoinExpr.Container, lambdas, CollectionItemType(JoinExpr.Container));
            Simplify(JoinExpr, join);
            string name;
            TypeReference type;
            InjectTransparentIdentifier(node, i, _range, JoinExpr.Identifier, out name, out type);
            _range = new Declaration(name, type);
            FromClause(node).Identifier = _range;            
         }
      }
      
      private void HandleJoinIntoClause(QueryExpression node, JoinClauseExpression JoinExpr, int i)
      {
         var l1 = MakeLambda(_range, JoinExpr.Left, TypeReference.Lift(JoinExpr.Left));
         var l2 = MakeLambda(JoinExpr.Identifier, JoinExpr.Right, TypeReference.Lift(JoinExpr.Right));
         BlockExpression[] lambdas = {l1, l2, null};
         var into = new Declaration(JoinExpr.Into.Name, TypeReference.Lift(JoinExpr.Into));
         if (i == node.Clauses.Count - 1 && node.Ending is SelectClauseExpression)
         {
            lambdas[2] = MakeLambda(_range, into, node.Ending.BaseExpr, TypeReference.Lift(node.Ending.BaseExpr));
            var gJoin = MakeMethodCall("GroupJoin", JoinExpr.Container, lambdas, CollectionItemType(JoinExpr.Container));
            Simplify(JoinExpr, gJoin);
            node.Ending = null;
         } else {
            var constructor = MakeTupleConstructor(_range, JoinExpr.Identifier);
            lambdas[2] = MakeLambda(_range, into, constructor, TypeReference.Lift(constructor));
            var gJoin = MakeMethodCall("GroupJoin", JoinExpr.Container, lambdas, CollectionItemType(JoinExpr.Container));
            Simplify(JoinExpr, gJoin);
            string name;
            TypeReference type;
            InjectTransparentIdentifier(node, i, _range, JoinExpr.Identifier, out name, out type);
            _range = new Declaration(name, type);
            FromClause(node).Identifier = _range;            
         }         
      }

      private void HandleLetClause(QueryExpression query, LetClauseExpression node, int i)
      {
         var constructor = MakeTupleConstructor(_range, node.Value);
         var lambda = MakeLambda(_range, constructor, TypeReference.Lift(constructor));
         var sm = MakeMethodCall("Select", lambda, lambda.ReturnType);
         Simplify(node, sm);
         string name;
         TypeReference type;
         var decl = new Declaration((node.Identifier as ReferenceExpression).Name, TypeReference.Lift(node.Value));
         InjectTransparentIdentifier(query, i, _range, decl, out name, out type);
         _range = new Declaration(name, type);
         FromClause(query).Identifier = _range;
      }

      private void HandleWhereClause(WhereClauseExpression node)
      {
         var parent = node.ParentNode as Expression;
         if (parent == null)
         {
            Context.Errors.Add(new CompilerError(node, "Parent is not an Expression. (Should not happen)"));
            return;
         }
         var lambda = MakeLambda(_range, node.Cond, TypeReference.Lift(Context.CodeBuilder.TypeSystemServices.BoolType.FullName));
         Simplify(node, MakeMethodCall("Where", lambda, _range.Type));
      }
      
      private void HandleOrderbyClause(OrderByClauseExpression node)
      {
         var parent = node.ParentNode as QueryClauseExpression;
         if (parent == null)
         {
            Context.Errors.Add(new CompilerError(node, "Parent is not an Expression. (Should not happen)"));
            return;
         }
         var order = node.Orderings.First;         
         var lambda = MakeLambda(_range, order.BaseExpr, TypeReference.Lift(order));
         string methodName = order.Descending ? "OrderByDescending" : "OrderBy";
         var chain = MakeMethodCall(methodName, lambda, _range.Type);
         for (var i = 1; i < node.Orderings.Count; ++i)
         {
            order = node.Orderings[i];
            lambda = MakeLambda(_range, order.BaseExpr, TypeReference.Lift(order));
            methodName = order.Descending ? "ThenByDescending" : "ThenBy";
            chain = MakeMethodCall(methodName, lambda, _range.Type);            
         }
         Simplify(node, chain);
      }
      
   }
}
