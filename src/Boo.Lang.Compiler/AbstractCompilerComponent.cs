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
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler
{
	public abstract class AbstractCompilerComponent : ICompilerComponent
	{
		protected CompilerContext _context;		
		
		protected AbstractCompilerComponent()
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
		
		protected TypeSystem.TagService TagService
		{
			get
			{
				return _context.TagService;
			}
		}
		
		protected TypeSystem.NameResolutionService NameResolutionService
		{
			get
			{
				return _context.NameResolutionService;
			}
		}
		
		public IElement GetTag(Node node)
		{
			if (null == node.Tag)
			{
				throw CompilerErrorFactory.NodeNotTagged(node);
			}
			return node.Tag;
		}		
		
		public virtual void Initialize(CompilerContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;			
		}
		
		public virtual void Dispose()
		{
			_context = null;
		}	
	}
}
