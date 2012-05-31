using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Services
{
	public class ExtensionTagger : AbstractCompilerComponent
	{
		private IConstructor _extensionAttributeConstructor;

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			CacheExtensionAttributeConstructor();
			TagAssembly();
		}

		public void TagAsExtension(TypeMember member)
		{
			AddAttributeTo(member.Attributes);
			TagDeclaringTypeOf(member);
		}

		private void TagDeclaringTypeOf(TypeMember member)
		{
			var declaringType = member.DeclaringType;
			if (declaringType == null) return;
			TagAsExtension(declaringType);
		}

		private void TagAssembly()
		{
			AddAttributeTo(CompileUnit.Modules.First.AssemblyAttributes);
		}

		private void AddAttributeTo(AttributeCollection attributes)
		{
			if (attributes.ContainsEntity(_extensionAttributeConstructor))
				return;
			attributes.Add(ExtensionAttributeInstance());
		}

		private Attribute ExtensionAttributeInstance()
		{
			return CodeBuilder.CreateAttribute(_extensionAttributeConstructor);
		}

		private void CacheExtensionAttributeConstructor()
		{
			_extensionAttributeConstructor = TypeSystemServices.Map(Types.ClrExtensionAttribute).GetConstructors().Single();
		}
	}
}
