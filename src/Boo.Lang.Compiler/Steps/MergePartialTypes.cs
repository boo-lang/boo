#region license
// Copyright (c) 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
	public class MergePartialTypes : AbstractTransformerCompilerStep
	{
		Dictionary<string, TypeDefinition> _partials = new Dictionary<string, TypeDefinition>();
		
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}

		public override void Dispose()
		{
			base.Dispose();

			_partials.Clear();
		}

		public override void OnModule(Module node)
		{
			Visit(node.Members);
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			OnCandidatePartialDefinition(node);
		}

		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			OnCandidatePartialDefinition(node);
		}

		override public void OnEnumDefinition(EnumDefinition node)
		{
			OnCandidatePartialDefinition(node);
		}

		private void OnCandidatePartialDefinition(TypeDefinition node)
		{
			if (!node.IsPartial)
				return;

			var typeName = node.FullName;

			TypeDefinition originalDefinition;
			if (_partials.TryGetValue(typeName, out originalDefinition))
			{
				if (node == originalDefinition) // MergePartialTypes can be executed more than once
					return;

				if (originalDefinition.NodeType != node.NodeType)
				{
					Errors.Add(CompilerErrorFactory.IncompatiblePartialDefinition(node, typeName, AstUtil.TypeKeywordFor(originalDefinition), AstUtil.TypeKeywordFor(node)));
					return;
				}

				MergeImports(node, originalDefinition);
				originalDefinition.Merge(node);
				RemoveCurrentNode();
			}
			else
				_partials[typeName] = node;
		}

		static void MergeImports(TypeDefinition from, TypeDefinition to)
		{
			Module fromModule = from.EnclosingModule;
			Module toModule = to.EnclosingModule;
			if (fromModule == toModule) return;
			if (toModule.ContainsAnnotation(fromModule)) return;
			
			toModule.Imports.ExtendWithClones(fromModule.Imports.Where(i => !toModule.Imports.Any(i2 => i.Matches(i2))));
			// annotate so we remember not to merge the imports
			// again in the future
			toModule.Annotate(fromModule);

			//annotate so that we know these modules have been merged
			//this is used by checkneverusedmembers step
			if (!fromModule.ContainsAnnotation("merged-module"))
				fromModule.Annotate("merged-module");
			if (!toModule.ContainsAnnotation("merged-module"))
				toModule.Annotate("merged-module");
		}
	}
}
