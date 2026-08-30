using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   public class QueryTransformerStep: AbstractTransformerCompilerStep
   {
      protected static FromClauseExpression FromClause(QueryExpression expr)
      {
         return expr.Clauses.First as FromClauseExpression;
      }
      
      protected MethodInvocationExpression MakeTupleConstructor(Declaration l, Declaration r, int size)
      {
         if (size >= 8)
         {
            Context.Errors.Add(new CompilerError(r, "Too many inputs in query expression"));
            Cancel();
         }
         var result = new MethodInvocationExpression(new ReferenceExpression("System.Tuple"));
         if (size > 2)
         {
            for (int i = 1; i < size; ++i)
               result.Arguments.Add(new ReferenceExpression(l.Name + ".Item" + i.ToString()));
         }
         else result.Arguments.Add(new ReferenceExpression(l.Name));
         result.Arguments.Add(new ReferenceExpression(r.Name));
         return result;
      }
      
      protected MethodInvocationExpression MakeTupleConstructor(Declaration l, Expression r, int size)
      {
         if (size >= 8)
         {
            Context.Errors.Add(new CompilerError(r, "Too many inputs in query expression"));
            Cancel();
         }
         var result = new MethodInvocationExpression(new ReferenceExpression("System.Tuple"));
         if (size > 2)
         {
            for (int i = 1; i < size; ++i)
               result.Arguments.Add(new ReferenceExpression(l.Name + ".Item" + i.ToString()));
         }
         else result.Arguments.Add(new ReferenceExpression(l.Name));
         result.Arguments.Add(r);
         return result;
      }
      
      protected int TupleDepth(QueryClauseExpression expr)
      {
         return Math.Min(expr.TupleSize + 1, 2);
      }
      
      protected void InjectTransparentIdentifier(QueryExpression expr, int index, int depth, Declaration l, Declaration r,
                                                 out string name)
      {         
         name = Context.GetUniqueName("TI");
         var injector = new TranparentIdentifierInjector(l, r, name, depth);
         for (int i = index + 1; i < expr.Clauses.Count; ++i)
         {
            var clause = expr.Clauses[i];
            injector.Visit(clause);
            if (clause is QueryClauseExpression)
               ((QueryClauseExpression) clause).TupleSize = ((QueryClauseExpression) clause).TupleSize + 1;
         }
         if (expr.Ending != null)
         {
            injector.Visit(expr.Ending);
            expr.Ending.TupleSize = expr.Ending.TupleSize + 1;
         }
      }

      protected void Simplify(QueryClauseExpression expr, Expression replacement)
      {
         expr.ParentNode.Replace(expr, replacement);
      }
      
      protected static BlockExpression MakeLambda(Declaration L, Expression R)
      {
         var result = new BlockExpression();
         var lExpr = new ReferenceExpression(L.Name);
         result.Parameters.Add(ParameterDeclaration.Lift(lExpr));
         result.Body.Add(new ReturnStatement(R));
         return result;
      }
      
      protected static BlockExpression MakeLambda(Declaration L1, Declaration L2, Expression R)
      {
         var result = new BlockExpression();
         var lExpr = new ReferenceExpression(L1.Name);
         result.Parameters.Add(ParameterDeclaration.Lift(lExpr));
         lExpr = new ReferenceExpression(L2.Name);
         result.Parameters.Add(ParameterDeclaration.Lift(lExpr));
         result.Body.Add(new ReturnStatement(R));
         return result;
      }
      
      protected static MethodInvocationExpression MakeMethodCall(string name, BlockExpression lambda)
      {
         var meth = new MethodInvocationExpression();
         meth.Target = new MemberReferenceExpression(null, name);         
         meth.Arguments.Add(lambda);
         return meth;
      }
      
      protected static MethodInvocationExpression MakeMethodCall(string name, BlockExpression[] lambdas)
      {
         var meth = new MethodInvocationExpression();
         meth.Target = new MemberReferenceExpression(null, name);
         meth.Arguments.AddRange(lambdas);
         return meth;
      }
      
      protected static void InsertFirstArgument(Expression arg, MethodInvocationExpression method)
      {
         var meth = method.Target as MethodInvocationExpression;
         meth.Arguments.Insert(0, arg);
      }
      
      protected void SetSelfArgument(Expression arg, MethodInvocationExpression method)
      {
         var target = method.Target as MemberReferenceExpression;
         if (target == null)
            Context.Errors.Add(new CompilerError("Tried to set Self on an invalid expression. (Should not happen)"));
         else if (target.Target != null)
            Context.Errors.Add(new CompilerError("Tried to set Self on an expression that already had one. (Should not happen)"));
         target.Target = arg;
      }
      
      protected static MethodInvocationExpression MakeMethodCall(string name, Expression collection, BlockExpression[] lambdas)
      {
         var result = MakeMethodCall(name, lambdas);
         InsertFirstArgument(collection, result);
         return result;
      }
      
      protected static bool IsDegenerateQuery(Boo.Lang.Compiler.Ast.QueryExpression node)
      {         
         if (node.Clauses.Count == 1 && node.Cont == null && node.Ending is SelectClauseExpression)
         {
            var ending = node.Ending;
            var id = new ReferenceExpression(FromClause(node).Identifier.Name);
            if (ending.BaseExpr.Matches(id))
            {
               return true;
            }
         }  
         return false;         
      }
   }
}
