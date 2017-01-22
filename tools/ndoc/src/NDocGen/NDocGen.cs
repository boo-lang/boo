using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

class NDocGen
{
	static void Main(string[] args)
	{
		int namespaces = 10;
		int classesPerNamespace = 10;
		int methodsPerClass = 10;

		CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

		for (int i = 0; i < namespaces; ++i)
		{
			CodeNamespace codeNamespace = new CodeNamespace();
			codeCompileUnit.Namespaces.Add(codeNamespace);
			codeNamespace.Name = "Namespace" + i;

			for (int j = 0; j < classesPerNamespace; ++j)
			{
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
				codeNamespace.Types.Add(codeTypeDeclaration);
				codeTypeDeclaration.Name = "Class" + j;
				codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;
				
				codeTypeDeclaration.Comments.Add(
					new CodeCommentStatement(
						"<summary>This is a summary.</summary>",
						true
					)
				);

				for (int k = 0; k < methodsPerClass; ++k)
				{
					CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
					codeTypeDeclaration.Members.Add(codeMemberMethod);
					codeMemberMethod.Name = "Method" + k;
					codeMemberMethod.Attributes = MemberAttributes.Public;

					codeMemberMethod.Comments.Add(
						new CodeCommentStatement(
							"<summary>This is a summary.</summary>",
							true
						)
					);
				}
			}
		}

		CodeDomProvider codeDomProvider = new CSharpCodeProvider();
		ICodeGenerator codeGenerator = codeDomProvider.CreateGenerator();
		CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions();
		codeGenerator.GenerateCodeFromCompileUnit(codeCompileUnit, Console.Out, codeGeneratorOptions);
	}
}
