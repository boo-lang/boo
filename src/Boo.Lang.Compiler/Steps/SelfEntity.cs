using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class SelfEntity : ITypedEntity
	{
		string _name;
		IType _type;
		
		public SelfEntity(string name, IType type)
		{
			_name = name;
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Unknown;
			}
		}

		public IType Type
		{
			get
			{
				return _type;
			}
			
			set
			{
				_type = value;
			}
		}
	}
}