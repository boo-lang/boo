using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Boo;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.NameBinding;

namespace Boo.Ast.Compilation.Steps
{		
	/// <summary>
	/// Step 4.
	/// </summary>
	public class SemanticStep : AbstractCompilerStep
	{
		ModuleBuilder _moduleBuilder;
		
		TypeBuilder _typeBuilder;
		
		INameSpace _namespace;
		
		public override void Run()
		{
			_moduleBuilder = AssemblySetupStep.GetModuleBuilder(CompilerContext);
			
			Switch(CompileUnit);
		}
		
		public override bool EnterCompileUnit(CompileUnit cu)
		{			
			// builtins resolution at the highest level
			_namespace = new TypeNameSpace(TypeManager, null, typeof(Boo.Lang.Builtins));
			return true;
		}
		
		public override bool EnterModule(Boo.Ast.Module module)
		{
			TypeAttributes attributes = TypeAttributes.Public|TypeAttributes.Sealed;
			_typeBuilder = _moduleBuilder.DefineType(module.FullyQualifiedName, attributes);
			
			TypeManager.SetNameInfo(module, _typeBuilder);
			
			_namespace = new TypeDefinitionNameSpace(TypeManager, _namespace, module);
			return true;
		}
		
		public override void LeaveMethod(Method method)
		{
			// Por enquanto, valor de retorno apenas void
			method.ReturnType = new TypeReference("void");
			TypeManager.SetNameInfo(method.ReturnType, TypeManager.ToTypeInfo(TypeManager.VoidType));
			
			MethodBuilder mbuilder = _typeBuilder.DefineMethod(method.Name,
				                     MethodAttributes.Static|MethodAttributes.Public,
				                     TypeManager.VoidType,
				                     new Type[0]);
			TypeManager.SetNameInfo(method, mbuilder);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			TypeManager.SetNameInfo(node, TypeManager.StringType);
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			INameInfo info = ResolveName(node, node.Name);
			if (null != info)
			{
				TypeManager.SetNameInfo(node, info);
			}
		}
		
		public override void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{			
			INameInfo targetType = TypeManager.GetNameInfo(node.Target);			
			if (targetType.InfoType == NameInfoType.MethodInfo)
			{				
				IMethodInfo targetMethod = (IMethodInfo)targetType;
				CheckParameters(targetMethod, node);
				
				// 1) conferir número de parâmetros ao método
				// 2) conferir compatibilidade dos parâmetros				
				TypeManager.SetNameInfo(node, targetMethod.ReturnType);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
		
		INameInfo ResolveName(Node node, string name)
		{
			INameInfo info = _namespace.Resolve(name);
			if (null == info)
			{
				Errors.UnknownName(node, name);			
			}
			else
			{
				if (info.InfoType == NameInfoType.AmbiguousNameInfo)
				{
					//Errors.AmbiguousName(node, name, info);
					throw new NotImplementedException();
				}
			}
			return info;
		}
		
		void CheckParameters(IMethodInfo method, MethodInvocationExpression mie)
		{			
			if (method.ParameterCount != mie.Arguments.Count)
			{
				Errors.MethodArgumentCount(mie, method);
			}
		}
	}
}
