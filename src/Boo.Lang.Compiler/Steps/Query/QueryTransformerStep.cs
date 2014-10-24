using System;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.Query
{
	public class QueryTransformerStep: AbstractTransformerCompilerStep
	{
		protected static FromClauseExpression FromClause(QueryExpression expr)
		{
			return expr.Clauses.First as FromClauseExpression;
		}
		
		private Module GetCurrentModule(Node n)
		{
			while (!(n is Module))
				n = n.ParentNode;
			return (Module) n;
		}
		
		private string MakeFieldName(string baseName)
		{
			return "_" + char.ToLowerInvariant(baseName[0]) + baseName.Substring(1);
		}
		
		private void AddAnonTypeField(ClassDefinition cd, Declaration r)
		{
			var fieldName = MakeFieldName(r.Name);
			var f = new Field();
			cd.Members.Add(f);
			f.Name = fieldName;
			
			var prop = new Property(r.Name);
			cd.Members.Add(prop);
			prop.Modifiers = TypeMemberModifiers.Public;
			prop.IsSynthetic = true;
			var getter = new Method("get_" + r.Name);
			getter.Modifiers = TypeMemberModifiers.Public;
			getter.IsSynthetic = true;
			getter.Body.Statements.Add(new ReturnStatement(new ReferenceExpression(fieldName)));
			prop.Getter = getter;
		}
		
		private void CreateDerivedAnonType(ClassDefinition cd, TypeReference type, Declaration r)
		{
			cd.BaseTypes.Add(type);
			AddAnonTypeField(cd, r);
			var cons = new Constructor(){Modifiers = TypeMemberModifiers.Public, IsSynthetic = true};
			cons.Parameters.Add(new ParameterDeclaration("parent", type){IsSynthetic = true});
			cons.Parameters.Add(new ParameterDeclaration("newVal", r.Type){IsSynthetic = true});
			cons.Body.Add(new MethodInvocationExpression(new SuperLiteralExpression(), new ReferenceExpression("parent")));
			cons.Body.Add(new Ast.ExpressionStatement(
				new BinaryExpression(BinaryOperatorType.Assign,
											new ReferenceExpression(r.Name),
											new ReferenceExpression("newVal"))));
			cd.Members.Add(cons);
			cons = new Constructor(){Modifiers = TypeMemberModifiers.Public, IsSynthetic = true};
			cons.Parameters.Add(new ParameterDeclaration("other", new SimpleTypeReference(cd.Name)){IsSynthetic = true});
			cons.Body.Add(new MethodInvocationExpression(new SuperLiteralExpression(), new ReferenceExpression("other")));
			cons.Body.Add(new Ast.ExpressionStatement(
				new BinaryExpression(BinaryOperatorType.Assign,
					new ReferenceExpression(r.Name),
					new MemberReferenceExpression(new ReferenceExpression("other"), r.Name))));
			cd.Members.Add(cons);
		}
		
		private void CreateBaseAnonType(ClassDefinition cd, Declaration l, Declaration r)
		{
			AddAnonTypeField(cd, l);
			AddAnonTypeField(cd, r);
			var cons = new Constructor(){Modifiers = TypeMemberModifiers.Public, IsSynthetic = true};
			cons.Parameters.Add(new ParameterDeclaration("l", l.Type){IsSynthetic = true});
			cons.Parameters.Add(new ParameterDeclaration("r", r.Type){IsSynthetic = true});
			cons.Body.Add(new Ast.ExpressionStatement(
				new BinaryExpression(BinaryOperatorType.Assign,
											new ReferenceExpression(MakeFieldName(l.Name)),
											new ReferenceExpression("l"))));
			cons.Body.Add(new Ast.ExpressionStatement(
				new BinaryExpression(BinaryOperatorType.Assign,
											new ReferenceExpression(MakeFieldName(r.Name)),
											new ReferenceExpression("r"))));
			cd.Members.Add(cons);
			cons = new Constructor(){Modifiers = TypeMemberModifiers.Public, IsSynthetic = true};
			cons.Parameters.Add(new ParameterDeclaration("other", new SimpleTypeReference(cd.Name)){IsSynthetic = true});
			cons.Body.Add(new Ast.ExpressionStatement(
				new BinaryExpression(BinaryOperatorType.Assign,
					new ReferenceExpression(MakeFieldName(l.Name)),
					new MemberReferenceExpression(new ReferenceExpression("other"), l.Name))));
			cons.Body.Add(new Ast.ExpressionStatement(
				new BinaryExpression(BinaryOperatorType.Assign,
					new ReferenceExpression(MakeFieldName(r.Name)),
					new MemberReferenceExpression(new ReferenceExpression("other"), r.Name))));
			cd.Members.Add(cons);
		}
		
		private ClassDefinition CreateAnonymousType(Declaration l, Declaration r)
		{
			Module module = GetCurrentModule(l);
			var cd = new ClassDefinition{Name = Context.GetUniqueName("Anon", module.Name), IsSynthetic = true, Modifiers = TypeMemberModifiers.Private};
			if (l.Type != null && l.Type.IsSynthetic && l.Type.Entity != null && l.Type.Entity.Name.StartsWith("$Anon"))
				CreateDerivedAnonType(cd, l.Type, r);
			else CreateBaseAnonType(cd, l, r);
			module.Members.Add(cd);
			return cd;
		}
		
		protected MethodInvocationExpression MakeTransparentConstructor(Declaration l, Declaration r)
		{
			var newType = CreateAnonymousType(l, r);
			return new MethodInvocationExpression(
				new ReferenceExpression(newType.Name),
				new ReferenceExpression(l.Name),
				new ReferenceExpression(r.Name));
		}
		
		protected MethodInvocationExpression MakeTransparentConstructor(Declaration l, Expression r, string name)
		{
			var newType = CreateAnonymousType(l, new Declaration(name, null));
			return new MethodInvocationExpression(
				new ReferenceExpression(newType.Name),
				new ReferenceExpression(l.Name),
				r);
		}
		
		protected void InjectTransparentIdentifier(QueryExpression expr, int index, Declaration l, Declaration r,
																 out string name)
		{			
			name = Context.GetUniqueName("TI");
			var injector = new TranparentIdentifierInjector(l, r, name);
			for (int i = index + 1; i < expr.Clauses.Count; ++i)
			{
				var clause = expr.Clauses[i];
				injector.Visit(clause);
			}
			if (expr.Ending != null)
			{
				injector.Visit(expr.Ending);
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
		
		private static void InsertFirstArgument(Expression arg, MethodInvocationExpression method)
		{
			method.Arguments.Insert(0, arg);
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
