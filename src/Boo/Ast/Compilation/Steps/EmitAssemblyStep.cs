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
		
		static MethodInfo String_Format = typeof(string).GetMethod("Format", new Type[] { Binding.BindingManager.StringType, Binding.BindingManager.ObjectArrayType });
		
		static MethodInfo RuntimeServices_MoveNext = Binding.BindingManager.RuntimeServicesType.GetMethod("MoveNext");
		
		static MethodInfo RuntimeServices_CheckArrayUnpack = Binding.BindingManager.RuntimeServicesType.GetMethod("CheckArrayUnpack");
		
		static MethodInfo RuntimeServices_GetEnumerable = Binding.BindingManager.RuntimeServicesType.GetMethod("GetEnumerable");
		
		static MethodInfo IEnumerable_GetEnumerator = Binding.BindingManager.IEnumerableType.GetMethod("GetEnumerator");
		
		static MethodInfo IEnumerator_MoveNext = Binding.BindingManager.IEnumeratorType.GetMethod("MoveNext");
		
		static MethodInfo IEnumerator_get_Current = Binding.BindingManager.IEnumeratorType.GetProperty("Current").GetGetMethod();
		
		AssemblyBuilder _asmBuilder;
		
		ModuleBuilder _moduleBuilder;
		
		ISymbolDocumentWriter _symbolDocWriter;
		
		ILGenerator _il;
		
		public override void Run()
		{				
			if (Errors.Count > 0 || 0 == CompileUnit.Modules.Count)
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
			TypeBuilder typeBuilder = GetTypeBuilder(module);
			typeBuilder.CreateType();
		}		
		
		public override void OnMethod(Method method)
		{			
			MethodBuilder methodBuilder = GetMethodBuilder(method);
			_il = methodBuilder.GetILGenerator();
			method.Locals.Switch(this);
			method.Body.Switch(this);
			_il.Emit(OpCodes.Ret);			
		}
		
		public override void OnLocal(Local local)
		{			
			LocalBinding info = GetLocalBinding(local);
			info.LocalBuilder = _il.DeclareLocal(info.Type);
			info.LocalBuilder.SetLocalSymInfo(local.Name);			
		}
		
		public override void OnForStatement(ForStatement node)
		{			
			ITypeBinding binding = GetTypeBinding(node.Iterator);			
			
			EmitDebugInfo(node.Iterator);
			// iterator = <node.Iterator>;
			node.Iterator.Switch(this);
			
			if (binding.Type.IsArray)
			{
				EmitArrayBasedFor(node, binding);
			}
			else
			{
				EmitEnumerableBasedFor(node, binding);
			}			
		}
		
		public override void OnUnpackStatement(UnpackStatement node)
		{
			DeclarationCollection decls = node.Declarations;
			
			ITypeBinding binding = GetTypeBinding(node.Expression);
			
			EmitDebugInfo(node);						
			node.Expression.Switch(this);
			
			EmitUnpackForDeclarations(node.Declarations, binding.Type);			
		}
		
		public override bool EnterExpressionStatement(ExpressionStatement node)
		{
			EmitDebugInfo(node);			
			return true;
		}		
		
		public override void LeaveExpressionStatement(ExpressionStatement node)
		{
			Type type = GetBoundType(node.Expression);
			
			// if the type of the inner expression is not
			// void we need to pop its return value to leave
			// the stack sane
			if (type != Binding.BindingManager.VoidType)
			{
				_il.Emit(OpCodes.Pop);
			}
		}
		
		public override void OnIfStatement(IfStatement node)
		{
			node.Expression.Switch(this);
			
			Label endLabel = _il.DefineLabel();
			_il.Emit(OpCodes.Brfalse, endLabel);			
			node.TrueBlock.Switch(this);
			_il.MarkLabel(endLabel);
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
				IBinding binding = BindingManager.GetBinding(node);
				if (BindingType.Method == binding.BindingType)
				{
					IMethodBinding methodBinding = (IMethodBinding)binding;
					node.Left.Switch(this);
					EmitCastIfNeeded(methodBinding.GetParameterType(0), GetBoundType(node.Left));					
					node.Right.Switch(this);
					EmitCastIfNeeded(methodBinding.GetParameterType(1), GetBoundType(node.Right));
					_il.EmitCall(OpCodes.Call, (MethodInfo)methodBinding.MethodInfo, null);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
			IBinding binding = BindingManager.GetBinding(node.Target);
			switch (binding.BindingType)
			{
				case BindingType.Method:
				{										
					IMethodBinding methodBinding = (IMethodBinding)binding;
					MethodInfo mi = (MethodInfo)methodBinding.MethodInfo;
					OpCode code = OpCodes.Call;
					if (!mi.IsStatic)
					{
						// pushes target reference
						node.Target.Switch(this);
						if (mi.IsVirtual)
						{
							code = OpCodes.Callvirt;
						}
					}
					PushArguments(methodBinding, node.Arguments);
					_il.EmitCall(code, mi, null);
					break;
				}
				
				case BindingType.Constructor:
				{
					IConstructorBinding constructorBinding = (IConstructorBinding)binding;
					PushArguments(constructorBinding, node.Arguments);
					_il.Emit(OpCodes.Newobj, constructorBinding.ConstructorInfo);
					foreach (ExpressionPair pair in node.NamedArguments)
					{
						// object reference
						_il.Emit(OpCodes.Dup);
						// value
						pair.Second.Switch(this);
						// field/property reference						
						SetFieldOrProperty(BindingManager.GetBinding(pair.First));
					}
					break;
				}
				
				default:
				{
					throw new NotImplementedException(binding.ToString());
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
				StoreElementReference(i, args[i]);				
			}
			
			_il.EmitCall(OpCodes.Call, String_Format, null);
		}
		
		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			node.Target.Switch(this);			
			IBinding binding = BindingManager.GetBinding(node);
			switch (binding.BindingType)
			{
				case BindingType.Property:
				{
					IPropertyBinding property = (IPropertyBinding)binding;
					_il.EmitCall(OpCodes.Callvirt, property.PropertyInfo.GetGetMethod(), null);
					break;
				}
				
				case BindingType.Method:
				{
					break;
				}
				
				default:
				{
					throw new NotImplementedException(binding.ToString());
				}
			}
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
		
		void SetFieldOrProperty(IBinding binding)
		{
			switch (binding.BindingType)
			{
				case BindingType.Property:
				{
					IPropertyBinding property = (IPropertyBinding)binding;
					PropertyInfo pi = property.PropertyInfo;
					_il.EmitCall(OpCodes.Callvirt, pi.GetSetMethod(), null);
					break;
				}
					
				case BindingType.Field:
				{
					throw new NotImplementedException();					
				}
				
				default:
				{
					throw new ArgumentException("binding");
				}				
			}
		}			
		
		void EmitDebugInfo(Node node)
		{
			LexicalInfo info = node.LexicalInfo;
			_il.MarkSequencePoint(_symbolDocWriter, info.Line, info.Column-1, info.Line, info.Column-1);
		}
		
		void EmitEnumerableBasedFor(ForStatement node, ITypeBinding iteratorBinding)
		{			
			Label labelTest = _il.DefineLabel();
			Label labelEnd = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(Binding.BindingManager.IEnumeratorType);
			EmitGetEnumerableIfNeeded(iteratorBinding.Type);			
			_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
			_il.Emit(OpCodes.Stloc, localIterator);
			
			// iterator.MoveNext()
			EmitDebugInfo(node.Iterator);
			_il.MarkLabel(labelTest);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_MoveNext, null);
			_il.Emit(OpCodes.Brfalse, labelEnd);			
			
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.EmitCall(OpCodes.Callvirt, IEnumerator_get_Current, null);
			EmitUnpackForDeclarations(node.Declarations, Binding.BindingManager.ObjectType);
			
			node.Statements.Switch(this);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelEnd);			
		}
		
		void EmitArrayBasedFor(ForStatement node, ITypeBinding iteratorBinding)
		{			
			Label labelTest = _il.DefineLabel();
			Label labelEnd = _il.DefineLabel();
			
			LocalBuilder localIterator = _il.DeclareLocal(iteratorBinding.Type);
			_il.Emit(OpCodes.Stloc, localIterator);
			
			// i = 0;
			LocalBuilder localIndex = _il.DeclareLocal(BindingManager.IntType);
			_il.Emit(OpCodes.Ldc_I4_0);
			_il.Emit(OpCodes.Stloc, localIndex);			
			
			// i<iterator.Length			
			_il.MarkLabel(labelTest);			
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.Emit(OpCodes.Ldlen);
			_il.Emit(OpCodes.Bge, labelEnd);			
			
			EmitDebugInfo(node.Iterator);
			// value = iterator[i]
			_il.Emit(OpCodes.Ldloc, localIterator);
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Ldelem_Ref);
			
			EmitUnpackForDeclarations(node.Declarations, iteratorBinding.Type.GetElementType());
			
			node.Statements.Switch(this);
			
			// ++i
			_il.Emit(OpCodes.Ldc_I4_1);
			_il.Emit(OpCodes.Ldloc, localIndex);
			_il.Emit(OpCodes.Add);
			_il.Emit(OpCodes.Stloc, localIndex);
			_il.Emit(OpCodes.Br, labelTest);
			
			_il.MarkLabel(labelEnd);
		}
		
		void EmitUnpackForDeclarations(DeclarationCollection decls, Type topOfStack)
		{
			if (1 == decls.Count)
			{
				// for arg in iterator
				LocalBuilder localValue = GetLocalBuilder(decls[0]);
				EmitDebugInfo(decls[0]);
				StoreLocal(topOfStack, localValue);
			}
			else
			{
				if (topOfStack.IsArray)
				{	
					Type elementType = topOfStack.GetElementType();
					
					// RuntimeServices.CheckArrayUnpack(array, decls.Count);					
					_il.Emit(OpCodes.Dup);
					_il.Emit(OpCodes.Ldc_I4, decls.Count);					
					_il.EmitCall(OpCodes.Call, RuntimeServices_CheckArrayUnpack, null);
					
					for (int i=0; i<decls.Count; ++i)
					{
						// local = array[i]
						_il.Emit(OpCodes.Dup);
						_il.Emit(OpCodes.Ldc_I4, i); // element index			
						_il.Emit(OpCodes.Ldelem_Ref);
						
						StoreLocal(elementType, GetLocalBuilder(decls[i]));					
					}
				}
				else
				{
					EmitGetEnumerableIfNeeded(topOfStack);
					_il.EmitCall(OpCodes.Callvirt, IEnumerable_GetEnumerator, null);
					
					foreach (Declaration d in decls)
					{
						_il.Emit(OpCodes.Dup);
						_il.EmitCall(OpCodes.Call, RuntimeServices_MoveNext, null);				
						StoreLocal(BindingManager.ObjectType, GetLocalBuilder(d));				
					}					
				}
				_il.Emit(OpCodes.Pop);
			}
		}
		
		void EmitGetEnumerableIfNeeded(Type topOfStack)
		{
			if (!IsIEnumerableCompatible(topOfStack))
			{
				_il.EmitCall(OpCodes.Call, RuntimeServices_GetEnumerable, null);
			}
		}
		
		bool IsIEnumerableCompatible(Type type)
		{
			return Binding.BindingManager.IEnumerableType.IsAssignableFrom(type);
		}
		
		void PushArguments(IMethodBinding binding, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				Expression arg = args[i];
				
				Type expectedType = binding.GetParameterType(i);
				Type actualType = GetBoundType(arg);
				arg.Switch(this);
				EmitCastIfNeeded(expectedType, actualType);
			}
		}
		
		void EmitCastIfNeeded(Type expectedType, Type actualType)
		{
			if (!expectedType.IsAssignableFrom(actualType))
			{
				_il.Emit(OpCodes.Castclass, expectedType);
			}
		}
		
		void StoreLocal(Type topOfStack, LocalBuilder localBuilder)
		{
			EmitCastIfNeeded(localBuilder.LocalType, topOfStack);
			_il.Emit(OpCodes.Stloc, localBuilder);
		}
		
		void StoreElementReference(int index, Node value)
		{
			_il.Emit(OpCodes.Dup);	// array reference
			_il.Emit(OpCodes.Ldc_I4, index); // element index
			value.Switch(this); // value
			_il.Emit(OpCodes.Stelem_Ref);
		}		
		
		void DefineEntryPoint()
		{
			if (CompilerOutputType.Library != CompilerParameters.OutputType)
			{
				Module main = CompileUnit.Modules[0];
				Method method = ModuleStep.GetMainMethod(main);
				if (null != method)
				{
					Type type = _asmBuilder.GetType(main.FullyQualifiedName, true);
					MethodInfo mi = type.GetMethod(method.Name, BindingFlags.Static|BindingFlags.NonPublic);
					
					_asmBuilder.SetEntryPoint(mi, (PEFileKinds)CompilerParameters.OutputType);
					CompileUnit[EntryPointKey] = mi;
				}
				else
				{
					Errors.NoEntryPoint(main);
				}
			}
		}	

	}
}
