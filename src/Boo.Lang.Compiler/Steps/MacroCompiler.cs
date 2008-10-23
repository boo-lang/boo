#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
			System.Reflection.Assembly[] references = new System.Reflection.Assembly[Context.Parameters.References.Count];
			(Context.Parameters.References as System.Collections.ICollection).CopyTo(references, 0);
			CompilerContext result = Compilation.compile_(CompileUnitFor(node), references);
			if (0 == result.Errors.Count)
			{
				TraceInfo("Macro '{0}' successfully compiled to '{1}'", node.FullName, result.GeneratedAssembly);
				return result.GeneratedAssembly.GetType(node.FullName);
			}
			ReportErrors(result.Errors);
			ReportWarnings(result.Warnings);
			return null;
		}

		private void TraceInfo(string format, params object[] args)
		{
			Context.TraceInfo(format, args);
		}
		
		private CompileUnit CompileUnitFor(TypeDefinition node)
		{
			CompileUnit unit = new CompileUnit();
			GetModuleFor(unit, node);
			return unit;
		}
		
		private void GetModuleFor(CompileUnit unit, TypeDefinition node)
		{
			unit.Modules.Add(ModuleFor(node));
			foreach (TypeReference typeRef in node.BaseTypes)
			{
				InternalClass ait = null;
				try
				{
					ait = TypeSystemServices.GetType(typeRef) as InternalClass;
				}
				catch (CompilerError)
				{
				}
				if(ait != null)
					GetModuleFor(unit, ait.TypeDefinition);
			}
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
				Errors.Add(e);
			}
		}

		private void ReportWarnings(CompilerWarningCollection warnings)
		{
			foreach (CompilerWarning w in warnings)
			{
				Warnings.Add(w);
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

		public static bool AlreadyCompiled(TypeDefinition node)
		{
			return node.ContainsAnnotation(CachedTypeAnnotation);
		}
	}
}
