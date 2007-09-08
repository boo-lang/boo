using System;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	class SliceDispatcherFactory : AbstractDispatcherFactory
	{
		public SliceDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments) : base(extensions, target, type, name == "" ? RuntimeServices.GetDefaultMemberName(type) : name, arguments)
		{
		}

		public Dispatcher CreateGetter()
		{
			MemberInfo member = ResolveMember();
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
			CandidateMethod method = ResolveMethod(GetArgumentTypes(), new MethodInfo[] { candidate });
			if (null == method) throw MissingField();

			return new MethodDispatcherEmitter(_type, method, GetArgumentTypes()).Emit();
		}

		private MemberInfo ResolveMember()
		{
			MemberInfo[] candidates = _type.GetMember(_name, MemberTypes.Property | MemberTypes.Field, RuntimeServices.DefaultBindingFlags);
			if (candidates.Length == 0) throw MissingField();
			if (candidates.Length > 1) throw new AmbiguousMatchException(Builtins.join(candidates, ", "));

			return candidates[0];
		}

		public Dispatcher CreateSetter()
		{
			MemberInfo member = ResolveMember();
			switch (member.MemberType)
			{
				case MemberTypes.Field:
				{
					FieldInfo field = (FieldInfo)member;
					return
						delegate(object o, object[] arguments) { return RuntimeServices.SetSlice(field.GetValue(o), "", arguments); };
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
			       		return RuntimeServices.SetSlice(RuntimeServices.GetProperty(o, _name), "", arguments);
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
