namespace Boo.Lang.Compiler.TypeSystem
{
	public class AliasedNamespace : IEntity, INamespace
	{
		string _alias;
		IEntity _subject;
		
		public AliasedNamespace(string alias, IEntity subject)
		{
			_alias = alias;			
			_subject = subject;
		}
		
		public string Name
		{
			get
			{
				return _alias;
			}
		}
		
		public string FullName
		{
			get
			{
				return _subject.FullName;
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
				return ((INamespace)_subject).ParentNamespace;
			}
		}
		
		public bool Resolve(List targetList, string name, EntityType flags)
		{
			if (name == _alias && NameResolutionService.IsFlagSet(flags, _subject.EntityType))
			{
				targetList.Add(_subject);
				return true;
			}
			return false;
		}
		
		public IEntity[] GetMembers()
		{
			return ((INamespace)_subject).GetMembers();
		}
	}
}