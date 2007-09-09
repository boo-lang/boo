using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public class ExtensionRegistry
	{
		private List<MemberInfo> _extensions = new List<MemberInfo>();
		
		public void Register(Type type)
		{
			lock (this)
			{
				_extensions = AddExtensionMembers(CopyExtensions(), type);
			}
		}

		public IEnumerable<MemberInfo> Extensions
		{
			get { lock(this) { return _extensions; }  }
		}

		public void UnRegister(Type type)
		{
			lock (this)
			{
				List<MemberInfo> extensions = CopyExtensions();
				extensions.RemoveAll(delegate(MemberInfo member) { return member.DeclaringType == type; });
				_extensions = extensions;
			}
		}

		private static List<MemberInfo> AddExtensionMembers(List<MemberInfo> extensions, Type type)
		{
			foreach (MemberInfo member in type.GetMembers(BindingFlags.Static | BindingFlags.Public))
			{
				if (!Attribute.IsDefined(member, typeof(Boo.Lang.ExtensionAttribute))) continue;
				if (extensions.Contains(member)) continue;
				extensions.Add(member);
			}
			return extensions;
		}

		private List<MemberInfo> CopyExtensions()
		{
			return new List<MemberInfo>(_extensions);
		}
	}
}
