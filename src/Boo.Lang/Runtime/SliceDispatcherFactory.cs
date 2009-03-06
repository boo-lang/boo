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
	class SliceDispatcherFactory : AbstractDispatcherFactory
	{
		public SliceDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments)
			: base(extensions, target, type, name.Length == 0 ? RuntimeServices.GetDefaultMemberName(type) : name, arguments)
		{
		}

		public Dispatcher CreateGetter()
		{
			MemberInfo[] candidates = ResolveMember();
			if (candidates.Length == 1) return CreateGetter(candidates[0]);
			return EmitMethodDispatcher(Getters(candidates));
		}

		private IEnumerable<MethodInfo> Getters(MemberInfo[] candidates)
		{
			foreach (MemberInfo info in candidates)
			{
				PropertyInfo p = info as PropertyInfo;
				if (null == p) continue;
				MethodInfo getter = p.GetGetMethod(true);
				if (null == getter) continue;
				yield return getter;
			}
		}

		private Dispatcher CreateGetter(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					{	
						FieldInfo field = (FieldInfo)member;
						return
							delegate(object o, object[] arguments) { return RuntimeServices.GetSlice(field.GetValue(o), "", arguments); };
					}
				case MemberTypes.Property:
					{
						MethodInfo getter = ((PropertyInfo) member).GetGetMethod(true);
						if (null == getter) throw MissingField();

						if (getter.GetParameters().Length > 0) return EmitMethodDispatcher(getter);

						// TODO: remove the reflection invocation getter.Invoke from the path

						// otherwise its a simple property and the slice
						// should be applied to the return value
						return
							delegate(object o, object[] arguments) { return RuntimeServices.GetSlice(getter.Invoke(o, null), "", arguments); };
					}
				default:
					{
						throw MissingField();
					}
			}
		}

		private Dispatcher EmitMethodDispatcher(MethodInfo candidate)
		{
			return EmitMethodDispatcher(new MethodInfo[] { candidate });
		}

		private Dispatcher EmitMethodDispatcher(IEnumerable<MethodInfo> candidates)
		{
			CandidateMethod method = ResolveMethod(GetArgumentTypes(), candidates);
			if (null == method) throw MissingField();

			return new MethodDispatcherEmitter(_type, method, GetArgumentTypes()).Emit();
		}

		private MemberInfo[] ResolveMember()
		{
			MemberInfo[] candidates = _type.GetMember(_name, MemberTypes.Property | MemberTypes.Field, RuntimeServices.DefaultBindingFlags);
			if (candidates.Length == 0) throw MissingField();
			return candidates;
		}

		public Dispatcher CreateSetter()
		{
			MemberInfo[] candidates = ResolveMember();
			if (candidates.Length > 1) throw new AmbiguousMatchException(Builtins.join(candidates, ", "));
			return CreateSetter(candidates[0]);
		}

		private Dispatcher CreateSetter(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					{
						FieldInfo field = (FieldInfo)member;
						return
							delegate(object o, object[] arguments) { return RuntimeServices.SetSlice(field.GetValue(o), string.Empty, arguments); };
					}
				case MemberTypes.Property:
					{
						PropertyInfo property = (PropertyInfo)member;
						if (property.GetIndexParameters().Length > 0)
						{
							MethodInfo setter = property.GetSetMethod(true);
							if (null == setter) throw MissingField();
							return EmitMethodDispatcher(setter);
						}

						return delegate(object o, object[] arguments)
						       	{
						       		return RuntimeServices.SetSlice(RuntimeServices.GetProperty(o, _name), string.Empty, arguments);
						       	};
					}
				default:
					{
						throw MissingField();
					}
			}
		}
	}
}
