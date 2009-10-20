using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps.Generators
{
	class GeneratorSkeletonBuilder : AbstractCompilerComponent
	{
		public GeneratorSkeleton SkeletonFor(InternalMethod generator)
		{
			Method enclosingMethod = generator.Method;
			return CreateGeneratorSkeleton(enclosingMethod, enclosingMethod, GetGeneratorItemType(generator));
		}

		public GeneratorSkeleton SkeletonFor(GeneratorExpression generator, Method enclosingMethod)
		{
			return CreateGeneratorSkeleton(generator, enclosingMethod, TypeSystemServices.GetConcreteExpressionType(generator.Expression));
		}

		private IType GetGeneratorItemType(InternalMethod generator)
		{
			IType returnType = generator.ReturnType;
			if (TypeSystemServices.IsGenericGeneratorReturnType(returnType))
				return returnType.ConstructedInfo.GenericArguments[0];
			return TypeSystemServices.ObjectType;
		}

		GeneratorSkeleton CreateGeneratorSkeleton(Node sourceNode, Method enclosingMethod, IType generatorItemType)
		{
			// create the class skeleton for type inference to work
			BooClassBuilder builder = SetUpEnumerableClassBuilder(sourceNode, enclosingMethod, generatorItemType);
			BooMethodBuilder getEnumeratorBuilder = SetUpGetEnumeratorMethodBuilder(sourceNode, builder, generatorItemType);

			enclosingMethod.DeclaringType.Members.Add(builder.ClassDefinition);

			return new GeneratorSkeleton(builder, generatorItemType, getEnumeratorBuilder);
		}

		private BooMethodBuilder SetUpGetEnumeratorMethodBuilder(Node sourceNode, BooClassBuilder builder, IType generatorItemType)
		{
			BooMethodBuilder getEnumeratorBuilder = builder.AddVirtualMethod(
				"GetEnumerator",
				TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(generatorItemType));
			getEnumeratorBuilder.Method.LexicalInfo = sourceNode.LexicalInfo;
			return getEnumeratorBuilder;
		}

		private BooClassBuilder SetUpEnumerableClassBuilder(Node sourceNode, Method enclosingMethod, IType generatorItemType)
		{
			BooClassBuilder builder = CodeBuilder.CreateClass(
				Context.GetUniqueName(enclosingMethod.Name),
				TypeMemberModifiers.Internal | TypeMemberModifiers.Final);

			builder.LexicalInfo = sourceNode.LexicalInfo;
  			builder.AddBaseType(
					TypeSystemServices.Map(typeof(GenericGenerator<>)).GenericInfo.ConstructType(generatorItemType));

			builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));
			return builder;
		}
	}
}