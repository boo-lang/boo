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

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	
	public class AstAnnotations
	{		
		static object EntryPointKey = new object();
		
		static object ModuleClassKey = new object();
		
		static object AssemblyBuilderKey = new object();
		
		public static Method GetEntryPoint(CompileUnit node)
		{
			return (Method)node[EntryPointKey];
		}
		
		public static void SetEntryPoint(CompileUnit node, Method method)
		{
			Method current = (Method)node[EntryPointKey];
			if (null != current)
			{
				throw CompilerErrorFactory.MoreThanOneEntryPoint(method);
			}
			node[EntryPointKey] = method;
		}
		
		public static ClassDefinition GetModuleClass(Module module)
		{
			return (ClassDefinition)module[ModuleClassKey];
		}
		
		public static void SetModuleClass(Module module, ClassDefinition classDefinition)
		{
			module[ModuleClassKey] = classDefinition;
		}
		
		public static System.Reflection.Emit.AssemblyBuilder GetAssemblyBuilder(CompileUnit node)
		{
			System.Reflection.Emit.AssemblyBuilder builder = (System.Reflection.Emit.AssemblyBuilder)node[AssemblyBuilderKey];
			if (null == builder)
			{
				throw CompilerErrorFactory.InvalidAssemblySetUp(node);
			}
			return builder;
		}
		
		public static void SetAssemblyBuilder(CompileUnit node, System.Reflection.Emit.AssemblyBuilder builder)
		{
			node[AssemblyBuilderKey] = builder;
		}
		
		private AstAnnotations()
		{
		}
	}
}
