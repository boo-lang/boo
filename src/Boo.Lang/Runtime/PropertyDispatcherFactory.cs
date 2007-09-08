using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Runtime
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
			MemberInfo[] candidates = _type.GetMember(_name, MemberTypes.Property|MemberTypes.Field, RuntimeServices.DefaultBindingFlags);
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

		private MissingFieldException MissingField()
		{
			return new MissingFieldException(_type.FullName, _name);
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
			return SetOrGet.Get == gos
			       	? new GetFieldEmitter(field).Emit()
			       	: new SetFieldEmitter(field, GetArgumentTypes()[0]).Emit();
		}

		private Dispatcher EmitPropertyDispatcher(PropertyInfo property, SetOrGet gos)
		{
			Type[] argumentTypes = GetArgumentTypes();
			MethodInfo accessor = Accessor(property, gos);
			if (null == accessor) throw MissingField();
			CandidateMethod found = ResolveMethod(argumentTypes, new MethodInfo[] { accessor });
			if (null == found) throw MissingField();
			if (SetOrGet.Get == gos) return new MethodDispatcherEmitter(_type, found, argumentTypes).Emit();
			return new SetPropEmitter(_type, found, argumentTypes).Emit();
		}
	}
}
