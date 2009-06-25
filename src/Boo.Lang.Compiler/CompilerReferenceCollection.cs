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
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;
using Assembly = System.Reflection.Assembly;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Referenced assemblies collection.
	/// </summary>
	public class CompilerReferenceCollection : Set<ICompileUnit>
	{
		private readonly IReflectionTypeSystemProvider _provider;

		public CompilerReferenceCollection(IReflectionTypeSystemProvider provider)
		{
			if (null == provider)
				throw new ArgumentNullException("provider");
			_provider = provider;
		}

		public IReflectionTypeSystemProvider Provider
		{
			get { return _provider; }
		}

		public void Add(Assembly assembly)
		{
			Add(_provider.ForAssembly(assembly));
		}

		public bool Contains(Assembly assembly)
		{
			return Contains(_provider.ForAssembly(assembly));
		}

		public void AddAll(IEnumerable<ICompileUnit> references)
		{
			foreach (ICompileUnit reference in references)
				Add(reference);
		}
		
		[Obsolete("Use AddAll")]
		public void Extend(IEnumerable assemblies)
		{
			foreach (Assembly assembly in assemblies)
				Add(assembly);
		}
		
		public ICompileUnit Find(string simpleName)
		{
			foreach (ICompileUnit reference in this)
				if (reference.Name == simpleName)
					return reference;
			return null;
		}

		public void Remove(Assembly assembly)
		{
			Remove(_provider.ForAssembly(assembly));
		}
	}
}
