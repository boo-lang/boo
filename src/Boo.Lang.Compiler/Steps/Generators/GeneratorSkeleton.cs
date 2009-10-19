using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;

namespace Boo.Lang.Compiler.Steps.Generators
{
	internal class GeneratorSkeleton
	{
		public readonly BooClassBuilder GeneratorClassBuilder;
		public readonly IType GeneratorItemType;
		public readonly BooMethodBuilder GetEnumeratorBuilder;

		public GeneratorSkeleton(BooClassBuilder generatorBuilder, IType generatorItemType, BooMethodBuilder getEnumeratorBuilder)
		{
			GeneratorClassBuilder = generatorBuilder;
			GeneratorItemType = generatorItemType;
			GetEnumeratorBuilder = getEnumeratorBuilder;
		}
	}
}
