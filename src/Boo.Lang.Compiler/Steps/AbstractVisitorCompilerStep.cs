#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public abstract class AbstractVisitorCompilerStep : Boo.Lang.Compiler.Ast.DepthFirstVisitor, ICompilerStep
	{
		protected CompilerContext _context;
		
		protected AbstractVisitorCompilerStep()
		{			
		}
		
		protected CompilerContext Context
		{
			get
			{
				return _context;
			}
		}
		
		protected Boo.Lang.Compiler.Ast.CompileUnit CompileUnit
		{
			get
			{
				return _context.CompileUnit;
			}
		}
		
		protected NameResolutionService NameResolutionService
		{
			get
			{
				return _context.NameResolutionService;
			}
		}
		
		protected CompilerParameters Parameters
		{
			get
			{
				return _context.Parameters;
			}
		}
		
		protected CompilerErrorCollection Errors
		{
			get
			{
				return _context.Errors;
			}
		}
		
		protected TypeSystemServices TypeSystemServices
		{
			get
			{
				return _context.TypeSystemServices;
			}
		}
		
		protected void Error(Expression node, CompilerError error)
		{
			Error(node);
			Errors.Add(error);
		}
		
		protected void Error(CompilerError error)
		{
			Errors.Add(error);
		}
		
		protected void Error(Expression node)
		{
			node.ExpressionType = TypeSystemServices.ErrorEntity;
		}

		protected void Bind(Node node, IEntity tag)
		{
			_context.TraceVerbose("{0}: Node '{1}' bound to '{2}'.", node.LexicalInfo, node, tag);
			node.Entity = tag;
		}		
		
		public IEntity GetEntity(Node node)
		{
			return TypeSystemServices.GetEntity(node);
		}
		
		protected void BindExpressionType(Expression node, IType type)
		{
			_context.TraceVerbose("{0}: Type of expression '{1}' bound to '{2}'.", node.LexicalInfo, node, type);  
			node.ExpressionType = type;
		}
		
		protected IType GetConcreteExpressionType(Expression expression)
		{
			return TypeSystemServices.GetConcreteExpressionType(expression);
		}
		
		protected IType GetExpressionType(Expression node)
		{			
			return TypeSystemServices.GetExpressionType(node);
		}
		
		public IType GetType(Node node)
		{
			return TypeSystemServices.GetType(node);
		}		
		
		public LocalVariable GetLocalVariable(Node local)
		{
			return (LocalVariable)GetEntity(local);
		}
		
		protected Boo.Lang.Compiler.Ast.TypeReference CreateTypeReference(IType tag)
		{
			return TypeSystemServices.CreateTypeReference(tag);
		}
		
		protected SelfLiteralExpression CreateSelfReference(IType self)
		{
			SelfLiteralExpression expression = new SelfLiteralExpression();
			BindExpressionType(expression, self);
			return expression;
		}
		
		protected MemberReferenceExpression CreateMemberReference(Expression target, IMember member)
		{
			MemberReferenceExpression reference = new MemberReferenceExpression(target.LexicalInfo);
			reference.Target = target;
			reference.Name = member.Name;
			Bind(reference, member);
			BindExpressionType(reference, member.Type);
			return reference;
		}
		
		protected MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod tag)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(target.LexicalInfo);
			mie.Target = CreateMemberReference(target, tag);			
			BindExpressionType(mie, tag.ReturnType);			
			return mie;			
		}
		
		protected void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
		}
		
		public virtual void Initialize(CompilerContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
		}
		
		public abstract void Run();
		
		public virtual void Dispose()
		{
			_context = null;
		}
	}
}
