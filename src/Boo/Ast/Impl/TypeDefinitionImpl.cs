using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class TypeDefinitionImpl : TypeMember
	{
		protected TypeMemberCollection _members;
		protected TypeReferenceCollection _baseTypes;
		
		protected TypeDefinitionImpl()
		{
			_members = new TypeMemberCollection(this);
			_baseTypes = new TypeReferenceCollection(this);
 		}
		
		internal TypeDefinitionImpl(antlr.Token token) : base(token)
		{
			_members = new TypeMemberCollection(this);
			_baseTypes = new TypeReferenceCollection(this);
 		}
		
		internal TypeDefinitionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_members = new TypeMemberCollection(this);
			_baseTypes = new TypeReferenceCollection(this);
 		}
		
		public TypeMemberCollection Members
		{
			get
			{
				return _members;
			}
			
			set
			{
				_members = value;
				if (null != _members)
				{
					_members.InitializeParent(this);
				}
			}
		}
		
		public TypeReferenceCollection BaseTypes
		{
			get
			{
				return _baseTypes;
			}
			
			set
			{
				_baseTypes = value;
				if (null != _baseTypes)
				{
					_baseTypes.InitializeParent(this);
				}
			}
		}
	}
}
