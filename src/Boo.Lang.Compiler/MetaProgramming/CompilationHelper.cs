using System.Linq;
using System.Reflection;
#if NET
using System.Reflection.Metadata;
#endif

namespace Boo.Lang.Compiler
{
	public static class CompilationHelper
	{
#if NET
		public static MethodInfo GetEntryPoint(this Assembly asm) =>
			asm.DefinedTypes
				.SingleOrDefault(t => t.GetCustomAttribute<EntryPointTypeAttribute>() != null)
				?.DeclaredMethods
				?.Single(t => t.GetCustomAttribute<EntryPointAttribute>() != null);

		public static Assembly GetGeneratedAssembly(this CompilerContext ctx)
        {
			var serializer = ctx.GeneratedBlobBuilder;
			if (serializer == null)
			{
				var builder = ctx.GeneratedPEBuilder;
				serializer = new BlobBuilder();
				builder.Serialize(serializer);
				ctx.GeneratedBlobBuilder = serializer;
			}
			return Assembly.Load(serializer.ToArray());
		}
#else
		public static MethodInfo GetEntryPoint(this Assembly asm)
		{
			return asm.EntryPoint;
		}

		public static Assembly GetGeneratedAssembly(this CompilerContext ctx) => ctx.GeneratedAssembly;
#endif
	}
}
