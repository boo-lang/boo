using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boo.Lang.Environments;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.Immutable;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
    internal static class SystemTypeMapper
    {
        public static ITypeReference GetTypeReference(Type value)
        {
            var host = CompilerContext.Current.Host;

            if (value.IsByRef)
            {
                var referredType = value.GetElementType();
                return GetTypeReference(referredType);
            }
            if (value.IsArray)
                return new VectorTypeReference
                {
                    ElementType = GetTypeReference(value.GetElementType()),
                    InternFactory = host.InternFactory
                }.ResolvedType;
            if (value.IsNested)
            {
                var declaringType = GetTypeReference(value.DeclaringType);
                if (value.IsGenericParameter)
                    throw new Exception("Generic parameters are not supported yet");
                if (value.ContainsGenericParameters)
                    throw new Exception("Generic nested types are not supported yet");
                return new Microsoft.Cci.Immutable.NestedTypeReference(host, declaringType, host.NameTable.GetNameFor(value.Name), 0,
                    value.IsEnum, value.IsValueType).ResolvedType;
            }
            if (value.IsGenericType && !value.ContainsGenericParameters)
                return GetGenericTypeReference(value, host);

            var reflectAsm = value.Assembly;
            var name = reflectAsm.GetName();
            var asm = host.LoadAssembly(new AssemblyIdentity(
                host.NameTable.GetNameFor(reflectAsm.FullName),
                name.CultureInfo.Name,
                name.Version,
                name.GetPublicKeyToken(),
                reflectAsm.Location));
            if (value.IsGenericType)
                return UnitHelper.FindType(host.NameTable, asm, NonGenericName(value.FullName), value.GetGenericArguments().Length);
            return UnitHelper.FindType(host.NameTable, asm, value.FullName);
        }

        private static string NonGenericName(string value)
        {
            var idx = value.IndexOf("`", StringComparison.Ordinal);
            return value.Substring(0, idx);
        }

        private static ITypeDefinition GetGenericTypeReference(Type value, PeReader.DefaultHost host)
        {
            var baseType = value.GetGenericTypeDefinition();
            var baseTypeRef = GetTypeReference(baseType);
            var args = value.GetGenericArguments().Select(GetTypeReference);
            return GenericTypeInstance.GetGenericTypeInstance((INamedTypeDefinition)baseTypeRef, args, host.InternFactory);
        }

    }
}
