using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;

namespace Boo.Ast.Compilation.Steps
{
	public class EmitAssemblyStep : AbstractCompilerStep
	{		
		public static MethodInfo GetEntryPoint(CompileUnit cu)
		{
			return (MethodInfo)cu[EntryPointKey];
		}
		
		static object EntryPointKey = new object();
		
		MethodInfo StringFormatMethodInfo = BindingManager.StringType.GetMethod("Format", new Type[] { BindingManager.StringType, BindingManager.ObjectArrayType });
		
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
			TypeBuilder typeBuilder = BindingManager.GetTypeBuilder(module);
			typeBuilder.CreateType();
		}		
		
		public override void OnMethod(Method method)
		{			
			MethodBuilder methodBuilder = BindingManager.GetMethodBuilder(method);
			_il = methodBuilder.GetILGenerator();
			method.Locals.Switch(this);
			method.Body.Switch(this);
			_il.Emit(OpCodes.Ret);			
		}
		
		public override void OnLocal(Local local)
		{			
			LocalBinding info = BindingManager.GetLocalBinding(local);
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
			Type type = (Type)BindingManager.GetBoundType(node.Expression);
			
			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			if (type != Binding.BindingManager.VoidType)
			{
				_il.Emit(OpCodes.Pop);
			}
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				LocalBinding local = BindingManager.GetBinding(node.Left) as LocalBinding;
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
			IBinding binding = BindingManager.GetBinding(node.Target);
			switch (binding.BindingType)
			{
				case BindingType.Method:
				{					
					MethodInfo mi = (MethodInfo)((IMethodBinding)binding).MethodInfo;
					if (!mi.IsStatic)
					{
						// pushes target reference
						node.Target.Switch(this);
					}
					node.Arguments.Switch(this);
					_il.EmitCall(OpCodes.Call, mi, null);
					break;
				}
				
				case BindingType.Constructor:
				{
					node.Arguments.Switch(this);
					_il.Emit(OpCodes.Newobj, ((IConstructorBinding)binding).ConstructorInfo);
					foreach (ExpressionPair pair in node.NamedArguments)
					{
						_il.Emit(OpCodes.Dup);
					}
					break;
				}
				
				default:
				{
					throw new NotImplementedException();
				}
			}
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
			_il.Emit(OpCodes.Newarr, BindingManager.ObjectType);
			
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
			IBinding info = BindingManager.GetBinding(node);
			switch (info.BindingType)
			{
				case BindingType.Local:
				{
					LocalBinding local = (LocalBinding)info;
					_il.Emit(OpCodes.Ldloc, local.LocalBuilder);
					break;
				}
				
				case BindingType.Parameter:
				{
					Binding.ParameterBinding param = (Binding.ParameterBinding)info;
					_il.Emit(OpCodes.Ldarg, param.Index);
					break;
				}
				
				case BindingType.Assembly:
				{
					// ignores "using namespace from assembly" clause
					break;
				}
				
				default:
				{
					throw new NotImplementedException(info.ToString());
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
