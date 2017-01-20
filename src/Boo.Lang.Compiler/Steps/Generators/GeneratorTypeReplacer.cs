using System.Linq;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;

namespace Boo.Lang.Compiler.Steps.Generators
{
    public class GeneratorTypeReplacer : TypeReplacer
    {
        public override IType MapType(IType sourceType)
        {
            var result = base.MapType(sourceType);
            if (result.ConstructedInfo == null && result.GenericInfo != null)
                result = ConstructType(sourceType);
            return result;
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
                typeMap.Add(match);
            }
            if (typeMap.Count > 0)
                return sourceType.GenericInfo.ConstructType(typeMap.ToArray());
            return sourceType;
        }
    }
}
