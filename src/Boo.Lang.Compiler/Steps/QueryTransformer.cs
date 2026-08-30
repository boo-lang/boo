using System;
using System.Linq;
using Boo.Lang.Compiler.Steps.Query;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
   public class QueryTransformer: QueryTransformerStep
   {
      
      private Node TransformQuery(QueryExpression node, QueryTransformerStep transformer)
      {
         transformer.Initialize(this.Context);
         return transformer.Visit(node);
      }

      private QueryExpression Step1(QueryExpression node)
      {
         var trsf = new QueryContinuationTransformer();
         while (true)
         {            
            var newNode = TransformQuery(node, trsf);
            if (newNode == node)
               break;
            else if (newNode is QueryExpression)               
               node = (QueryExpression)newNode;
            else
            {
               Context.Errors.Add(new CompilerError(node, "Invalid transformation #1! (Should not happen)"));
               Cancel();
               return null;
            }
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
         while (true)
         {
            var newNode =  TransformQuery(node, trsf);
            if (newNode == node)
               break;
            else if (newNode is QueryExpression)
            {
               Context.Errors.Add(new CompilerError(node, "Invalid transformation #3! (Should not happen)"));
               Cancel();
               return null;
            }
            else return newNode;
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
         var tail = node.Clauses.Last as MethodInvocationExpression;
         var clauses = node.Clauses.Skip(1).Reverse().Skip(1);
         foreach (var clause in clauses)
         {
            if (!(clause is MethodInvocationExpression))
            {
               Context.Errors.Add(new CompilerError(clause, "Clause has not been transformed! (Should not happen)"));
               Cancel();
            }
            SetSelfArgument(clause, tail);
            tail = (MethodInvocationExpression) clause;
         }
         SetSelfArgument(FromClause(node).Container, tail);
         return head;
      }
      
      private Method GetMethod(Node node)
      {
         while (!(node is Method))
            node = node.ParentNode;
         return (Method)node;
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
            node.Clauses.Add(newNode2 as Expression);
            node.Ending = null;
         } else {
            
            Step4(node);
            
            if (node.Ending is SelectClauseExpression)
               Step5(node);
            else if (node.Ending is GroupClauseExpression)
               Step6(node);
         }
         var result = FinalStep(node);
         ReplaceCurrentNode(result);
      }
   }
}
