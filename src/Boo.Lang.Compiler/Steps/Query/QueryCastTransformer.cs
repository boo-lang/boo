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
      override public void OnFromClauseExpression(FromClauseExpression node)
      {
         if (node.DeclaredType)
         {
            var typ = node.Identifier.Type;
            var gen = new GenericReferenceExpression(typ.LexicalInfo);
            gen.GenericArguments.Add(typ);
            gen.Target = new MemberReferenceExpression(node.Container, "Cast");
            var inv = new MethodInvocationExpression(node.Container.LexicalInfo, gen);
            node.Container = inv;
         }
         base.OnFromClauseExpression(node);
      }

      override public void OnJoinClauseExpression(JoinClauseExpression node)
      {
         if (node.DeclaredType)
         {
            var typ = node.Identifier.Type;
            var gen = new GenericReferenceExpression(typ.LexicalInfo);
            gen.GenericArguments.Add(typ);
            gen.Target = new MemberReferenceExpression(node.Container, "Cast");
            var inv = new MethodInvocationExpression(node.Container.LexicalInfo, gen);
            node.Container = inv;
         }
         base.OnJoinClauseExpression(node);
      }
   }
}
