namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class TransformCallableDefinitions : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnMethod(Method node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members);
		}
		
		override public void OnCallableDefinition(CallableDefinition node)
		{
			if (null == node.ReturnType)
			{
				node.ReturnType = CreateTypeReference(TypeSystemServices.VoidType);
			}
			
			foreach (ParameterDeclaration parameter in node.Parameters)
			{
				if (null == parameter.Type)
				{
					parameter.Type = CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
			
			ClassDefinition cd = new ClassDefinition(node.LexicalInfo);
			cd.BaseTypes.Add(CreateTypeReference(TypeSystemServices.MulticastDelegateType));
			cd.Name = node.Name;
			cd.Modifiers = TypeMemberModifiers.Final;
			cd.Members.Add(CreateCallableConstructor());
			cd.Members.Add(CreateInvokeMethod(node));
			
			ReplaceCurrentNode(cd);
		}
		
		Constructor CreateCallableConstructor()
		{
			Constructor constructor = new Constructor();
			constructor.Modifiers = TypeMemberModifiers.Public;
			constructor.ImplementationFlags = MethodImplementationFlags.Runtime;
			constructor.Parameters.Add(
						new ParameterDeclaration("instance", CreateTypeReference(TypeSystemServices.ObjectType)));
			constructor.Parameters.Add(
						new ParameterDeclaration("method", CreateTypeReference(TypeSystemServices.IntPtrType)));						
			return constructor;
		}
		
		Method CreateInvokeMethod(CallableDefinition node)
		{
			Method method = new Method("Invoke");
			method.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Virtual;
			method.ImplementationFlags = MethodImplementationFlags.Runtime;
			method.Parameters = node.Parameters;
			method.ReturnType = node.ReturnType;
			return method;
		}
	}
}
