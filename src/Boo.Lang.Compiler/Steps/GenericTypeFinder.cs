using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
    public class GenericTypeFinder : TypeFinder
    {
	    private bool _localOnly;

	    public GenericTypeFinder() : base(new TypeCollector(type => type is IGenericParameter))
        {
        }

	    public GenericTypeFinder(bool localOnly) : this()
	    {
		    _localOnly = localOnly;
	    }

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			if (!_localOnly || IsLocal(node.Entity, node))
				base.OnReferenceExpression(node);
		}

	    private bool IsLocal(IEntity entity, Node node)
	    {
		    if (entity.EntityType == EntityType.Local)
		    {
			    var local = (InternalLocal) entity;
			    return local.Local.GetAncestor<Method>() == node.GetAncestor<Method>();
		    }
		    if (entity.EntityType == EntityType.Parameter)
		    {
			    var param = (InternalParameter) entity;
			    return param.Node.ParentNode == node.GetAncestor<Method>();
		    }
		    return false;
	    }
    }
}
