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
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Core
{
	public static class Namespaces
	{
		public static bool ResolveCoalescingNamespaces(INamespace parent, IEnumerable<INamespace> namespacesToResolveAgainst, string name, EntityType typesToConsider, ICollection<IEntity> resultingSet)
		{
			bool success = false;

			Set<IEntity> resolved = new Set<IEntity>();
			foreach (INamespace root in namespacesToResolveAgainst)
			{
				if (root.Resolve(resolved, name, typesToConsider))
					success = true;
			}

			if (!success)
				return false;

			return CoalesceResolved(resolved, parent, name, resultingSet);
		}

		public static bool ResolveCoalescingNamespaces(INamespace parent, INamespace namespaceToResolveAgainst, string name, EntityType typesToConsider, ICollection<IEntity> resultingSet)
		{
			Set<IEntity> resolved = new Set<IEntity>();
			if (!namespaceToResolveAgainst.Resolve(resolved, name, typesToConsider))
				return false;
			return CoalesceResolved(resolved, parent, name, resultingSet);
		}

		private static bool CoalesceResolved(Set<IEntity> resolved, INamespace parent, string name, ICollection<IEntity> resultingSet)
		{
			List<INamespace> namespaces = new List<INamespace>();
			foreach (IEntity entity in resolved)
			{
				if (entity.EntityType == EntityType.Namespace)
					namespaces.Add((INamespace) entity);
				else
					resultingSet.Add(entity);
			}

			INamespace resolvedNamespace = CoalescedNamespaceFor(parent, name, namespaces);
			if (resolvedNamespace != null)
				resultingSet.Add(resolvedNamespace);

			return true;
		}

		public static INamespace CoalescedNamespaceFor(INamespace parent, string name, List<INamespace> namespaces)
		{
			switch (namespaces.Count)
			{
				case 0:
					return null;
				case 1:
					return Collections.First(namespaces);
				default:
					return new ResolvedNamespaces(name, parent, namespaces.ToArray());
			}
		}
	}
}
