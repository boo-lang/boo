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

		override public Dispatcher Create()
		{
			MemberInfo[] candidates = _type.GetMember(_name, MemberTypes.Property|MemberTypes.Field, RuntimeServices.DefaultBindingFlags);
			if (candidates.Length == 0) return FindExtension();
			if (candidates.Length > 1) throw new AmbiguousMatchException(Builtins.join(candidates, ", "));
			return EmitDispatcherFor(candidates[0]);
		}

		private Dispatcher FindExtension()
		{
			CandidateMethod found = ResolveExtension(GetCandidateExtensions());
			if (null != found) return EmitExtensionDispatcher(found);

			throw new MissingFieldException(_type.FullName, _name);
		}

		private IEnumerable<MethodInfo> GetCandidateExtensions()
		{
			foreach (PropertyInfo p in GetExtensions<PropertyInfo>(MemberTypes.Property))
			{
				yield return p.GetGetMethod(true);
			}
		}

		private Dispatcher EmitDispatcherFor(MemberInfo info)
		{
			switch (info.MemberType)
			{
				case MemberTypes.Property:
					return EmitPropertyDispatcher((PropertyInfo) info);
				default:
					return EmitFieldDispatcher((FieldInfo) info);
			}
		}

		private Dispatcher EmitFieldDispatcher(FieldInfo field)
		{
			return new FieldDispatcherEmitter(field).Emit();
		}

		private Dispatcher EmitPropertyDispatcher(PropertyInfo property)
		{
			Type[] argumentTypes = GetArgumentTypes();
			MethodResolver resolver = new MethodResolver(argumentTypes);
			CandidateMethod found = resolver.ResolveMethod(new MethodInfo[] { property.GetGetMethod(true) });
			return new MethodDispatcherEmitter(_type, found, argumentTypes).Emit();
		}
	}
}
