using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Boo.Lang.Compiler.Util;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
    public static class ExternalClassHelper
    {
        private static readonly Dictionary<string, Assembly> _asmCache = new Dictionary<string, Assembly>();
        private static readonly Dictionary<INamedTypeDefinition, Type> _typeCache = new Dictionary<INamedTypeDefinition, Type>();

        public static Attribute CreateAttribute(ICustomAttribute value)
        {
            return (Attribute) CreateObject(value.Type, value.Arguments.ToArray());
        }

        public static object CreateObject(ITypeReference type, params IMetadataExpression[] args)
        {
            var arguments = args.Cast<IMetadataConstant>().Select(a => a.Value).ToArray();
            var rType = (INamedTypeDefinition)type.ResolvedType;
            var objType = GetType(rType);
            return Activator.CreateInstance(objType, arguments);
        }

        public static Type GetType(INamedTypeDefinition cciType)
        {
            Type result;
            if (_typeCache.TryGetValue(cciType, out result))
            {
                var unit = TypeUtilitiesCci.GetUnit(cciType);
                var loc = unit.Location;
                Assembly asm;
                if (!_asmCache.TryGetValue(loc, out asm))
                {
                    asm = Assembly.LoadFile(loc);
                    _asmCache[loc] = asm;
                }
                var ns = TypeUtilitiesCci.GetNamespace(cciType);
                var typeName = ns.Name.Value + '.' + cciType.Name.Value;
                result = asm.GetType(typeName, true);
                _typeCache[cciType] = result;
            }
            return result;

        }
    }
}
