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
		
		INameResolver _resolver;
		
		public override void Run()
		{
			_moduleBuilder = AssemblySetupStep.GetModuleBuilder(CompilerContext);
			
			Switch(CompileUnit);
		}
		
		public override bool EnterCompileUnit(CompileUnit cu)
		{			
			_resolver = new TypeNameResolver(null, typeof(Boo.Lang.Builtins));
			return true;
		}
		
		public override bool EnterModule(Boo.Ast.Module module)
		{
			TypeAttributes attributes = TypeAttributes.Public|TypeAttributes.Sealed;
			_typeBuilder = _moduleBuilder.DefineType(module.FullyQualifiedName, attributes);
			
			TypeManager.SetMemberInfo(module, _typeBuilder);
			
			_resolver = new TypeDefinitionNameResolver(TypeManager, _resolver, module);
			return true;
		}
		
		public override void LeaveMethod(Method method)
		{
			MethodBuilder mbuilder = _typeBuilder.DefineMethod(method.Name,
				                     MethodAttributes.Static|MethodAttributes.Public,
				                     TypeManager.VoidType,
				                     new Type[0]);
			TypeManager.SetMemberInfo(method, mbuilder);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			SetMemberInfo(node, TypeManager.StringType);
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			MemberInfo info = ResolveName(node, node.Name);
			if (null != info)
			{
				SetMemberInfo(node, info);
			}
		}
		
		public override void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{			
			MemberInfo targetType = GetMemberInfo(node.Target);			
			if (targetType.MemberType == MemberTypes.Method)
			{				
				MethodInfo targetMethod = (MethodInfo)targetType;
				CheckParameters(targetMethod, node);
				
				// 1) conferir número de parâmetros ao método
				// 2) conferir compatibilidade dos parâmetros				
				SetMemberInfo(node, targetMethod.ReturnType);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
		
		void SetMemberInfo(Node node, MemberInfo mi)
		{
			TypeManager.SetMemberInfo(node, mi);
		}
		
		MemberInfo GetMemberInfo(Node node)
		{
			return TypeManager.GetMemberInfo(node);
		}
		
		MemberInfo ResolveName(Node node, string name)
		{
			System.Reflection.MemberInfo[] mi = _resolver.Resolve(name);
			if (1 == mi.Length)
			{
				return mi[0];
			}
			
			if (0 == mi.Length)
			{
				Errors.UnknownName(node, name);			
			}
			else
			{
				Errors.AmbiguousName(node, name, mi);
			}
			return null;
		}
		
		void CheckParameters(MethodInfo method, MethodInvocationExpression mie)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != mie.Arguments.Count)
			{
				Errors.MethodArgumentCount(mie, method);
			}
		}
	}
}
