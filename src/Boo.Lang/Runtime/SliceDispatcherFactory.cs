using System;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	class SliceDispatcherFactory : AbstractDispatcherFactory
	{
		public SliceDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments) : base(extensions, target, type, name, arguments)
		{
		}

		public Dispatcher Create()
		{
			string name = _name == "" ? RuntimeServices.GetDefaultMemberName(_type) : _name;
			MemberInfo[] candidates = _type.GetMember(name, MemberTypes.Property | MemberTypes.Field, RuntimeServices.DefaultBindingFlags);
			if (candidates.Length == 0) throw new MissingFieldException(_type.FullName, name);
			if (candidates.Length > 1) throw new AmbiguousMatchException(Builtins.join(candidates, ", "));

			MemberInfo member = candidates[0];
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
					if (getter.GetParameters().Length > 0)
					{
						CandidateMethod method = ResolveMethod(GetArgumentTypes(), new MethodInfo[] { getter });
						if (null == method) throw new MissingFieldException(_type.FullName, name);

						return new MethodDispatcherEmitter(_type, method, GetArgumentTypes()).Emit();
					}

					// otherwise its a simple property and the slice
					// should be applied to the return value
					return
						delegate(object o, object[] arguments) { return RuntimeServices.GetSlice(getter.Invoke(o, null), "", arguments); };
				}
				default:
				{
					throw new MissingFieldException(_type.FullName, name);
				}
			}
		}

	}
}
