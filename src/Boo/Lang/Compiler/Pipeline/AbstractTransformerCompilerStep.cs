#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Reflection;
using Boo.Lang.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;

namespace Boo.Lang.Compiler.Pipeline
{
	public abstract class AbstractTransformerCompilerStep : Boo.Lang.Ast.DepthFirstTransformer, ICompilerStep
	{
		protected CompilerContext _context;
		
		protected AbstractTransformerCompilerStep()
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
		
		public IBinding GetBinding(Node node)
		{
			return BindingManager.GetBinding(node);
		}
		
		public IBinding GetOptionalBinding(Node node)
		{
			return BindingManager.GetOptionalBinding(node);
		}
		
		public ITypeBinding GetBoundType(Node node)
		{
			return BindingManager.GetBoundType(node);
		}

		protected TypeReference CreateBoundTypeReference(ITypeBinding binding)
		{
			return BindingManager.CreateBoundTypeReference(binding);
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
