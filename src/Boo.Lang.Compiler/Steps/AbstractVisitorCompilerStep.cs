#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;

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
			get { return _context; }
		}
		
		protected BooCodeBuilder CodeBuilder
		{
			get { return _context.CodeBuilder; }
		}
		
		protected Boo.Lang.Compiler.Ast.CompileUnit CompileUnit
		{
			get { return _context.CompileUnit; }
		}
		
		protected NameResolutionService NameResolutionService
		{
			get { return _context.NameResolutionService; }
		}
		
		protected CompilerParameters Parameters
		{
			get { return _context.Parameters; }
		}
		
		protected CompilerErrorCollection Errors
		{
			get { return _context.Errors; }
		}
		
		protected CompilerWarningCollection Warnings
		{
			get { return _context.Warnings; }
		}
		
		protected TypeSystemServices TypeSystemServices
		{
			get { return _context.TypeSystemServices; }
		}

		public override void OnQuasiquoteExpression(Boo.Lang.Compiler.Ast.QuasiquoteExpression node)
		{
			// ignore quasi-quotes
		}

		override protected void OnError(Node node, Exception error)
		{
			_context.TraceError("{0}: Internal compiler error on node '{2}': {1}", node.LexicalInfo, error, node);
			base.OnError(node, error);
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

		public IMethod GetEntity(Method node)
		{
			return (IMethod)TypeSystemServices.GetEntity(node);
		}

		public IProperty GetEntity(Property node)
		{
			return (IProperty)TypeSystemServices.GetEntity(node);
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

		protected virtual IType GetExpressionType(Expression node)
		{
			return TypeSystemServices.GetExpressionType(node);
		}
		
		public IType GetType(Node node)
		{
			return TypeSystemServices.GetType(node);
		}
		
		public InternalLocal GetInternalLocal(Node local)
		{
			return (InternalLocal)GetEntity(local);
		}
		
		protected void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
		}
		
		public virtual void Initialize(CompilerContext context)
		{
			if (null == context)
				throw new ArgumentNullException("context");
			_context = context;
		}
		
		public abstract void Run();
		
		public virtual void Dispose()
		{
			_context = null;
		}

		private readonly object VisitedAnnotationKey = new object();

		protected void MarkVisited(Node node)
		{
			node[VisitedAnnotationKey] = VisitedAnnotationKey;
			_context.TraceInfo("{0}: node '{1}' mark visited.", node.LexicalInfo, node);
		}

		protected virtual void EnsureRelatedNodeWasVisited(Node sourceNode, IEntity entity)
		{
			IInternalEntity internalEntity = GetConstructedInternalEntity(entity);
			if (null == internalEntity)
				return;

			Node node = internalEntity.Node;
			if (WasVisited(node))
				return;

			Visit(node);
		}

		protected static IInternalEntity GetConstructedInternalEntity(IEntity entity)
		{
			IConstructedMethodInfo constructedMethod = entity as IConstructedMethodInfo;
			if (null != constructedMethod)
				entity = constructedMethod.GenericDefinition;

			IConstructedTypeInfo constructedType = entity as IConstructedTypeInfo;
			if (null != constructedType)
				entity = constructedType.GenericDefinition;

			return entity as IInternalEntity;
		}

		protected bool WasVisited(Node node)
		{
			return node.ContainsAnnotation(VisitedAnnotationKey);
		}
	}
}
