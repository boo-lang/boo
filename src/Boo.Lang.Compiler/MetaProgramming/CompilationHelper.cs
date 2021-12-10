using System.Linq;
using System.Reflection;

namespace Boo.Lang.Compiler
{
	public static class CompilationHelper
	{
#if NET
		public static MethodInfo GetEntryPoint(this Assembly asm) =>
			asm.DefinedTypes
				.Single(t => t.GetCustomAttribute<EntryPointTypeAttribute>() != null)
				.DeclaredMethods
				.Single(t => t.GetCustomAttribute<EntryPointAttribute>() != null);
#else
		public static MethodInfo GetEntryPoint(this Assembly asm)
		{
			return asm.EntryPoint;
		}
#endif
	}
}
