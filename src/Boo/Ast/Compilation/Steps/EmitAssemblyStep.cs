using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public class EmitAssemblyStep : AbstractCompilerStep
	{		
		AssemblyBuilder _asmBuilder;
		
		ModuleBuilder _moduleBuilder;
		
		ILGenerator _il;
		
		public override void Run()
		{						
			_asmBuilder = AssemblySetupStep.GetAssemblyBuilder(CompilerContext);
			_moduleBuilder = AssemblySetupStep.GetModuleBuilder(CompilerContext);
			
			Switch(CompileUnit);
			
			DefineEntryPoint();			
		}
		
		public override void LeaveModule(Boo.Ast.Module module)
		{			
			TypeBuilder typeBuilder = (TypeBuilder)TypeManager.GetMemberInfo(module);
			typeBuilder.CreateType();
		}
		
		public override void OnMethod(Method method)
		{			
			MethodBuilder methodBuilder = (MethodBuilder)TypeManager.GetMemberInfo(method);
			_il = methodBuilder.GetILGenerator();
			method.Body.Switch(this);
			_il.Emit(OpCodes.Ret);			
		}
		
		public override void LeaveExpressionStatement(ExpressionStatement node)
		{
			Type type = (Type)TypeManager.GetMemberInfo(node.Expression);
			
			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			if (type != TypeManager.VoidType)
			{
				_il.Emit(OpCodes.Pop);
			}
		}
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
			MethodInfo mi = (MethodInfo)TypeManager.GetMemberInfo(node.Target);
					
			// Empilha os argumentos
			node.Arguments.Switch(this);
			_il.EmitCall(OpCodes.Call, mi, null);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldstr, node.Value);
		}
		
		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != CompilerParameters.OutputType)
			{
				Method method = ModuleStep.GetMainMethod(CompileUnit.Modules[0]);
				MethodInfo mi = (MethodInfo)TypeManager.GetMemberInfo(method);
				
				_asmBuilder.SetEntryPoint(mi, (PEFileKinds)CompilerParameters.OutputType);
			}
		}		
	}
}
