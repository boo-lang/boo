using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   /// <summary>
   /// Implements transformation step #2 of the LINQ expression reduction process
   /// as described in section 7.16.2.2 (Explicit range variable types)
   /// of the C# Language Standard
   /// </summary>
   public class QueryCastTransformer: QueryTransformerStep
   {      
      override public void OnFromClauseExpression(Boo.Lang.Compiler.Ast.FromClauseExpression node)
      {
         if (node.Identifier.Type != null)
         {
            var typ = node.Identifier.Type;
            var gen = new GenericReferenceExpression(typ.LexicalInfo);
            gen.GenericArguments.Add(typ);
            var inv = new MethodInvocationExpression(node.Container.LexicalInfo, new StringLiteralExpression("Cast"));
            inv.Arguments.Add(node.Container);
            gen.Target = inv;
            node.Container = gen;
         }
         base.Visit(node);
      }

      override public void OnJoinClauseExpression(Boo.Lang.Compiler.Ast.JoinClauseExpression node)
      {
         if (node.Identifier.Type != null)
         {
            var typ = node.Identifier.Type;
            var gen = new GenericReferenceExpression(typ.LexicalInfo);
            gen.GenericArguments.Add(typ);
            var inv = new MethodInvocationExpression(node.Container.LexicalInfo, new StringLiteralExpression("Cast"));
            inv.Arguments.Add(node.Container);
            gen.Target = inv;
            node.Container = gen;
         }
         base.Visit(node);
      }
   }
}
