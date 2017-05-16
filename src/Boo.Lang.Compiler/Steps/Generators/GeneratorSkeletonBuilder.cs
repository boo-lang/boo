#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.Generators
{
	public class GeneratorSkeletonBuilder : AbstractCompilerComponent
	{
		public GeneratorSkeleton SkeletonFor(InternalMethod generator)
		{
			var enclosingMethod = generator.Method;
			return CreateGeneratorSkeleton(enclosingMethod, enclosingMethod, GeneratorItemTypeFor(generator));
		}

		public GeneratorSkeleton SkeletonFor(GeneratorExpression generator, Method enclosingMethod)
		{
			return CreateGeneratorSkeleton(generator, enclosingMethod, TypeSystemServices.GetConcreteExpressionType(generator.Expression));
		}

		protected virtual IType GeneratorItemTypeFor(InternalMethod generator)
		{
			return _generatorItemTypeInferrer.Instance.GeneratorItemTypeFor(generator);
		}

		private EnvironmentProvision<GeneratorItemTypeInferrer> _generatorItemTypeInferrer;

		GeneratorSkeleton CreateGeneratorSkeleton(Node sourceNode, Method enclosingMethod, IType generatorItemType)
		{
			// create the class skeleton for type inference to work
            var replacer = new GeneratorTypeReplacer();
            var builder = SetUpEnumerableClassBuilder(sourceNode, enclosingMethod, generatorItemType, replacer);
			var getEnumeratorBuilder = SetUpGetEnumeratorMethodBuilder(sourceNode, builder, generatorItemType, replacer);

			enclosingMethod.DeclaringType.Members.Add(builder.ClassDefinition);

			return new GeneratorSkeleton(builder, getEnumeratorBuilder, generatorItemType, replacer);
		}

        private BooMethodBuilder SetUpGetEnumeratorMethodBuilder(Node sourceNode, BooClassBuilder builder, IType generatorItemType,
            TypeReplacer replacer)
        {
            generatorItemType = replacer.MapType(generatorItemType);
			var getEnumeratorBuilder = builder.AddVirtualMethod(
				"GetEnumerator",
				TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(generatorItemType));
			getEnumeratorBuilder.Method.LexicalInfo = sourceNode.LexicalInfo;
			return getEnumeratorBuilder;
		}

        private static void CopyConstraints(GenericParameterDeclaration fromParam, GenericParameterDeclaration toParam)
        {
            toParam.Constraints = fromParam.Constraints;
            foreach (var baseType in fromParam.BaseTypes)
                toParam.BaseTypes.Add(baseType.CloneNode());
        }

		private BooClassBuilder SetUpEnumerableClassBuilder(Node sourceNode, Method enclosingMethod, IType generatorItemType,
            TypeReplacer replacer)
		{
			var builder = CodeBuilder.CreateClass(
				Context.GetUniqueName(enclosingMethod.Name, "Enumerable"),
				TypeMemberModifiers.Internal | TypeMemberModifiers.Final);

			if (enclosingMethod.DeclaringType.IsTransient)
				builder.Modifiers |= TypeMemberModifiers.Transient;

			builder.LexicalInfo = new LexicalInfo(sourceNode.LexicalInfo);
			builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));
            foreach (var gen in enclosingMethod.DeclaringType.GenericParameters.Concat(enclosingMethod.GenericParameters))
		    {
		        var replacement = builder.AddGenericParameter(gen.Name);
                CopyConstraints(gen, replacement);
                replacer.Replace((IType)gen.Entity, (IType)replacement.Entity);
		    }
		    generatorItemType = replacer.MapType(generatorItemType);
            builder.AddBaseType(
                    TypeSystemServices.Map(typeof(GenericGenerator<>)).GenericInfo.ConstructType(generatorItemType));

		    return builder;
		}
	}
}