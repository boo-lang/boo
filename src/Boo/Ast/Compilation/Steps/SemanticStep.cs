using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Boo;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;
using List=Boo.Lang.List;

namespace Boo.Ast.Compilation.Steps
{		
	/// <summary>
	/// Step 4.
	/// </summary>
	public class SemanticStep : AbstractCompilerStep
	{
		ModuleBuilder _moduleBuilder;
		
		TypeBuilder _typeBuilder;
		
		Method _method;
		
		Stack _namespaces = new Stack();		
		
		public override void Run()
		{			
			_moduleBuilder = AssemblySetupStep.GetModuleBuilder(CompilerContext);
			
			Switch(CompileUnit);
		}
		
		public override bool EnterCompileUnit(CompileUnit cu)
		{			
			// Boo.Lang at the highest level
			//_namespace = new NameSpaceNameSpace(BindingManager, 
			                           
			// then builtins resolution
			PushNamespace(new ExternalTypeBinding(BindingManager, typeof(Boo.Lang.Builtins)));
			return true;
		}
		
		public override void OnModule(Boo.Ast.Module module)
		{
			TypeAttributes attributes = TypeAttributes.Public|TypeAttributes.Sealed;
			_typeBuilder = _moduleBuilder.DefineType(module.FullyQualifiedName, attributes);
			
			BindingManager.Bind(module, _typeBuilder);			
			
			PushNamespace(new ModuleNameSpace(BindingManager, module));
			
			module.Attributes.Switch(this);
			module.Members.Switch(this);
			
			PopNamespace();
		}
		
		public override void OnMethod(Method method)
		{
			_method = method;
			
			ProcessParameters(method);
			ProcessReturnType(method);			
			
			MethodBuilder builder = _typeBuilder.DefineMethod(method.Name,
				                     MethodAttributes.Static|MethodAttributes.Private,
				                     BindingManager.GetBoundType(method.ReturnType),
				                     GetParameterTypes(method));
			
			InternalMethodBinding binding = new InternalMethodBinding(BindingManager, method, builder);
			BindingManager.Bind(method, binding);
			
			PushNamespace(binding);
			method.Body.Switch(this);
			PopNamespace();
		}
		
		public override void OnTypeReference(TypeReference node)
		{
			IBinding info = ResolveName(node, node.Name);
			if (null != info)
			{
				if (BindingType.Type != info.BindingType)
				{
					Errors.NameNotType(node, node.Name);
				}
				else
				{
					BindingManager.Bind(node, info);
				}
			}
		}	
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			BindingManager.Bind(node, BindingManager.StringTypeBinding);
		}
		
		public override void LeaveStringFormattingExpression(StringFormattingExpression node)
		{
			BindingManager.Bind(node, BindingManager.StringTypeBinding);
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			IBinding info = ResolveName(node, node.Name);
			if (null != info)
			{
				BindingManager.Bind(node, info);
			}
		}
		
