using System;
using System.Linq;
using Boo.Lang.Compiler.Steps.Query;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
   public class QueryTransformer: QueryTransformerStep
   {
      
      private bool TransformQuery(QueryExpression node, QueryTransformerStep transformer)
      {
         transformer.Initialize(this.Context);
         transformer.Visit(node);
         return transformer.Changed;
      }

      private QueryExpression Step1(QueryExpression node)
      {
         var trsf = new QueryContinuationTransformer();
         while (TransformQuery(node, trsf))
         {
            var newNode = trsf.Replacement as QueryExpression;
            if (newNode == null)
            {
               Context.Errors.Add(new CompilerError(node, "Invalid transformation #1! (Should not happen)"));
               return null;
            }
            else
               node = newNode;
            trsf = new QueryContinuationTransformer();
         }
         return node;
      }
      
      private void Step2(QueryExpression node)
      {
         TransformQuery(node, new QueryCastTransformer());
      }
      
      private Node Step3(QueryExpression node)
      {
         var trsf = new DegenerateQueryProtector();
         if (TransformQuery(node, trsf))
         {
            var newNode = trsf.Replacement as QueryExpression;
            if (node == null)
               return trsf.Replacement;
            else
            {
               Context.Errors.Add(new CompilerError(node, "Invalid transformation #3! (Should not happen)"));
               return null;
            }
         }
         return node;
      }

      private void Step4(QueryExpression node)
      {
         TransformQuery(node, new QuerySimplifier());
      }
      
      private void Step5(QueryExpression node)
      {
         TransformQuery(node, new SelectTransformer(FromClause(node).Identifier, node));
      }

      private void Step6(QueryExpression node)
      {
         TransformQuery(node, new GroupByTransformer(FromClause(node).Identifier, node));
      }
      
      private Expression FinalStep(QueryExpression node)
      {
         if (node.Ending != null)
         {
            Context.Errors.Add(new CompilerError(node, "Ending has not been transformed! (Should not happen)"));
            Cancel();
         }
         var head = node.Clauses.Last;
         var tail = node.Clauses.Last as GenericReferenceExpression;
         var clauses = node.Clauses.Skip(1).Reverse().Skip(1);
         foreach (var clause in clauses)
         {
            if (!(clause is GenericReferenceExpression))
            {
               Context.Errors.Add(new CompilerError(clause, "Clause has not been transformed! (Should not happen)"));
               Cancel();
            }
            InsertFirstArgument(clause, tail);
            tail = (GenericReferenceExpression) clause;
         }
         InsertFirstArgument(FromClause(node).Container, tail);
         return head;
      }

      override public void OnQueryExpression(QueryExpression node)
      {
         var newNode = Step1(node);
         if (newNode != node)
         {
            Visit(newNode);
            ReplaceCurrentNode(newNode);
            return;
         }
         
         Step2(node);
         
         var newNode2 = Step3(node);
         if (newNode2 != node)
         {
            ReplaceCurrentNode(newNode2);
            return;
         }
         
         Step4(node);
         
         if (node.Ending is SelectClauseExpression)
            Step5(node);
         else if (node.Ending is GroupClauseExpression)
            Step6(node);
         
         ReplaceCurrentNode(FinalStep(node));
      }
   }
}
