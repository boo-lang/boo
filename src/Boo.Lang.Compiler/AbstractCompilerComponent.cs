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
		
		protected System.IO.TextWriter OutputWriter
		{
			get
			{
				return _context.Parameters.OutputWriter;
			}
		}
		
		protected CompilerErrorCollection Errors
		{
			get
			{
				return _context.Errors;
			}
		}
		
		protected TypeSystem.TypeSystemServices TypeSystemServices
		{
			get
			{
				return _context.TypeSystemServices;
			}
		}
		
		protected TypeSystem.NameResolutionService NameResolutionService
		{
			get
			{
				return _context.NameResolutionService;
			}
		}
		
		public IEntity GetEntity(Node node)
		{
			if (null == node.Entity)
			{
				throw CompilerErrorFactory.InvalidNode(node);
			}
			return node.Entity;
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
