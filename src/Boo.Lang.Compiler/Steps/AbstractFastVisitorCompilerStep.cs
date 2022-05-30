#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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


using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	public class AbstractFastVisitorCompilerStep : FastDepthFirstVisitor, ICompilerStep
	{
		public virtual void Run()
		{
			CompileUnit.Accept(this);
		}

		public virtual void Initialize(CompilerContext context)
		{
			_context = context;
			_codeBuilder = new EnvironmentProvision<BooCodeBuilder>();
			_typeSystemServices = new EnvironmentProvision<TypeSystemServices>();
			_nameResolutionService = new EnvironmentProvision<NameResolutionService>();
		}

		public virtual void Dispose()
		{
			_context = null;
		}

		protected CompilerContext Context
		{
			get { return _context; }
		}

		private CompilerContext _context;

		protected CompilerErrorCollection Errors
		{
			get { return _context.Errors; }
		}

		protected CompilerWarningCollection Warnings
		{
			get { return _context.Warnings; }
		}

		protected CompilerParameters Parameters
		{
			get { return _context.Parameters; }
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return _codeBuilder;  }
		}

		private EnvironmentProvision<BooCodeBuilder> _codeBuilder;

		protected TypeSystemServices TypeSystemServices
		{
			get { return _typeSystemServices; }
		}

		private EnvironmentProvision<TypeSystemServices> _typeSystemServices;

		protected NameResolutionService NameResolutionService
		{
			get { return _nameResolutionService; }
		}

		private EnvironmentProvision<NameResolutionService> _nameResolutionService;

		protected IType GetType(Node node)
		{
			return TypeSystemServices.GetType(node);
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

		protected void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
		}

		protected IType GetEntity(TypeReference node)
		{
			return (IType)TypeSystemServices.GetEntity(node);
		}

		protected IEntity GetEntity(Node node)
		{
			return TypeSystemServices.GetEntity(node);
		}

		protected IMethod GetEntity(Method node)
		{
			return (IMethod)TypeSystemServices.GetEntity(node);
		}

		protected IProperty GetEntity(Property node)
		{
			return (IProperty)TypeSystemServices.GetEntity(node);
		}

		protected virtual IType GetExpressionType(Expression node)
		{
			return TypeSystemServices.GetExpressionType(node);
		}

		protected void BindExpressionType(Expression node, IType type)
		{
			_context.TraceVerbose("{0}: Type of expression '{1}' bound to '{2}'.", node.LexicalInfo, node, type);
			node.ExpressionType = type;
		}

		protected CompileUnit CompileUnit
		{
			get { return _context.CompileUnit; }
		}
	}
}