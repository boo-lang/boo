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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler
{
	public abstract class AbstractCompilerComponent : ICompilerComponent
	{
		private CompilerContext _context;

		protected AbstractCompilerComponent()
		{
		}

		protected AbstractCompilerComponent(CompilerContext context)
		{
			if (null == context)
				throw new ArgumentNullException("context");
			_context = context;
		}


		protected CompilerContext Context
		{
			get { return _context; }
		}
		
		protected BooCodeBuilder CodeBuilder
		{
			get { return Context.CodeBuilder; }
		}
		
		protected Boo.Lang.Compiler.Ast.CompileUnit CompileUnit
		{
			get { return Context.CompileUnit; }
		}
		
		protected CompilerParameters Parameters
		{
			get { return Context.Parameters; }
		}
		
		protected System.IO.TextWriter OutputWriter
		{
			get { return Context.Parameters.OutputWriter; }
		}
		
		protected CompilerErrorCollection Errors
		{
			get { return Context.Errors; }
		}
		
		protected CompilerWarningCollection Warnings
		{
			get { return Context.Warnings; }
		}
		
		protected TypeSystem.TypeSystemServices TypeSystemServices
		{
			get { return Context.TypeSystemServices; }
		}
		
		protected NameResolutionService NameResolutionService
		{
			get { return Context.NameResolutionService; }
		}
		
		public IEntity GetEntity(Node node)
		{
			if (null == node.Entity)
				throw CompilerErrorFactory.InvalidNode(node);

			return node.Entity;
		}		
		
		public virtual void Initialize(CompilerContext context)
		{
			if (null == context)
				throw new ArgumentNullException("context");
			_context = context;			
		}
		
		public virtual void Dispose()
		{
			_context = null;
		}	
	}
}
