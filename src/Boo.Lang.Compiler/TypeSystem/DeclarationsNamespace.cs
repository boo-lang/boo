using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	class DeclarationsNamespace : INamespace
	{
		INamespace _parent;
		DeclarationCollection _declarations;
		
		public DeclarationsNamespace(INamespace parent, TypeSystemServices tagManager, DeclarationCollection declarations)
		{
			_parent = parent;
			_declarations = declarations;
		}
		
		public DeclarationsNamespace(INamespace parent, TypeSystemServices tagManager, Declaration declaration)
		{
			_parent = parent;
			_declarations = new DeclarationCollection();
			_declarations.Add(declaration);
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public bool Resolve(List targetList, string name, EntityType flags)
		{
			Declaration found = _declarations[name];
			if (null != found)
			{
				IEntity element = TypeSystemServices.GetEntity(found);
				if (NameResolutionService.IsFlagSet(flags, element.EntityType))
				{
					targetList.Add(element);
					return true;
				}
			}
			return false;
		}
		
		public IEntity[] GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
	}
}