using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class UsingImpl : Node
	{
		protected string _namespace;
		protected ReferenceExpression _assemblyReference;
		protected ReferenceExpression _alias;
		
		protected UsingImpl()
		{
 		}
		
		protected UsingImpl(string namespace_, ReferenceExpression assemblyReference, ReferenceExpression alias)
		{
 			Namespace = namespace_;
			AssemblyReference = assemblyReference;
			Alias = alias;
		}
		
		protected UsingImpl(antlr.Token token, string namespace_, ReferenceExpression assemblyReference, ReferenceExpression alias) : base(token)
		{
 			Namespace = namespace_;
			AssemblyReference = assemblyReference;
			Alias = alias;
		}
		
		internal UsingImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal UsingImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			
			set
			{
				_namespace = value;
			}
		}
		public ReferenceExpression AssemblyReference
		{
			get
			{
				return _assemblyReference;
			}
			
			set
			{
				_assemblyReference = value;
				if (null != _assemblyReference)
				{
					_assemblyReference.InitializeParent(this);
				}
			}
		}
		public ReferenceExpression Alias
		{
			get
			{
				return _alias;
			}
			
			set
			{
				_alias = value;
				if (null != _alias)
				{
					_alias.InitializeParent(this);
				}
			}
		}
	}
}
