using System.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class AssemblyQualifiedNamespaceEntity : IEntity, INamespace
	{
		Assembly _assembly;
		NamespaceEntity _subject;
		
		public AssemblyQualifiedNamespaceEntity(Assembly assembly, NamespaceEntity subject)
		{
			_assembly = assembly;
			_subject = subject;
		}
		
		public string Name
		{
			get
			{
				return _subject.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return string.Format("{0}, {1}", _subject.Name, _assembly);
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Namespace;
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _subject.ParentNamespace;
			}
		}
		
		public bool Resolve(List targetList, string name, EntityType flags)
		{
			return _subject.Resolve(targetList, name, _assembly, flags);
		}
		
		public IEntity[] GetMembers()
		{
			return _subject.GetMembers();
		}
	}
}