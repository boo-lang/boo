#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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
