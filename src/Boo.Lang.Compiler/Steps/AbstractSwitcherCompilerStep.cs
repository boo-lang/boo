#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;

namespace Boo.Lang.Compiler.Steps
{
	public abstract class AbstractVisitorCompilerStep : Boo.Lang.Compiler.Ast.DepthFirstVisitor, ICompilerStep
	{
		protected CompilerContext _context;
		
		protected AbstractVisitorCompilerStep()
		{			
		}
		
		protected CompilerContext CompilerContext
		{
			get
			{
				return _context;
			}
		}
		
		protected CompileUnit CompileUnit
		{
			get
			{
				return _context.CompileUnit;
			}
		}
		
		protected CompilerParameters CompilerParameters
		{
			get
			{
				return _context.CompilerParameters;
			}
		}
		
		protected CompilerErrorCollection Errors
		{
			get
			{
				return _context.Errors;
			}
		}
		
		protected Bindings.BindingManager BindingManager
		{
			get
			{
				return _context.BindingManager;
			}
		}
		
		protected void Error(Node node, CompilerError error)
		{
			BindingManager.Error(node);
			Errors.Add(error);
		}
		
		protected void Error(CompilerError error)
		{
			Errors.Add(error);
		}
		
		protected void Error(Node node)
		{
			BindingManager.Error(node);
		}

		protected void Bind(Node node, IBinding binding)
		{
			_context.TraceVerbose("{0}: Node '{1}' bound to '{2}'.", node.LexicalInfo, node, binding);
			BindingManager.Bind(node, binding);
		}		
		
		public IBinding GetBinding(Node node)
		{
			return BindingManager.GetBinding(node);
		}
		
		public ITypeBinding GetBoundType(Node node)
		{
			return BindingManager.GetBoundType(node);
		}		
		
		public LocalBinding GetLocalBinding(Node local)
		{
			return (LocalBinding)GetBinding(local);
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
