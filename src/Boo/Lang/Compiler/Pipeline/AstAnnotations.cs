namespace Boo.Lang.Compiler.Pipeline
{
	using Boo.Lang.Ast;
	
	public class AstAnnotations
	{		
		static object EntryPointKey = new object();
		
		static object ModuleClassKey = new object();
		
		static object AssemblyBuilderKey = new object();
		
		static object AssemblyEntryPointKey = new object();
		
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
		
		public static System.Reflection.MethodInfo GetAssemblyEntryPoint(CompileUnit cu)
		{
			return (System.Reflection.MethodInfo)cu[AssemblyEntryPointKey];
		}
		
		public static void SetAssemblyEntryPoint(CompileUnit node, System.Reflection.MethodInfo method)
		{
			node[AssemblyEntryPointKey] = method;
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
