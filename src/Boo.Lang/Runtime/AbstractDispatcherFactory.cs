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
	}
}