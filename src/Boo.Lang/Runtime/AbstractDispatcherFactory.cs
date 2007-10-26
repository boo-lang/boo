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

namespace Boo.Lang.Runtime
{
	public abstract class AbstractDispatcherFactory
	{
		private readonly ExtensionRegistry _extensions;
		private readonly object _target;
		protected readonly Type _type;
		protected readonly string _name;
		private readonly object[] _arguments;

		public AbstractDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments)
		{
			_extensions = extensions;
			_target = target;
			_type = type;
			_name = name;
			_arguments = arguments;
		}

		protected IEnumerable<MemberInfo> Extensions
		{
			get { return _extensions.Extensions;  }
		}
		
		protected object[] GetExtensionArgs()
		{	
			object[] extensionArgs = new object[_arguments.Length + 1];
			extensionArgs[0] = _target;
			Array.Copy(_arguments, 0, extensionArgs, 1, _arguments.Length);
			return extensionArgs;
		}

		protected Type[] GetArgumentTypes()
		{
			return MethodResolver.GetArgumentTypes(_arguments);
		}

		protected Type[] GetExtensionArgumentTypes()
		{
			return MethodResolver.GetArgumentTypes(GetExtensionArgs());
		}

		protected Dispatcher EmitExtensionDispatcher(CandidateMethod found)
		{
			return new ExtensionMethodDispatcherEmitter(found, GetArgumentTypes()).Emit();
		}

		protected CandidateMethod ResolveExtension(IEnumerable<MethodInfo> candidates)
		{
			MethodResolver resolver = new MethodResolver(GetExtensionArgumentTypes());
			return resolver.ResolveMethod(candidates);
		}

		protected IEnumerable<MethodInfo> GetExtensionMethods()
		{
			return GetExtensions<MethodInfo>(MemberTypes.Method);
		}

		protected IEnumerable<T> GetExtensions<T>(MemberTypes memberTypes)
			where T: MemberInfo
		{
			foreach (MemberInfo m in Extensions)
			{	
				if (m.MemberType != memberTypes) continue;
				if (m.Name != _name) continue;
				yield return (T)m;
			}
		}

		protected static CandidateMethod ResolveMethod(Type[] argumentTypes, IEnumerable<MethodInfo> candidates)
		{
			return new MethodResolver(argumentTypes).ResolveMethod(candidates);
		}

		protected MissingFieldException MissingField()
		{
			return new MissingFieldException(_type.FullName, _name);
		}
	}
}