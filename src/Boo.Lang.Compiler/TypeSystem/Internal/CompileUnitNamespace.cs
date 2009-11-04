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


using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	internal class CompileUnitNamespace : AbstractNamespace
	{
		private readonly CompileUnit _compileUnit;
		private readonly NameResolutionService _nameResolutionService;
		private readonly InternalTypeSystemProvider _internalTypeSystemProvider;

		public CompileUnitNamespace(CompileUnit unit)
		{
			_nameResolutionService = My<NameResolutionService>.Instance;
			_internalTypeSystemProvider = My<InternalTypeSystemProvider>.Instance;
			_compileUnit = unit;
		}

		public override bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			bool found = false;
			foreach (InternalModule m in InternalModules())
			{
				if (string.IsNullOrEmpty(m.Namespace))
				{
					if (m.Resolve(resultingSet, name, typesToConsider))
						found = true;
					continue;
				}

				if (!HasNamespacePrefix(m, name))
					continue;

				if (m.Namespace.Length == name.Length)
				{
					resultingSet.Add(m.ModuleMembersNamespace);
					found = true;
					continue;
				}

				if (m.Namespace[name.Length] == '.')
				{
					resultingSet.Add(new PartialModuleNamespace(name, m));
					found = true;
					continue;
				}
			}
			return found;
		}

		private static bool HasNamespacePrefix(InternalModule module, string @namespace)
		{
			string moduleNamespace = module.Namespace;
			return moduleNamespace.StartsWith(@namespace);
		}

		private IEnumerable<InternalModule> InternalModules()
		{
			foreach (Module m in _compileUnit.Modules)
			{
				if (m.Entity == null)
					continue;
				yield return (InternalModule) _internalTypeSystemProvider.EntityFor(m);
			}
		}

		#region Overrides of AbstractNamespace

		public override INamespace ParentNamespace
		{
			get { return _nameResolutionService.GlobalNamespace; }
		}

		public override IEnumerable<IEntity> GetMembers()
		{
			yield break;
		}

		#endregion
	}
}