using System;
using System.Diagnostics.SymbolStore;
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
		public static MethodInfo GetEntryPoint(CompileUnit cu)
		{
			return (MethodInfo)cu[EntryPointKey];
		}
		
		static object EntryPointKey = new object();
		
		MethodInfo StringFormatMethodInfo = TypeManager.StringType.GetMethod("Format", new Type[] { TypeManager.StringType, TypeManager.ObjectArrayType });
		
		AssemblyBuilder _asmBuilder;
		
		ModuleBuilder _moduleBuilder;
		
		ISymbolDocumentWriter _symbolDocWriter;
		
		ILGenerator _il;
		
		public override void Run()
		{				
			if (Errors.Count > 0)
			{
				return;				
			}
			
			_asmBuilder = AssemblySetupStep.GetAssemblyBuilder(CompilerContext);
			_moduleBuilder = AssemblySetupStep.GetModuleBuilder(CompilerContext);			
			
			Switch(CompileUnit);
			
			DefineEntryPoint();			
		}
		
		public override bool EnterModule(Boo.Ast.Module module)
		{
			_symbolDocWriter = _moduleBuilder.DefineDocument(module.LexicalInfo.FileName, Guid.Empty, Guid.Empty, Guid.Empty);
			return true;
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
		
		public override bool EnterExpressionStatement(ExpressionStatement node)
		{
			EmitDebugInfo(node.LexicalInfo);			
			return true;
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
		
		public override void OnStringFormattingExpression(StringFormattingExpression node)
		{
			_il.Emit(OpCodes.Ldstr, node.Template);
			
			// new object[node.Arguments.Count]
			_il.Emit(OpCodes.Ldc_I4, node.Arguments.Count);
			_il.Emit(OpCodes.Newarr, TypeManager.ObjectType);
			
			ExpressionCollection args = node.Arguments;
			for (int i=0; i<args.Count; ++i)
			{			
				_il.Emit(OpCodes.Dup);	// array reference
				_il.Emit(OpCodes.Ldc_I4, i); // element index
				args[i].Switch(this); // value
				_il.Emit(OpCodes.Stelem_Ref);
			}
			
			_il.EmitCall(OpCodes.Call, StringFormatMethodInfo, null);
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			INameInfo info = TypeManager.GetNameInfo(node);
			switch (info.InfoType)
			{
				case NameInfoType.Local:
				{
					LocalInfo local = (LocalInfo)info;
					_il.Emit(OpCodes.Ldloc, local.LocalBuilder);
					break;
				}
				
				case NameInfoType.Parameter:
				{
					NameBinding.ParameterInfo param = (NameBinding.ParameterInfo)info;
					_il.Emit(OpCodes.Ldarg, param.Index);
					break;
				}
				
				default:
				{
					throw new NotImplementedException();
				}
				
			}			
		}
		
		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != CompilerParameters.OutputType)
			{
				Module main = CompileUnit.Modules[0];
				Method method = ModuleStep.GetMainMethod(main);
				Type type = _asmBuilder.GetType(main.FullyQualifiedName, true);
				MethodInfo mi = type.GetMethod(method.Name, BindingFlags.Static|BindingFlags.NonPublic);
				
				_asmBuilder.SetEntryPoint(mi, (PEFileKinds)CompilerParameters.OutputType);
				CompileUnit[EntryPointKey] = mi;
			}
		}		
		
		void EmitDebugInfo(LexicalInfo info)
		{
			_il.MarkSequencePoint(_symbolDocWriter, info.Line, info.Column-1, info.Line, info.Column-1);
		}
	}
}
