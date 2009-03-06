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
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	internal sealed class ReflectionNamespaceBuilder
	{
		private Assembly _assembly;
		private ReflectionNamespace _root;

		public ReflectionNamespaceBuilder(IReflectionTypeSystemProvider provider, Assembly assembly)
		{
			_root = new ReflectionNamespace(provider);
			_assembly = assembly;
		}

		public INamespace Build()
		{
			try
			{
				CatalogPublicTypes(_assembly.GetTypes());
			}
			catch (ReflectionTypeLoadException x)
			{
				My<CompilerWarningCollection>.Instance.Add(CompilerWarningFactory.CustomWarning("Could not load types from '" + _assembly + "': " + Builtins.join(x.LoaderExceptions, "\n")));
			}
			return _root;
		}

		private void CatalogPublicTypes(IEnumerable<Type> types)
		{
			string lastNs = "!!not a namespace!!";
			ReflectionNamespace lastNsEntity = null;

			foreach (Type type in types)
			{
				if (!type.IsPublic) continue;

				string ns = type.Namespace ?? string.Empty;
				//retrieve the namespace only if we don't have it handy already
				//usually we'll have it since GetExportedTypes() seems to export
				//types in a sorted fashion.
				if (ns != lastNs)
				{
					lastNs = ns;
					lastNsEntity = GetNamespace(ns);
					lastNsEntity.Add(type);
				}
				else
				{
					lastNsEntity.Add(type);
				}
			}
		}

		public ReflectionNamespace GetNamespace(string ns)
		{
			if (ns.Length == 0)
				return _root;

			string[] namespaceHierarchy = ns.Split('.');
			ReflectionNamespace current = _root;
			foreach (string namespacePart in namespaceHierarchy)
			{
				current = current.Produce(namespacePart);
			}
			return current;
		}
	}
}
