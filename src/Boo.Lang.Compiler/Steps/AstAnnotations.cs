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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	
	public class ContextAnnotations
	{		
		static object EntryPointKey = new object();
		
		static object AssemblyBuilderKey = new object();
		
		public static Method GetEntryPoint(CompilerContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			return (Method)context.Properties[EntryPointKey];
		}
		
		public static void SetEntryPoint(CompilerContext context, Method method)
		{
			if (null == method)
			{
				throw new ArgumentNullException("method");
			}
			
			Method current = GetEntryPoint(context);
			if (null != current)
			{
				throw CompilerErrorFactory.MoreThanOneEntryPoint(method);
			}
			context.Properties[EntryPointKey] = method;
		}
		
		public static System.Reflection.Emit.AssemblyBuilder GetAssemblyBuilder(CompilerContext context)
		{
			System.Reflection.Emit.AssemblyBuilder builder = (System.Reflection.Emit.AssemblyBuilder)context.Properties[AssemblyBuilderKey];
			if (null == builder)
			{
				throw CompilerErrorFactory.InvalidAssemblySetUp(context.CompileUnit);
			}
			return builder;
		}
		
		public static void SetAssemblyBuilder(CompilerContext context, System.Reflection.Emit.AssemblyBuilder builder)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			if (null == builder)
			{
				throw new ArgumentNullException("builder");
			}
			context.Properties[AssemblyBuilderKey] = builder;
		}
		
		private ContextAnnotations()
		{
		}
	}
}
