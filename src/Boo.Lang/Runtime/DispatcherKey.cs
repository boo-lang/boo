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

namespace Boo.Lang.Runtime
{
	public class DispatcherKey
	{
		public static readonly IEqualityComparer<DispatcherKey> EqualityComparer = new _EqualityComparer();

		private readonly Type _type;
		private readonly string _name;
		private readonly Type[] _arguments;

		public DispatcherKey(Type type, string name) : this(type, name, Type.EmptyTypes)
		{
		}

		public DispatcherKey(Type type, string name, Type[] arguments)
		{
			_type = type;
			_name = name;
			_arguments = arguments;
		}

		public Type[] Arguments
		{
			get { return _arguments;  }
		}

		sealed class _EqualityComparer : IEqualityComparer<DispatcherKey>
		{
			public int GetHashCode(DispatcherKey key)
			{
				return key._type.GetHashCode() ^ key._name.GetHashCode() ^ key._arguments.Length;
			}

			public bool Equals(DispatcherKey x, DispatcherKey y)
			{
				if (x._type != y._type) return false;
				if (x._arguments.Length != y._arguments.Length) return false;
				if (x._name != y._name) return false;
				for (int i = 0; i < x._arguments.Length; ++i)
				{
					if (x._arguments[i] != y._arguments[i]) return false;
				}
				return true;
			}
		}
	}
}
