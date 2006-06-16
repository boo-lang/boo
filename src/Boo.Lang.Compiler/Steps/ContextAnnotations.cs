using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
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