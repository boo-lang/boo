using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   public class QueryTransformerStep: AbstractTransformerCompilerStep
   {
      private bool _changed;
      protected bool _recursing;
      
      public bool Changed
      {
         get
         {
            return _changed;
         }
      }
      
      public Node Replacement
      {
         get 
         {
            return _resultingNode;
         }
      }
      
      protected static FromClauseExpression FromClause(QueryExpression expr)
      {
         return expr.Clauses.First as FromClauseExpression;
      }
      
      private GenericTypeReference MakeTupleType(Expression expr, Declaration l)
      {
         GenericTypeReference tuple;
         if (l.Type is GenericTypeReference && ((GenericTypeReference)l.Type).Name.Equals("System.Tuple"))
         {
            tuple = (GenericTypeReference)l.Type;
            if (tuple.GenericArguments.Count == 8)
            {
               Context.Errors.Add(new CompilerError(expr, "Too many inputs in query expression"));
               Cancel();
            }
            tuple = tuple.CloneNode();
         } else {
            tuple = new GenericTypeReference("System.Tuple", l.Type);
         }
         return tuple;
      }
      
      protected GenericTypeReference MakeTupleType(Expression expr, Declaration l, Declaration r)
      {
         GenericTypeReference tuple = MakeTupleType(expr, l);
         tuple.GenericArguments.Add(r.Type);
         return tuple;         
      }
      
      protected GenericTypeReference MakeTupleType(Expression expr, Declaration l, Expression r)
      {
         GenericTypeReference tuple = MakeTupleType(expr, l);
         tuple.GenericArguments.Add(TypeReference.Lift(r));
         return tuple;         
      }
      
      protected MethodInvocationExpression MakeTupleConstructor(Declaration l, Declaration r)
      {
         var tuple = MakeTupleType(r.ParentNode as Expression, l, r);
         var result = new MethodInvocationExpression(new ReferenceExpression(tuple.Name));
         if (tuple.GenericArguments.Count > 2)
         {
            var oldTuple = l.Type as GenericTypeReference;
            for (var i = 0; i < oldTuple.GenericArguments.Count; ++i)
               result.Arguments.Add(new ReferenceExpression(l.Name + ".Item" + i.ToString()));
         }
         else result.Arguments.Add(new ReferenceExpression(l.Name));
         result.Arguments.Add(new ReferenceExpression(r.Name));
         return result;
      }
      
      protected MethodInvocationExpression MakeTupleConstructor(Declaration l, Expression r)
      {
         var tuple = MakeTupleType(r as Expression, l, r);
         var result = new MethodInvocationExpression(new ReferenceExpression(tuple.Name));
         if (tuple.GenericArguments.Count > 2)
         {
            var oldTuple = l.Type as GenericTypeReference;
            for (var i = 0; i < oldTuple.GenericArguments.Count; ++i)
               result.Arguments.Add(new ReferenceExpression(l.Name + ".Item" + i.ToString()));
         }
         else result.Arguments.Add(new ReferenceExpression(l.Name));
         result.Arguments.Add(r);
         return result;
      }
      
      protected void InjectTransparentIdentifier(QueryExpression expr, int index, Declaration l, Declaration r,
                                                 out string name, out TypeReference type)
      {         
         type = MakeTupleType(expr.Clauses[index], l, r);
         name = Context.GetUniqueName("TI");
         var injector = new TranparentIdentifierInjector(l, r, name, expr.Clauses.Count);
         for (int i = index + 1; i < expr.Clauses.Count; ++i)
            injector.Visit(expr.Clauses[i]);
         if (expr.Ending != null)
            injector.Visit(expr.Ending);
      }

      protected void Simplify(QueryClauseExpression expr, Expression replacement)
      {
         expr.ParentNode.Replace(expr, replacement);
      }
      
      override protected void ReplaceCurrentNode(Node replacement)
      {
         base.ReplaceCurrentNode(replacement);
         if (! _recursing)
            _changed = true;
      }
      
      protected static BlockExpression MakeLambda(Declaration L, Expression R, TypeReference returnType)
      {
         var result = new BlockExpression();
         var lExpr = new ReferenceExpression(L.Name);
         result.Parameters.Add(ParameterDeclaration.Lift(lExpr));
         result.Body.Add(new ReturnStatement(R));
         result.ReturnType = returnType;
         return result;
      }
      
      protected static BlockExpression MakeLambda(Declaration L1, Declaration L2, Expression R, TypeReference returnType)
      {
         var result = new BlockExpression();
         var lExpr = new ReferenceExpression(L1.Name);
         result.Parameters.Add(ParameterDeclaration.Lift(lExpr));
         lExpr = new ReferenceExpression(L2.Name);
         result.Parameters.Add(ParameterDeclaration.Lift(lExpr));
         result.Body.Add(new ReturnStatement(R));
         result.ReturnType = returnType;
         return result;
      }
      
      protected static GenericReferenceExpression MakeMethodCall(string name, BlockExpression lambda, TypeReference genType)
      {
         var result = new GenericReferenceExpression(); 
         result.GenericArguments.Add(genType);
         result.GenericArguments.Add(lambda.ReturnType);
         var meth = new MethodInvocationExpression();
         meth.Target = new ReferenceExpression(name);
         meth.Arguments.Add(lambda);
         result.Target = meth;
         return result;
      }
      
      protected static GenericReferenceExpression MakeMethodCall(string name, BlockExpression[] lambdas, TypeReference genType)
      {
         var result = new GenericReferenceExpression();
         if (genType != null)
            result.GenericArguments.Add(genType);
         foreach (var lambda in lambdas)
            result.GenericArguments.Add(lambda.ReturnType);
         var meth = new MethodInvocationExpression();
         meth.Target = new ReferenceExpression(name);
         meth.Arguments.AddRange(lambdas);
         result.Target = meth;
         return result;
      }
      
      protected static void InsertFirstArgument(Expression arg, GenericReferenceExpression method)
      {
         var meth = method.Target as MethodInvocationExpression;
         meth.Arguments.Insert(0, arg);
      }
      
      protected static GenericReferenceExpression MakeMethodCall(string name, Expression collection, BlockExpression[] lambdas, TypeReference genType)
      {
         var result = MakeMethodCall(name, lambdas, genType);
         InsertFirstArgument(collection, result);
         return result;
      }
      
      protected TypeReference CollectionItemType(Expression collection)
      {
         Cancel(); //figure this out
         return null;
      }
      
      protected static bool IsDegenerateQuery(Boo.Lang.Compiler.Ast.QueryExpression node)
      {         
         if (node.Clauses == null && node.Cont == null && node.Ending is SelectClauseExpression)
         {
            var ending = node.Ending;
            var id = Expression.Lift(FromClause(node).Identifier.Name);
            if (ending.BaseExpr.Matches(id))
            {
               return true;
            }
         }  
         return false;         
      }
   }
}
