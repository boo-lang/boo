using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ModuleImpl : TypeDefinition
	{
		protected Package _package;
		protected UsingCollection _using;
		protected Block _globals;
		
		protected ModuleImpl()
		{
			_using = new UsingCollection(this);
			Globals = new Block();
 		}
		
		protected ModuleImpl(Package package)
		{
			_using = new UsingCollection(this);
			Globals = new Block();
 			Package = package;
		}
		
		protected ModuleImpl(antlr.Token token, Package package) : base(token)
		{
			_using = new UsingCollection(this);
			Globals = new Block();
 			Package = package;
		}
		
		internal ModuleImpl(antlr.Token token) : base(token)
		{
			_using = new UsingCollection(this);
			Globals = new Block();
 		}
		
		internal ModuleImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_using = new UsingCollection(this);
			Globals = new Block();
 		}
		
		public Package Package
		{
			get
			{
				return _package;
			}
			
			set
			{
				_package = value;
				if (null != _package)
				{
					_package.InitializeParent(this);
				}
			}
		}
		
		public UsingCollection Using
		{
			get
			{
				return _using;
			}
			
			set
			{
				_using = value;
				if (null != _using)
				{
					_using.InitializeParent(this);
				}
			}
		}
		
		public Block Globals
		{
			get
			{
				return _globals;
			}
			
			set
			{
				_globals = value;
				if (null != _globals)
				{
					_globals.InitializeParent(this);
				}
			}
		}
	}
}
