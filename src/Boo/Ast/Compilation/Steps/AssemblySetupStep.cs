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
		
		public static string GetOutputAssemblyFileName(CompilerContext context)
		{			
			CompilerParameters parameters = context.CompilerParameters;
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
			return Path.GetFileName(fname);
		}
		                                                 
		static object AssemblyBuilderKey = new object();
		
		static object ModuleBuilderKey = new object();
		
		public override void Run()
		{
			if (0 == CompilerParameters.OutputAssembly.Length)
			{
				//throw new ApplicationException(Boo.ResourceManager.GetString("BooC.NoOutputSpecified"));
				CompilerParameters.OutputAssembly = Path.GetFullPath(CompileUnit.Modules[0].Name);
			}
			
			AssemblyName asmName = new AssemblyName();
			asmName.Name = GetAssemblyName(CompilerParameters.OutputAssembly);
			
			AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, GetTargetDirectory(CompilerParameters.OutputAssembly));
			ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(asmName.Name, GetOutputAssemblyFileName(_context), true);			
			
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