		public override void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			ITypedBinding binding = (ITypedBinding)BindingManager.GetBinding(node.Target);			
			IBinding member = binding.BoundType.Resolve(node.Name);
			if (null != member)
			{
				BindingManager.Bind(node, member);
			}
			else
			{
				Errors.MemberNotFound(node);
			}
		}
		
		public override void OnForStatement(ForStatement node)
		{
			node.Iterator.Switch(this);
			
			ITypeBinding iteratorType = (ITypeBinding)GetBinding(node.Iterator);
			ProcessDeclarationsForIterator(node.Declarations, iteratorType, true);
			
			PushNamespace(new DeclarationsNameSpace(BindingManager, node.Declarations));
			node.Statements.Switch(this);
			PopNamespace();
		}
		
		public override void OnUnpackStatement(UnpackStatement node)
		{
			node.Expression.Switch(this);
			
			ITypeBinding expressionType = (ITypeBinding)GetBinding(node.Expression);
			ProcessDeclarationsForIterator(node.Declarations, expressionType, false);			
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (node.Operator == BinaryOperatorType.Assign &&
			    NodeType.ReferenceExpression == node.Left.NodeType)
			{
				// Auto local declaration:
				// assign to unknown reference implies local
				// declaration
				ReferenceExpression reference = (ReferenceExpression)node.Left;
				node.Right.Switch(this);
					
				ITypeBinding expressionTypeInfo = BindingManager.GetTypeBinding(node.Right);
				
				IBinding info = Resolve(reference.Name);					
				if (null == info)
				{
					DeclareLocal(reference, new Local(reference), expressionTypeInfo);
				}
				else
				{
					// default reference resolution
					if (CheckNameResolution(reference, reference.Name, info))
					{
						BindingManager.Bind(reference, info);
					}
				}
				
				LeaveBinaryExpression(node);
			}
			else
			{
				base.OnBinaryExpression(node);
			}
		}
		
		public override void LeaveBinaryExpression(BinaryExpression node)
		{
			// expression type is the same as the right expression's
			BindingManager.Bind(node, BindingManager.GetBinding(node.Right));
		}		
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
			node.Target.Switch(this);
			node.Arguments.Switch(this);
			
			IBinding targetBinding = BindingManager.GetBinding(node.Target);
			if (BindingType.Ambiguous == targetBinding.BindingType)
			{		
				IBinding[] bindings = ((AmbiguousBinding)targetBinding).Bindings;
				targetBinding = ResolveMethodReference(node, bindings);				
				if (null == targetBinding)
				{
					return;
				}
				BindingManager.Bind(node.Target, targetBinding);
			}	
			
			switch (targetBinding.BindingType)
			{				
				case BindingType.Method:
				{				
					IMethodBinding targetMethod = (IMethodBinding)targetBinding;
					CheckParameters(targetMethod, node);			
					if (node.NamedArguments.Count > 0)
					{
						Errors.NamedParametersNotAllowed(node.NamedArguments[0]);
					}
					// todo: if not CheckParameter
					// Bind(node, BindingManager.UnknownType)
					BindingManager.Bind(node, targetMethod.ReturnType);
					break;
				}
				
				case BindingType.Type:
				{					
					ITypeBinding typeBinding = (ITypeBinding)targetBinding;					
					ResolveNamedArguments(typeBinding, node);
					
					IConstructorBinding ctorBinding = FindCorrectConstructor(typeBinding, node);
					if (null != ctorBinding)
					{
						// rebind the target now we know
						// it is a constructor call
						BindingManager.Bind(node.Target, ctorBinding);
						// expression result type is a new object
						// of type
						BindingManager.Bind(node, typeBinding);
					}
					break;
				}
				
				case BindingType.Error:
				{
					break;
				}
				
				default:
				{
					throw new NotImplementedException();
				}
			}
		}
		
		IBinding Resolve(string name)
		{
			foreach (INameSpace ns in _namespaces)
			{
				IBinding binding = ns.Resolve(name);
				if (null != binding)
				{
					return binding;
				}
			}
			return null;
		}
		
		IBinding ResolveName(Node node, string name)
		{
			IBinding info = null;
			switch (name)
			{
				case "void":
				{
					info = BindingManager.ToTypeBinding(BindingManager.VoidType);
					break;
				}
				
				case "string":
				{
					info = BindingManager.ToTypeBinding(BindingManager.StringType);
					break;
				}
				
				default:
				{
					info = Resolve(name);
					CheckNameResolution(node, name, info);
					break;
				}
			}			
			return info;
		}
		
		bool CheckNameResolution(Node node, string name, IBinding info)
		{
			if (null == info)
			{
				Errors.UnknownName(node, name);			
				return false;
			}
			else
			{
				if (info.BindingType == BindingType.Ambiguous)
				{
					//Errors.AmbiguousName(node, name, info);
					//return false;
					throw new NotImplementedException();
				}
			}
			return true;
		}
		
		void ResolveNamedArguments(ITypeBinding typeBinding, MethodInvocationExpression node)
		{
			foreach (ExpressionPair arg in node.NamedArguments)
			{			
				arg.Second.Switch(this);				
				
				ReferenceExpression name = arg.First as ReferenceExpression;
				if (null == name)
				{
					Errors.NamedParameterMustBeReference(arg);
					continue;				
				}
				
				IBinding member = typeBinding.Resolve(name.Name);
				if (null == member)
				{
					Errors.NotAPublicFieldOrProperty(node, typeBinding.Type, name.Name);
					continue;
				}
				
				BindingManager.Bind(arg.First, member);				
				
				Type memberType = ((ITypedBinding)member).Type;
				Type expressionType = BindingManager.GetBoundType(arg.Second);
				if (!IsAssignableFrom(memberType, expressionType))
				{
					Errors.IncompatibleExpressionType(arg, memberType, expressionType);
				}
			}
		}
		
		void CheckParameters(IMethodBinding method, MethodInvocationExpression mie)
		{	
			ExpressionCollection args = mie.Arguments;
			if (method.ParameterCount != args.Count)
			{
				Errors.MethodArgumentCount(mie, method);
				return;
			}	
			
			for (int i=0; i<args.Count; ++i)
			{
				Type expressionType = BindingManager.GetBoundType(args[i]);
				Type parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType))
				{
					Errors.MethodSignature(mie, GetSignature(mie), GetSignature(method));
					break;
				}
			}
		}
		
		bool IsAssignableFrom(Type expectedType, Type actualType)
		{
			return expectedType.IsAssignableFrom(actualType);
		}
		
		IConstructorBinding FindCorrectConstructor(ITypeBinding typeBinding, MethodInvocationExpression mie)
		{
			IConstructorBinding[] ctors = typeBinding.GetConstructors();
			if (ctors.Length > 0)
			{
				return ctors[0];
			}
			return null;
		}
		
		IBinding ResolveMethodReference(MethodInvocationExpression node, IBinding[] bindings)
		{			
			ExpressionCollection args = node.Arguments;
			
			List valid = new List();			
			foreach (IBinding binding in bindings)
			{
				if (BindingType.Method == binding.BindingType)
				{
					IMethodBinding mb = (IMethodBinding)binding;
					if (args.Count == mb.ParameterCount)
					{
						for (int i=0; i<args.Count; ++i)
						{
							Type expressionType = BindingManager.GetBoundType(args[i]);
							Type parameterType = mb.GetParameterType(i);
							if (!IsAssignableFrom(parameterType, expressionType))
							{
								goto incompatible;
							}
						}
						valid.Add(mb);
					}
					
					incompatible: continue;
				}
			}
			
			if (1 == valid.Count)
			{
				return (IBinding)valid[0];
			}
			
			BindingManager.Error(node);					
			if (valid.Count > 1)
			{
				Errors.AmbiguousName(node, "", valid);							
			}
			else
			{
				Errors.NoApropriateOverloadFound(node, GetSignature(node));
			}
			return null;
		}
		
		void DeclareLocal(Node sourceNode, Local local, ITypeBinding localType)
		{			
			LocalBinding binding = new LocalBinding(local, localType);
			BindingManager.Bind(local, binding);
			
			_method.Locals.Add(local);
			BindingManager.Bind(sourceNode, binding);
		}
		
		void ProcessDeclarationsForIterator(DeclarationCollection declarations, ITypeBinding iteratorType, bool declarePrivateLocals)
		{
			ITypeBinding defaultDeclType = BindingManager.ObjectTypeBinding;
			
			if (iteratorType.Type.IsArray)
			{		
				defaultDeclType = BindingManager.ToTypeBinding(iteratorType.Type.GetElementType());
			}
			
			foreach (Declaration d in declarations)
			{				
				if (null == d.Type)
				{
					d.Type = new TypeReference(defaultDeclType.Type.FullName);
					BindingManager.Bind(d.Type, defaultDeclType);
				}
				else
				{
					d.Type.Switch(this);
					// todo: check types here
				}
				
				DeclareLocal(d, new Local(d), BindingManager.GetTypeBinding(d.Type));
			}
		}
		
		void ProcessParameters(Method method)
		{
			ParameterDeclarationCollection parameters = method.Parameters;
			for (int i=0; i<parameters.Count; ++i)
			{
				ParameterDeclaration parameter = parameters[i];
				if (null == parameter.Type)
				{
					parameter.Type = new TypeReference("object");
					BindingManager.Bind(parameter.Type, BindingManager.ToTypeBinding(BindingManager.ObjectType));
				}		
				else
				{
					parameter.Type.Switch(this);
				}
				Binding.ParameterBinding info = new Binding.ParameterBinding(parameter, BindingManager.GetTypeBinding(parameter.Type), i);
				BindingManager.Bind(parameter, info);
			}
		}
		
		void ProcessReturnType(Method method)
		{
			if (null == method.ReturnType)
			{
				// Por enquanto, valor de retorno apenas void
				method.ReturnType = new TypeReference("void");
				BindingManager.Bind(method.ReturnType, BindingManager.ToTypeBinding(BindingManager.VoidType));
			}
			else
			{
				if (!BindingManager.IsBound(method.ReturnType))
				{
					method.ReturnType.Switch(this);
				}
			}
		}
		
		Type[] GetParameterTypes(Method method)
		{
			ParameterDeclarationCollection parameters = method.Parameters;
			Type[] types = new Type[parameters.Count];
			for (int i=0; i<types.Length; ++i)
			{
				types[i] = BindingManager.GetBoundType(parameters[i].Type);
			}
			return types;
		}
		
		string GetSignature(MethodInvocationExpression mie)
		{
			StringBuilder sb = new StringBuilder("(");
			foreach (Expression arg in mie.Arguments)
			{
				if (sb.Length > 1)
				{
					sb.Append(", ");
				}
				sb.Append(BindingManager.GetBoundType(arg));
			}
			sb.Append(")");
			return sb.ToString();
		}
		
		string GetSignature(IMethodBinding binding)
		{
			return BindingManager.GetSignature(binding);
		}
		
		void PushNamespace(INameSpace ns)
		{
			_namespaces.Push(ns);
		}
		
		void PopNamespace()
		{
			_namespaces.Pop();
		}
		
		IBinding GetBinding(Node node)
		{
			return BindingManager.GetBinding(node);
		}
	}
}
