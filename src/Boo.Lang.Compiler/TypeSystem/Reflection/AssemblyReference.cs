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

using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	using System;

	class AssemblyReference : ICompileUnit, IEquatable<AssemblyReference>
	{
		private readonly System.Reflection.Assembly _assembly;
		private readonly IReflectionTypeSystemProvider _provider;
		private INamespace _rootNamespace;

		internal AssemblyReference(IReflectionTypeSystemProvider provider, System.Reflection.Assembly assembly)
		{
			if (null == assembly)
				throw new System.ArgumentNullException("assembly");
			_provider = provider;
			_assembly = assembly;
		}
		
		public string Name
		{
			get
			{
				if (null == _name)
					_name = new AssemblyName(_assembly.FullName).Name;
				return _name;
			}
		}
		string _name;

		public string FullName
		{
			get { return _assembly.FullName; }
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Assembly; }
		}
		
		public System.Reflection.Assembly Assembly
		{
			get { return _assembly; }
		}

		#region Implementation of ICompileUnit

		public INamespace RootNamespace
		{
			get
			{
				if (_rootNamespace != null)
					return _rootNamespace;
				return _rootNamespace = CreateRootNamespace();
			}
		}

		private INamespace CreateRootNamespace()
		{
			return new ReflectionNamespaceBuilder(_provider, _assembly).Build();
		}

		#endregion

		public override bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			AssemblyReference aref = other as AssemblyReference;
			return Equals(aref);
		}
	
		public bool Equals(AssemblyReference other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return AssemblyEqualityComparer.Default.Equals(_assembly, other._assembly);
		}

		public override int GetHashCode()
		{
			return AssemblyEqualityComparer.Default.GetHashCode(_assembly);
		}

		public override string ToString()
		{
			return _assembly.FullName;
		}
	}
}
