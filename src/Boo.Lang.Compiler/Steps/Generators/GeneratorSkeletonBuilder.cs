using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps.Generators
{
	class GeneratorSkeletonBuilder : AbstractCompilerComponent
	{
		private Method _enclosingMethod;
		private IType _generatorItemType;
		private Node _sourceNode;

		public BooClassBuilder SkeletonFor(InternalMethod generator)
		{
			_enclosingMethod = generator.Method;
			_sourceNode = _enclosingMethod;
			_generatorItemType = GetGeneratorItemType(generator);

			return CreateGeneratorSkeleton();
		}

		public BooClassBuilder SkeletonFor(GeneratorExpression generator, Method enclosingMethod)
		{
			_enclosingMethod = enclosingMethod;
			_sourceNode = generator;
			_generatorItemType = TypeSystemServices.GetConcreteExpressionType(generator.Expression);

			return CreateGeneratorSkeleton();
		}

		private IType GetGeneratorItemType(InternalMethod generator)
		{
			IType itemType = null;

			if (TypeSystemServices.IsGenericGeneratorReturnType(generator.ReturnType))
			{
				itemType = generator.ReturnType.ConstructedInfo.GenericArguments[0];
			}
			if (itemType == null)
			{
				ExpressionCollection yieldExpressions = generator.YieldExpressions;

				itemType = yieldExpressions.Count > 0
				           	? TypeSystemServices.GetMostGenericType(yieldExpressions)
				           	: TypeSystemServices.ObjectType;
			}
			return itemType;
		}

		BooClassBuilder CreateGeneratorSkeleton()
		{
			// create the class skeleton for type inference to work
			BooClassBuilder builder = CodeBuilder.CreateClass(
				Context.GetUniqueName(_enclosingMethod.Name),
				TypeMemberModifiers.Internal | TypeMemberModifiers.Final);

			builder.LexicalInfo = _sourceNode.LexicalInfo;
			builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));

			BooMethodBuilder getEnumeratorBuilder = null;
			if (_generatorItemType != TypeSystemServices.VoidType)
			{
				builder.AddBaseType(
					TypeSystemServices.Map(typeof(GenericGenerator<>)).GenericInfo.ConstructType(_generatorItemType));

				getEnumeratorBuilder = builder.AddVirtualMethod(
					"GetEnumerator",
					TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(_generatorItemType));

				getEnumeratorBuilder.Method.LexicalInfo = _sourceNode.LexicalInfo;
			}

			_sourceNode["GeneratorClassBuilder"] = builder;
			_sourceNode["GetEnumeratorBuilder"] = getEnumeratorBuilder;
			_sourceNode["GeneratorItemType"] = _generatorItemType;

			return builder;
		}

	}
}