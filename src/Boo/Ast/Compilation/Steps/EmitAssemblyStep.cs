using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.NameBinding;

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
			TypeBuilder typeBuilder = TypeManager.GetTypeBuilder(module);
			typeBuilder.CreateType();
		}
		
		public override void OnMethod(Method method)
		{			
			MethodBuilder methodBuilder = TypeManager.GetMethodBuilder(method);
			_il = methodBuilder.GetILGenerator();
			method.Locals.Switch(this);
			method.Body.Switch(this);
			_il.Emit(OpCodes.Ret);			
		}
		
		public override void OnLocal(Local local)
		{			
			LocalInfo info = TypeManager.GetLocalInfo(local);
			LocalBuilder builder = _il.DeclareLocal(info.Type);
			builder.SetLocalSymInfo(local.Name);
			info.LocalBuilder = builder;
		}
		
		public override void LeaveExpressionStatement(ExpressionStatement node)
		{
			Type type = (Type)TypeManager.GetType(node.Expression);
			
			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			if (type != NameBinding.TypeManager.VoidType)
			{
				_il.Emit(OpCodes.Pop);
			}
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				LocalInfo local = TypeManager.GetNameInfo(node.Left) as LocalInfo;
				if (null == local)
				{
					throw new NotImplementedException();
				}
				
				node.Right.Switch(this);
				
				// assignment result is right expression
				_il.Emit(OpCodes.Dup);
				_il.Emit(OpCodes.Stloc, local.LocalBuilder);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
			MethodInfo mi = TypeManager.GetMethodInfo(node.Target);
					
			// Empilha os argumentos
			node.Arguments.Switch(this);
			_il.EmitCall(OpCodes.Call, mi, null);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			_il.Emit(OpCodes.Ldstr, node.Value);
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			LocalInfo local = TypeManager.GetNameInfo(node) as LocalInfo;
			if (null == local)
			{
				throw new NotImplementedException();
			}
			_il.Emit(OpCodes.Ldloc, local.LocalBuilder);
		}
		
		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != CompilerParameters.OutputType)
			{
				Method method = ModuleStep.GetMainMethod(CompileUnit.Modules[0]);
				MethodInfo mi = TypeManager.GetMethodInfo(method);
				
				_asmBuilder.SetEntryPoint(mi, (PEFileKinds)CompilerParameters.OutputType);
			}
		}		
	}
}
