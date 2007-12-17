using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.MetaProgramming;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	class MacroCompiler : AbstractCompilerComponent
	{
		private static readonly object CachedTypeAnnotation = new object();

		public MacroCompiler(CompilerContext context)
		{
			Initialize(context);
		}

		public Type Compile(InternalClass macro)
		{
			return Compile(macro.TypeDefinition);
		}

		private Type Compile(TypeDefinition node)
		{
			Type type = CachedType(node);
			if (type != null) return type;

			if (AlreadyCompiled(node)) return null;

			Type compiledType = RunCompiler(node);
			CacheType(node, compiledType);
			return compiledType;
		}

		private Type RunCompiler(TypeDefinition node)
		{
			TraceInfo("Compiling macro '{0}'", node.FullName);
			CompilerContext result = Compilation.compile_(ModuleFor(node));
			if (0 == result.Errors.Count)
			{
				TraceInfo("Macro '{0}' successfully compiled to '{1}'", node.FullName, result.GeneratedAssembly);
				return result.GeneratedAssembly.GetType(node.FullName);
			}
			ReportErrors(result.Errors);
			return null;
		}

		private void TraceInfo(string format, params object[] args)
		{
			Context.TraceInfo(format, args);
		}

		private Module ModuleFor(TypeDefinition node)
		{
			Module m = new Module();
			m.Namespace = ClearClone(node.EnclosingModule.Namespace);
			m.Name = node.Name;
			foreach (Import i in node.EnclosingModule.Imports)
			{
				m.Imports.Add(ClearClone(i));
			}
			m.Members.Add(ClearClone(node));
			return m;
		}

		private T ClearClone<T>(T node) where T: Node
		{
			if (node == null) return null;
			T clone = (T)node.CloneNode();
			clone.ClearTypeSystemBindings();
			return clone;
		}
		
		private void ReportErrors(CompilerErrorCollection errors)
		{
			foreach (CompilerError e in errors)
			{
				this.Errors.Add(e);
			}
		}

		private static void CacheType(TypeDefinition node, Type type)
		{
			node[CachedTypeAnnotation] = type;
		}

		private static Type CachedType(TypeDefinition node)
		{
			return node[CachedTypeAnnotation] as System.Type;
		}

		private static bool AlreadyCompiled(TypeDefinition node)
		{
			return node.ContainsAnnotation(CachedTypeAnnotation);
		}
	}
}
