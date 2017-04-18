using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem;
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
                    node.Entity = new InternalLocal(local.Local, mappedType);
                }
            }
			var te = node.Entity as ITypedEntity;
			if (te != null && te.Type != node.ExpressionType)
				node.ExpressionType = te.Type;
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

        private void ReplaceMappedEntity(MemberReferenceExpression node, IType mappedType)
        {
            var entity = (IMember)node.Entity;
            var targetType = node.Target.ExpressionType;
            node.Entity = NameResolutionService.ResolveMember(targetType, entity.Name, entity.EntityType);
            if (!((IMember)node.Entity).Type.Equals(mappedType))
                throw new System.NotImplementedException("Incorrect mapped type for " + node.ToCodeString());
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
