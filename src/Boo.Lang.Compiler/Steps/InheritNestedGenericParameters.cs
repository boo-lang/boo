using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.Steps
{
    class InheritNestedGenericParameters : AbstractFastVisitorCompilerStep, ITypeMemberReifier
    {
        public override void OnStructDefinition(StructDefinition node)
        {
            VisitTypeDefinition(node);
            base.OnStructDefinition(node);
        }

        public override void OnClassDefinition(ClassDefinition node)
        {
            VisitTypeDefinition(node);
            base.OnClassDefinition(node);
        }

        public override void OnInterfaceDefinition(InterfaceDefinition node)
        {
            VisitTypeDefinition(node);
            base.OnInterfaceDefinition(node);
        }

        public override void OnEnumDefinition(EnumDefinition node)
        {
        }

        public override void OnMethod(Method method)
        {
        }

        public override void OnProperty(Property property)
        {
        }

        public override void OnField(Field field)
        {
        }

        private static void VisitTypeDefinition(TypeDefinition node)
        {
            if (node.GenericParameters.IsEmpty)
                return;

            foreach (var subtype in node.Members.OfType<TypeDefinition>())
            {
                if (subtype.NodeType == NodeType.EnumDefinition)
                    continue;
                InheritTypeParameters(subtype, node.GenericParameters);
            }
        }

        private static void InheritTypeParameters(TypeDefinition type, GenericParameterDeclarationCollection gpds)
        {
            foreach (var param in gpds.Reverse())
            {
                var match = type.GenericParameters.FirstOrDefault(p => p.Name.Equals(param.Name));
                if (match == null)
                {
                    match = param.CloneNode();
                    type.GenericParameters.Insert(0, match);
                }
                match["InternalGenericParent"] = param;
            }
            type["TypeRefReplacement"] = BuildReplacement(type);
        }

        private static GenericReferenceExpression BuildReplacement(TypeDefinition type)
        {
            var result = new GenericReferenceExpression{Target = new ReferenceExpression(type.Name)};
            foreach (var gpd in type.GenericParameters)
                result.GenericArguments.Add(new SimpleTypeReference(gpd.Name));
            return result;
        }
        
        public TypeMember Reify(TypeMember node)
        {
            Visit(node);
            return node;
        }
    }
}
