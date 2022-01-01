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
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;
using Module=Boo.Lang.Compiler.Ast.Module;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	public class MacroCompiler
	{
		private static readonly object CachedTypeAnnotation = new object();

		public virtual bool AlreadyCompiled(TypeDefinition node)
		{
			return node.ContainsAnnotation(CachedTypeAnnotation);
		}

		public virtual Type Compile(TypeDefinition node)
		{
			var cached = CachedType(node);
			if (cached != null)
				return cached;

			if (AlreadyCompiled(node))
				return null;

			var compiledType = IsNestedMacro(node) ? CompileNestedMacro(node) : CompileRegularMacro(node);
			CacheType(node, compiledType);
			return compiledType;
		}

		private static bool IsNestedMacro(TypeDefinition node)
		{
			return node.DeclaringType is ClassDefinition;
		}

		private Type CompileNestedMacro(TypeDefinition node)
		{
			Type enclosingType = Compile(node.DeclaringType);
			if (enclosingType == null)
				return null;
			return enclosingType.GetNestedType(node.Name);
		}

		private Type CompileRegularMacro(TypeDefinition node)
		{
			TraceInfo("Compiling macro '{0}'", node);

			var result = Compilation.compile_(CompileUnitFor(node), Context.Parameters.References.ToArray());
			if (result.Errors.Count == 0)
			{
				TraceInfo("Macro '{0}' successfully compiled to '{1}'", node, result.GeneratedAssembly);
				return result.GetGeneratedAssembly().GetType(node.FullName);
			}
			Context.Errors.Extend(result.Errors);
			Context.Warnings.Extend(result.Warnings);
			return null;
		}

		private void TraceInfo(string format, params object[] args)
		{
			Context.TraceInfo(format, args);
		}

		private EnvironmentProvision<CompilerContext> _context = new EnvironmentProvision<CompilerContext>();

		protected CompilerContext Context
		{
			get { return _context; }
		}

		protected CompileUnit CompileUnitFor(TypeDefinition node)
		{
			CompileUnit unit = new CompileUnit();
			GetModuleFor(unit, node);
			return unit;
		}

		private void GetModuleFor(CompileUnit unit, TypeDefinition node)
		{
			unit.Modules.Add(ModuleFor(node));
			CollectModulesForBaseTypes(unit, node);
		}

		private void CollectModulesForBaseTypes(CompileUnit unit, TypeDefinition node)
		{
			foreach (TypeReference baseType in node.BaseTypes)
			{
				InternalClass internalClass = baseType.Entity as InternalClass;
				if (internalClass == null)
					continue;
				GetModuleFor(unit, internalClass.TypeDefinition);
			}
		}

		private static Module ModuleFor(TypeDefinition node)
		{
			var m = new Module
						{
							Namespace = SafeCleanClone(node.EnclosingModule.Namespace),
							Name = node.Name
						};
			foreach (var i in node.EnclosingModule.Imports)
				m.Imports.Add(i.CleanClone());
			m.Members.Add(node.CleanClone());
			return m;
		}

		private static T SafeCleanClone<T>(T node) where T : Node
		{
			return node != null ? (T) node.CleanClone() : null;                 
		}

		protected static void CacheType(TypeDefinition node, Type type)
		{
			node[CachedTypeAnnotation] = type;
		}

		protected static Type CachedType(TypeDefinition node)
		{
			return node[CachedTypeAnnotation] as Type;
		}
	}
}
