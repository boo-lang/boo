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

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem.Core;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	internal class PartialModuleNamespace : AbstractNamespace
	{
		private InternalModule _module;
		private string _name;

		public PartialModuleNamespace(string name, InternalModule m)
		{
			_name = name;
			_module = m;
		}

		#region Overrides of AbstractNamespace

		public override INamespace ParentNamespace
		{
			get { return _module.ParentNamespace; }
		}

		public override string Name
		{
			get { return _name; }
		}

		public override bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			string moduleNamespace = _module.Namespace;
			string fullyQualifiedName = _name + "." + name;
			if (moduleNamespace.StartsWith(fullyQualifiedName))
			{
				if (moduleNamespace.Length == fullyQualifiedName.Length)
				{
					resultingSet.Add(new NamespaceDelegator(this, _module.ModuleMembersNamespace));
					return true;
				}
				if (moduleNamespace[fullyQualifiedName.Length] == '.')
				{
					resultingSet.Add(new PartialModuleNamespaceMember(this, fullyQualifiedName, name, _module));
					return true;
				}
			}
			return false;
		}

		public override IEnumerable<IEntity> GetMembers()
		{
			yield break;
		}

		#endregion
	}

	internal class PartialModuleNamespaceMember : PartialModuleNamespace
	{
		private readonly PartialModuleNamespace _parent;
		private string _simpleName;

		public PartialModuleNamespaceMember(PartialModuleNamespace parent, string fullyQualifiedName, string simpleName, InternalModule module) : base(fullyQualifiedName, module)
		{
			_parent = parent;
			_simpleName = simpleName;
		}

		public override string Name
		{
			get { return _simpleName; }
		}

		public override INamespace ParentNamespace
		{
			get { return _parent; }
		}
	}
}