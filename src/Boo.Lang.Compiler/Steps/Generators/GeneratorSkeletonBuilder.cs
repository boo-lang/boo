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
			return CreateGeneratorSkeleton(enclosingMethod, generator, TypeSystemServices.GetConcreteExpressionType(generator.Expression));
		}

		private IType GetGeneratorItemType(InternalMethod generator)
		{
			IType returnType = generator.ReturnType;
			if (TypeSystemServices.IsGenericGeneratorReturnType(returnType))
				return returnType.ConstructedInfo.GenericArguments[0];
			return TypeSystemServices.ObjectType;
		}

		GeneratorSkeleton CreateGeneratorSkeleton(Method enclosingMethod, Node sourceNode, IType generatorItemType)
		{
			// create the class skeleton for type inference to work
			BooClassBuilder builder = CodeBuilder.CreateClass(
				Context.GetUniqueName(enclosingMethod.Name),
				TypeMemberModifiers.Internal | TypeMemberModifiers.Final);

			builder.LexicalInfo = sourceNode.LexicalInfo;
			builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));

			BooMethodBuilder getEnumeratorBuilder = null;
			if (generatorItemType != TypeSystemServices.VoidType)
			{
				builder.AddBaseType(
					TypeSystemServices.Map(typeof(GenericGenerator<>)).GenericInfo.ConstructType(generatorItemType));

				getEnumeratorBuilder = builder.AddVirtualMethod(
					"GetEnumerator",
					TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(generatorItemType));

				getEnumeratorBuilder.Method.LexicalInfo = sourceNode.LexicalInfo;
			}

			enclosingMethod.DeclaringType.Members.Add(builder.ClassDefinition);

			return new GeneratorSkeleton(builder, generatorItemType, getEnumeratorBuilder);
		}

	}
}