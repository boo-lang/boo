using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class FieldImpl : TypeMember
	{
		protected TypeReference _type;
		
		protected FieldImpl()
		{
 		}
		
		protected FieldImpl(TypeReference type)
		{
 			Type = type;
		}
		
		protected FieldImpl(antlr.Token token, TypeReference type) : base(token)
		{
 			Type = type;
		}
		
		internal FieldImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal FieldImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public TypeReference Type
		{
			get
			{
				return _type;
			}
			
			set
			{
				_type = value;
				if (null != _type)
				{
					_type.InitializeParent(this);
				}
			}
		}
	}
}
