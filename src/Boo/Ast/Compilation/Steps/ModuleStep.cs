using System;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	/// <summary>
	/// Step 3. Cria uma classe para as sentenças e métodos
	/// globais do módulo.
	/// </summary>
	public class ModuleStep : AbstractCompilerStep
	{
		public const string MainMethodName = "__ModuleMain__";
		
		public static Method GetMainMethod(Module module)
		{
			return (Method)module.Members[MainMethodName];
		}
		
		public override void Run()
		{			
			foreach (Module module in CompileUnit.Modules)
			{
				foreach (TypeMember member in module.Members)
				{
					member.Modifiers |= TypeMemberModifiers.Static;
				}		
				
				if (module.Globals.Statements.Count > 0)
				{
					Method method = new Method();
					method.ReturnType = new TypeReference("void");
					method.Body = module.Globals;
					method.Name = MainMethodName;
					method.Modifiers = TypeMemberModifiers.Static;
					module.Members.Add(method);				
				
					module.Globals = new Block();
				}
			}
		}
	}
}
