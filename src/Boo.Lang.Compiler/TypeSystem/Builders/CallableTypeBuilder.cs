using System;
using System.Runtime.CompilerServices;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public class CallableTypeBuilder : AbstractCompilerComponent
	{
		public ClassDefinition ForCallableDefinition(CallableDefinition node)
		{
			ClassDefinition cd = CreateEmptyCallableDefinition(node.Name);
			cd.LexicalInfo = node.LexicalInfo;
			cd.GenericParameters = node.GenericParameters;

			cd.Members.Add(CreateInvokeMethod(node));
			cd.Members.Add(CreateBeginInvokeMethod(node));
			cd.Members.Add(CreateEndInvokeMethod(node));

			return cd;
		}

		public ClassDefinition CreateEmptyCallableDefinition(string name)
		{
			var cd = new ClassDefinition();
			cd.IsSynthetic = true;
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(TypeSystemServices.MulticastDelegateType));
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(TypeSystemServices.ICallableType));
			cd.Name = name;
			cd.Modifiers = TypeMemberModifiers.Final;
			cd.Members.Add(CreateCallableConstructor());
			cd.Members.Add(CreateCallMethod());
			cd.Entity = new InternalCallableType(My<InternalTypeSystemProvider>.Instance, cd);
			cd.Attributes.Add(CodeBuilder.CreateAttribute(typeof(CompilerGeneratedAttribute)));
			return cd;
		}

		private Method CreateCallMethod()
		{
			Method method = CodeBuilder.CreateMethod("Call", TypeSystemServices.ObjectType, TypeMemberModifiers.Public | TypeMemberModifiers.Virtual);
			method.Parameters.Add(CodeBuilder.CreateParameterDeclaration(1, "args", TypeSystemServices.ObjectArrayType));
			return method;
		}

		private Constructor CreateCallableConstructor()
		{
			Constructor constructor = CodeBuilder.CreateConstructor(TypeMemberModifiers.Public);
			constructor.ImplementationFlags = MethodImplementationFlags.Runtime;
			constructor.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(1, "instance", TypeSystemServices.ObjectType));
			constructor.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(2, "method", TypeSystemServices.IntPtrType));
			return constructor;
		}


		Method CreateInvokeMethod(CallableDefinition node)
		{
			Method method = CreateRuntimeMethod("Invoke", node.ReturnType);
			method.Parameters = node.Parameters;
			return method;
		}

		Method CreateBeginInvokeMethod(CallableDefinition node)
		{
			Method method = CreateRuntimeMethod("BeginInvoke",
						CodeBuilder.CreateTypeReference(node.LexicalInfo, typeof(IAsyncResult)));
			method.Parameters.ExtendWithClones(node.Parameters);
			method.Parameters.Add(
				new ParameterDeclaration("callback",
					CodeBuilder.CreateTypeReference(node.LexicalInfo, typeof(AsyncCallback))));
			method.Parameters.Add(
				new ParameterDeclaration("asyncState",
					CodeBuilder.CreateTypeReference(node.LexicalInfo, TypeSystemServices.ObjectType)));
			return method;
		}

		Method CreateEndInvokeMethod(CallableDefinition node)
		{
			Method method = CreateRuntimeMethod("EndInvoke", node.ReturnType);

			foreach (ParameterDeclaration p in node.Parameters)
				if (p.IsByRef)
					method.Parameters.Add(p.CloneNode());

			method.Parameters.Add(
				new ParameterDeclaration("asyncResult",
					CodeBuilder.CreateTypeReference(node.LexicalInfo, typeof(IAsyncResult))));
			return method;
		}

		Method CreateRuntimeMethod(string name, TypeReference returnType)
		{
			Method method = new Method();
			method.Name = name;
			method.ReturnType = returnType;
			method.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Virtual;
			method.ImplementationFlags = MethodImplementationFlags.Runtime;
			return method;
		}

	}
}
