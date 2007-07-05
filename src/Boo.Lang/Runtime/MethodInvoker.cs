#region license
// Copyright (c) 2004, 2007 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	 * Redistributions of source code must retain the above copyright notice,
//	 this list of conditions and the following disclaimer.
//	 * Redistributions in binary form must reproduce the above copyright notice,
//	 this list of conditions and the following disclaimer in the documentation
//	 and/or other materials provided with the distribution.
//	 * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	 contributors may be used to endorse or promote products derived from this
//	 software without specific prior written permission.
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
using System.Reflection;
using System.Collections.Generic;

namespace Boo.Lang.Runtime
{
	internal class MethodInvoker
	{
		private object _target;
		private Type _type;
		private string _methodName;
		private object[] _arguments;

		private static Dictionary<MethodDispatcherKey, MethodDispatcher> _cache =
			new Dictionary<MethodDispatcherKey, MethodDispatcher>(MethodDispatcherKey.EqualityComparer);

		public MethodInvoker(object target, Type type, string methodName, object[] arguments)
		{
			_target = target;
			_type = type;
			_methodName = methodName;
			_arguments = arguments;
		}

		public object InvokeResolvedMethod()
		{
			Type[] argumentTypes = GetArgumentTypes();
			MethodDispatcherKey key = new MethodDispatcherKey(_type, _methodName, argumentTypes);
			MethodDispatcher dispatcher;
			if (!_cache.TryGetValue(key, out dispatcher))
			{
				CandidateMethod found = ResolveMethod(argumentTypes);
				dispatcher = EmitMethodDispatcher(found, argumentTypes);
				_cache.Add(key, dispatcher);
			}
			return dispatcher(_target, _arguments);
		}

		private CandidateMethod ResolveMethod(Type[] argumentTypes)
		{
			CandidateMethod found = new MethodResolver(argumentTypes).ResolveMethod(GetCandidates());
			if (found == null) throw new System.MissingMethodException(_type.FullName, _methodName);
			return found;
		}

		private IEnumerable<MethodInfo> GetCandidates()
		{
			foreach (MethodInfo method in _type.GetMethods(RuntimeServices.DefaultBindingFlags))
			{
				if (_methodName != method.Name) continue;
				yield return method;
			}
		}

		private Type[] GetArgumentTypes()
		{
			return MethodResolver.GetArgumentTypes(_arguments);
		}

		private MethodDispatcher EmitMethodDispatcher(CandidateMethod found, Type[] argumentTypes)
		{
			return new MethodDispatcherEmitter(_type, found, argumentTypes).Emit();
		}

		class MethodDispatcherKey
		{
			public static readonly IEqualityComparer<MethodDispatcherKey> EqualityComparer = new _EqualityComparer();

			private readonly Type _type;
			private readonly string _methodName;
			private readonly Type[] _arguments;

			public MethodDispatcherKey(Type type, string methodName, Type[] arguments)
			{
				_type = type;
				_methodName = methodName;
				_arguments = arguments;
			}

			class _EqualityComparer : IEqualityComparer<MethodDispatcherKey>
			{
				public int GetHashCode(MethodDispatcherKey key)
				{
					return key._type.GetHashCode() ^ key._methodName.GetHashCode() ^ key._arguments.Length;
				}

				public bool Equals(MethodDispatcherKey x, MethodDispatcherKey y)
				{
					if (x._type != y._type) return false;
					if (x._arguments.Length != y._arguments.Length) return false;
					if (x._methodName != y._methodName) return false;
					for (int i = 0; i < x._arguments.Length; ++i)
					{
						if (x._arguments[i] != y._arguments[i]) return false;
					}
					return true;
				}
			}
		}
	}
}