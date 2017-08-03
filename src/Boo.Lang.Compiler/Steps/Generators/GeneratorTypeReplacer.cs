using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;

namespace Boo.Lang.Compiler.Steps.Generators
{
    public class GeneratorTypeReplacer : TypeReplacer
    {
        private readonly Stack<IType> _inConstructedTypes = new Stack<IType>();

        public override IType MapType(IType sourceType)
        {
            var result = base.MapType(sourceType);
            if (result.ConstructedInfo == null && result.GenericInfo != null && !_inConstructedTypes.Contains(sourceType))
                result = ConstructType(sourceType);
            return result;
        }

        public override IType MapConstructedType(IType sourceType)
        {
            var baseType = sourceType.ConstructedInfo.GenericDefinition;
            _inConstructedTypes.Push(baseType);
            try
            {
                return base.MapConstructedType(sourceType);
            }
            finally
            {
                _inConstructedTypes.Pop();
            }
        }

        private IType ConstructType(IType sourceType)
        {
            var parameters = sourceType.GenericInfo.GenericParameters;
            var typeMap = new List<IType>();
            foreach (var param in parameters)
            {
                var match = TypeMap.Keys.FirstOrDefault(t => t.Name.Equals(param.Name));
                if (match == null)
                    break;
                typeMap.Add(TypeMap[match]);
            }
            if (typeMap.Count > 0)
                return sourceType.GenericInfo.ConstructType(typeMap.ToArray());
            return sourceType;
        }

        public static IType MapTypeInMethodContext(IType type, Ast.Method method)
        {
            GeneratorTypeReplacer mapper;
            return MapTypeInMethodContext(type, method, out mapper);
        }

        public bool ContainsType(IType type)
        {
            return TypeMap.ContainsKey(type);
        }

        public bool Any
        {
            get { return TypeMap.Count > 0; }
        }

        public static IType MapTypeInMethodContext(IType type, Ast.Method method, out GeneratorTypeReplacer mapper)
        {
	        if (type.GenericInfo != null && type.ConstructedInfo == null)
	        {
	            var td = method.GetAncestor<Ast.TypeDefinition>();
	            var allGenParams = td.GenericParameters.Concat(method.GenericParameters)
	                .Select(gp => (IGenericParameter) gp.Entity).ToArray();
                mapper = new GeneratorTypeReplacer();
                foreach (var genParam in type.GenericInfo.GenericParameters)
                {
                    var replacement = allGenParams.FirstOrDefault(gp => gp.Name.Equals(genParam.Name));
                    if (replacement != null)
                        mapper.Replace(genParam, replacement);
                }
	            return mapper.MapType(type);
	        }
            mapper = null;
            return type;
        }
    }
}
