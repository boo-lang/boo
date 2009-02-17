#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

using Boo.Lang.Compiler.TypeSystem.Core;

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	using System;

	public class InitializeNameResolutionService : AbstractVisitorCompilerStep
	{	
		override public void Run()
		{
			NameResolutionService.GlobalNamespace = My<GlobalNamespace>.Instance;
			EnsureModulesImportEnclosingNamespace();
			ResolveImportAssemblyReferences();
		}

		private void EnsureModulesImportEnclosingNamespace()
		{
			foreach (Module module in CompileUnit.Modules)
			{
				if (module.Namespace == null)
					continue;

				string moduleNamespace = module.Namespace.Name;
				if (module.Imports.Contains(delegate(Import candidate)
				{
					return candidate.Namespace == moduleNamespace;
				}))
					continue;

				module.Imports.Add(new Import(module.Namespace.LexicalInfo, moduleNamespace));
			}
		}

		void ResolveImportAssemblyReferences()
		{
			foreach (Module module in CompileUnit.Modules)
			{
				ImportCollection imports = module.Imports;
				ResolveAssemblyReferences(imports);
			}
		}

		private void ResolveAssemblyReferences(ImportCollection imports)
		{
			Import[] importArray = imports.ToArray();
			for (int i=0; i<importArray.Length; ++i)
			{
				Import current = importArray[i];
				ReferenceExpression reference = current.AssemblyReference;
				if (null == reference)
					continue;
                try
                {
                	reference.Entity = ResolveAssemblyReference(reference);
                }
                catch (Exception x)
                {
                    Errors.Add(CompilerErrorFactory.UnableToLoadAssembly(reference, reference.Name, x));
                    imports.RemoveAt(i);
                }
			}
		}

		private ICompileUnit ResolveAssemblyReference(ReferenceExpression reference)
		{
			ICompileUnit asm = Parameters.FindAssembly(reference.Name);
			if (null == asm)
			{
				asm = Parameters.LoadAssembly(reference.Name);
				if (null != asm)
				{
					Parameters.References.Add(asm);
				}
			}
			return asm;
		}
	}
}
