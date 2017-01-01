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
#if DNXCORE50
using System.Linq;
#endif
using System.Reflection;

namespace Boo.Lang.Runtime.DynamicDispatching
{
	public class PropertyDispatcherFactory : AbstractDispatcherFactory
	{
		public PropertyDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments) : base(extensions, target, type, name, arguments)
		{
		}

		public Dispatcher CreateSetter()
		{
			return Create(SetOrGet.Set);
		}

		public Dispatcher CreateGetter()
		{
			return Create(SetOrGet.Get);
		}

		private Dispatcher Create(SetOrGet gos)
		{
#if !DNXCORE50
			MemberInfo[] candidates = _type.GetMember(_name, MemberTypes.Property|MemberTypes.Field, RuntimeServices.DefaultBindingFlags);
#else
		    MemberInfo[] candidates = _type.GetMember(_name, RuntimeServices.DefaultBindingFlags);
		    candidates = candidates.Where((v) => v.MemberType == MemberTypes.Property || v.MemberType == MemberTypes.Field).ToArray();
#endif
			if (candidates.Length == 0) return FindExtension(GetCandidateExtensions(gos));
			if (candidates.Length > 1) throw new AmbiguousMatchException(Builtins.join(candidates, ", "));
			return EmitDispatcherFor(candidates[0], gos);
		}

		private Dispatcher FindExtension(IEnumerable<MethodInfo> candidates)
		{
			CandidateMethod found = ResolveExtension(candidates);
			if (null != found) return EmitExtensionDispatcher(found);
			throw MissingField();
		}

		private IEnumerable<MethodInfo> GetCandidateExtensions(SetOrGet gos)
		{
			foreach (PropertyInfo p in GetExtensions<PropertyInfo>(MemberTypes.Property))
			{
				MethodInfo m = Accessor(p, gos);
				if (null == m) continue;
				yield return m;
			}
		}

		private static MethodInfo Accessor(PropertyInfo p, SetOrGet gos)
		{
			return gos == SetOrGet.Get ? p.GetGetMethod(true) : p.GetSetMethod(true);
		}

		private Dispatcher EmitDispatcherFor(MemberInfo info, SetOrGet gos)
		{
			switch (info.MemberType)
			{
				case MemberTypes.Property:
					return EmitPropertyDispatcher((PropertyInfo) info, gos);
				default:
					return EmitFieldDispatcher((FieldInfo) info, gos);
			}
		}

		private Dispatcher EmitFieldDispatcher(FieldInfo field, SetOrGet gos)
		{
#if NO_SYSTEM_REFLECTION_EMIT
			return ReflectionBasedFieldDispatcherFor(field, gos);
#else
			if (field.IsLiteral)
				return ReflectionBasedFieldDispatcherFor(field, gos);
			return SetOrGet.Get == gos
				? new Emitters.GetFieldEmitter(field).Emit()
				: new Emitters.SetFieldEmitter(field, GetArgumentTypes()[0]).Emit();
#endif
		}

		static Dispatcher ReflectionBasedFieldDispatcherFor(FieldInfo field, SetOrGet gos)
		{
			switch (gos)
			{
				case SetOrGet.Get:
					return (target, args) => field.GetValue(target);
				case SetOrGet.Set:
					return (target, args) =>
					{
						var value = args[0];
						field.SetValue(target, RuntimeServices.Coerce(value, field.FieldType));
						return value;
					};
				default:
					throw new ArgumentException();
			}
		}

		private Dispatcher EmitPropertyDispatcher(PropertyInfo property, SetOrGet gos)
		{
			Type[] argumentTypes = GetArgumentTypes();
			MethodInfo accessor = Accessor(property, gos);
			if (null == accessor) throw MissingField();
			CandidateMethod found = ResolveMethod(argumentTypes, new MethodInfo[] { accessor });
			if (null == found) throw MissingField();

#if NO_SYSTEM_REFLECTION_EMIT
			switch (gos)
			{
				case SetOrGet.Get:
					return (target, args) => property.GetValue(target, args);
				case SetOrGet.Set:
					return (target, args) =>
					       	{
								var value = args[args.Length - 1];
								var remainingArgs = new object[args.Length - 1];
								Array.Copy(args, remainingArgs, remainingArgs.Length);
								property.SetValue(target, RuntimeServices.Coerce(value, property.PropertyType), remainingArgs);
								return value;
					       	};
				default:
					throw new ArgumentException();
			}
#else
			if (SetOrGet.Get == gos) return new Emitters.MethodDispatcherEmitter(_type, found, argumentTypes).Emit();
			return new Emitters.SetPropertyEmitter(_type, found, argumentTypes).Emit();
#endif
		}
	}
}
