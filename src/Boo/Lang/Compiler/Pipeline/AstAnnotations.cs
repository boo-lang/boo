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

namespace Boo.Lang.Compiler.Pipeline
{
	using Boo.Lang.Ast;
	
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
