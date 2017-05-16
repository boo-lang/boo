using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
    class GenericTypeMapper : AbstractFastVisitorCompilerStep
    {
        private readonly GeneratorTypeReplacer _replacer;

        public GenericTypeMapper(GeneratorTypeReplacer replacer)
        {
            _replacer = replacer;
        }

        private void OnTypeReference(TypeReference node)
        {
            var type = (IType)node.Entity;
            node.Entity = _replacer.MapType(type);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            var local = node.Entity as InternalLocal;
            if (local != null)
            {
                var type = local.Type;
                var mappedType = _replacer.MapType(type);
                if (mappedType != type)
                {
                    node.Entity = UpdateLocal(local.Local, mappedType);
                }
            }
			var te = node.Entity as ITypedEntity;
	        if (te != null)
	        {
				if (node.Entity is IGenericMappedMember)
					ReplaceMappedEntity(node, te.Type);
				else if (node.Entity.EntityType == EntityType.Type)
				{
					var type = (IType)node.Entity;
					node.Entity = _replacer.MapType(type);					
				}
				node.ExpressionType = ((ITypedEntity)node.Entity).Type;
			}
        }

	    private static IEntity UpdateLocal(Local local, IType type)
	    {
		    if (type != ((ITypedEntity) local.Entity).Type)
		    {
			    local.Entity = new InternalLocal(local, type);
		    }
		    return local.Entity;
	    }

	    public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            base.OnMemberReferenceExpression(node);
            var member = node.Entity as IMember;
            if (member != null)
            {
                var type = member.Type;
                var mappedType = _replacer.MapType(type);
                if (mappedType != type)
                {
                    _replacer.Replace(type, mappedType);
                    node.ExpressionType = mappedType;
                    ReplaceMappedEntity(node, mappedType);
                }
            }
        }

	    public override void OnGenericReferenceExpression(GenericReferenceExpression node)
	    {
		    base.OnGenericReferenceExpression(node);
		    node.ExpressionType = _replacer.MapType(node.ExpressionType);
	    }

	    public override void OnSelfLiteralExpression(SelfLiteralExpression node)
	    {
		    base.OnSelfLiteralExpression(node);
		    node.ExpressionType = _replacer.MapType(node.ExpressionType);
	    }

	    private void ReplaceMappedEntity(MemberReferenceExpression node, IType mappedType)
        {
            var entity = (IMember)node.Entity;
            var targetType = node.Target.ExpressionType;
			var newEntity = (IMember)NameResolutionService.ResolveMember(targetType, entity.Name, entity.EntityType);
			node.Entity = newEntity;
			if (!newEntity.Type.Equals(mappedType))
			{
				var gmi = newEntity as IGenericMethodInfo;
				if (gmi != null)
				{
					var args = ((IConstructedMethodInfo)entity).GenericArguments.Select(_replacer.MapType).ToArray();
					newEntity = gmi.ConstructMethod(args);
					if (newEntity.Type.Equals(mappedType))
					{
						node.Entity = newEntity;
						return;
					}
				}
				throw new System.NotImplementedException("Incorrect mapped type for " + node.ToCodeString());
			}
        }

		private void ReplaceMappedEntity(ReferenceExpression node, IType mappedType)
		{
			var entity = (IMember)node.Entity;
			var targetType = _replacer.MapType(entity.DeclaringType);
			node.Entity = NameResolutionService.ResolveMember(targetType, entity.Name, entity.EntityType);
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
        {
            base.OnMethodInvocationExpression(node);
            node.ExpressionType = _replacer.MapType(node.ExpressionType);
        }

        public override void OnAwaitExpression(AwaitExpression node)
        {
            base.OnAwaitExpression(node);
            node.ExpressionType = _replacer.MapType(node.ExpressionType);
        }

        public override void OnField(Field node)
        {
            base.OnField(node);
        }

        public override void OnSimpleTypeReference(SimpleTypeReference node)
        {
            OnTypeReference(node);
        }

        public override void OnArrayTypeReference(ArrayTypeReference node)
        {
            base.OnArrayTypeReference(node);
            OnTypeReference(node);
        }

        public override void OnCallableTypeReference(CallableTypeReference node)
        {
            base.OnCallableTypeReference(node);
            OnTypeReference(node);
        }

        public override void OnGenericTypeReference(GenericTypeReference node)
        {
            base.OnGenericTypeReference(node);
            OnTypeReference(node);
        }

        public override void OnGenericTypeDefinitionReference(GenericTypeDefinitionReference node)
        {
            base.OnGenericTypeDefinitionReference(node);
            OnTypeReference(node);
        }
    }
}
