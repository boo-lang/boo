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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public class AssemblySetupStep : AbstractCompilerStep
	{
		public static AssemblyBuilder GetAssemblyBuilder(CompilerContext context)
		{
			AssemblyBuilder builder = (AssemblyBuilder)context.CompileUnit[AssemblyBuilderKey];
			if (null == builder)
			{
				throw new ApplicationException(Boo.ResourceManager.GetString("InvalidAssemblySetup"));
			}
			return builder;
		}
		
		public static ModuleBuilder GetModuleBuilder(CompilerContext context)
		{
			ModuleBuilder builder = (ModuleBuilder)context.CompileUnit[ModuleBuilderKey];
			if (null == builder)
			{
				throw new ApplicationException(Boo.ResourceManager.GetString("InvalidAssemblySetup"));
			}
			return builder;
		}
		
		string BuildOutputAssemblyName()
		{			
			CompilerParameters parameters = CompilerParameters;
			string fname = parameters.OutputAssembly;
			if (!Path.HasExtension(fname))
			{
				if (CompilerOutputType.Library == parameters.OutputType)
				{
					fname += ".dll";
				}
				else
				{
					fname += ".exe";
			
				}
			}
			return Path.GetFullPath(fname);
		}
		                                                 
		static object AssemblyBuilderKey = new object();
		
		static object ModuleBuilderKey = new object();
		
		public override void Run()
		{
			if (0 == CompilerParameters.OutputAssembly.Length)
			{				
				CompilerParameters.OutputAssembly = CompileUnit.Modules[0].Name;			
			}
			
			CompilerParameters.OutputAssembly = BuildOutputAssemblyName();
			
			AssemblyName asmName = new AssemblyName();
			asmName.Name = GetAssemblyName(CompilerParameters.OutputAssembly);
			
			AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, GetTargetDirectory(CompilerParameters.OutputAssembly));
			ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(asmName.Name, Path.GetFileName(CompilerParameters.OutputAssembly), true);
			
			CompileUnit[AssemblyBuilderKey] = asmBuilder;
			CompileUnit[ModuleBuilderKey] = moduleBuilder;
		}
		
		string GetAssemblyName(string fname)
		{
			return Path.GetFileNameWithoutExtension(fname);
		}
		
		string GetTargetDirectory(string fname)
		{
			return Path.GetDirectoryName(Path.GetFullPath(fname));
		}	

	}
}
