#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler
{
	using System;
	using System.Text;
	using Boo.Lang.Compiler.Ast;
	
	public class CompilerErrorFactory
	{
		private CompilerErrorFactory()
		{
		}
		
		public static CompilerError ClassAlreadyHasBaseType(Node node, string className, string baseType)
		{
			return new CompilerError("BCE0001", node.LexicalInfo, className, baseType);
		}
		
		public static CompilerError NamedParameterMustBeIdentifier(ExpressionPair pair)
		{
			return new CompilerError("BCE0002", pair.First.LexicalInfo);
		}
		
		public static CompilerError NamedArgumentsNotAllowed(Node node)
		{
			return new CompilerError("BCE0003", node.LexicalInfo);
		}
		
		public static CompilerError AmbiguousReference(ReferenceExpression reference, System.Reflection.MemberInfo[] members)
		{
			return new CompilerError("BCE0004",
									  reference.LexicalInfo,
									  reference.Name,
									  ToNameList(members)
									  );
		}
		
		public static CompilerError AmbiguousReference(Node node, string name, System.Collections.IEnumerable names)
		{
			return new CompilerError("BCE0004", node.LexicalInfo, name, ToStringList(names));
		}
		
		public static CompilerError UnknownIdentifier(Node node, string name)
		{
			return new CompilerError("BCE0005", node.LexicalInfo, name);
		}
		
		public static CompilerError CantCastToValueType(Node node, string typeName)
		{
			return new CompilerError("BCE0006", node.LexicalInfo, typeName);
		}

		public static CompilerError NotAPublicFieldOrProperty(Node node, string name, string typeName)
		{
			return new CompilerError("BCE0007", node.LexicalInfo, typeName, name);
		}
		
		public static CompilerError MissingConstructor(Exception error, Node node, Type type, object[] parameters)
		{
			return new CompilerError("BCE0008", node.LexicalInfo, error, type, GetSignature(parameters));
		}
		
		public static CompilerError AttributeApplicationError(Exception error, Boo.Lang.Compiler.Ast.Attribute attribute, Type attributeType)
		{
			return new CompilerError("BCE0009",
					                  attribute.LexicalInfo,
					                  error,
					                  attributeType,
					                  error.Message);
		}
		
		public static CompilerError AstAttributeMustBeExternal(Node node, string attributeType)
		{
			return new CompilerError("BCE0010", node.LexicalInfo, attributeType);
		}
		
		public static CompilerError StepExecutionError(Exception error, ICompilerStep step)
		{
			return new CompilerError("BCE0011", error, step, error.Message);
		}
		
		public static CompilerError TypeMustImplementICompilerStep(string typeName)
		{
			return new CompilerError("BCE0012", LexicalInfo.Empty, typeName);
		}
		
		public static CompilerError AttributeNotFound(string elementName, string attributeName)
		{
			return new CompilerError("BCE0013", LexicalInfo.Empty, elementName, attributeName);
		}
		
		public static CompilerError InvalidAssemblySetUp(Node node)
		{
			return new CompilerError("BCE0014", node.LexicalInfo);
		}
		
		public static CompilerError InvalidNode(Node node)
		{
			return new CompilerError("BCE0015", node.LexicalInfo, node);
		}
		
		public static CompilerError MethodArgumentCount(Node node, string name, int count)
		{
			return new CompilerError("BCE0016", node.LexicalInfo, name, count);
		}
		
		public static CompilerError MethodSignature(Node node, string expectedSignature, string actualSignature)
		{
			return new CompilerError("BCE0017", node.LexicalInfo, expectedSignature, actualSignature);
		}
		
		public static CompilerError NameNotType(Node node, string name)
		{
			return new CompilerError("BCE0018", node.LexicalInfo, name);
		}
		
		public static CompilerError MemberNotFound(MemberReferenceExpression node, string namespace_)
		{
			return new CompilerError("BCE0019", node.LexicalInfo, node.Name, namespace_);
		}

		public static CompilerError InstanceRequired(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0020", node.LexicalInfo, typeName, memberName);
		}
		
		public static CompilerError InvalidNamespace(Import import)
		{
			return new CompilerError("BCE0021", import.LexicalInfo, import.Namespace);
		}
		
		public static CompilerError IncompatibleExpressionType(Node node, string expectedType, string actualType)
		{
			return new CompilerError("BCE0022", node.LexicalInfo, expectedType, actualType);
		}
		
		public static CompilerError NoApropriateOverloadFound(Node node, string signature, string memberName)
		{
			return new CompilerError("BCE0023", node.LexicalInfo, signature, memberName);
		}
		
		public static CompilerError NoApropriateConstructorFound(Node node, string typeName, string signature)
		{
			return new CompilerError("BCE0024", node.LexicalInfo, typeName, signature);
		}
		
		public static CompilerError InvalidArray(Node node)
		{
			return new CompilerError("BCE0025", node.LexicalInfo);
		}
		
		public static CompilerError BoolExpressionRequired(Node node, string typeName)
		{
			return new CompilerError("BCE0026", node.LexicalInfo, typeName);
		}
		
		public static CompilerError NoEntryPoint()
		{
			return new CompilerError("BCE0028", LexicalInfo.Empty);
		}
		
		public static CompilerError MoreThanOneEntryPoint(Method method)
		{
			return new CompilerError("BCE0029", method.LexicalInfo);
		}
		
		public static CompilerError NotImplemented(Node node, string message)
		{
			return new CompilerError("BCE0031", node.LexicalInfo, message);
		}
		
		public static CompilerError EventArgumentMustBeAMethod(Node node, string eventName, string eventType)
		{
			return new CompilerError("BCE0032", node.LexicalInfo, eventName, eventType);
		}
		
		public static CompilerError TypeNotAttribute(Node node, string attributeType)
		{
			return new CompilerError("BCE0033", node.LexicalInfo, attributeType);
		}
		
		public static CompilerError ExpressionMustBeExecutedForItsSideEffects(Node node)
		{
			return new CompilerError("BCE0034", node.LexicalInfo);
		}

		public static CompilerError ConflictWithInheritedMember(Node node, string member, string baseMember)
		{
			return new CompilerError("BCE0035", node.LexicalInfo, member, baseMember);
		}
		
		public static CompilerError InvalidTypeof(Node node)
		{
			return new CompilerError("BCE0036", node.LexicalInfo);
		}
		
		public static CompilerError UnknownMacro(Node node, string name)
		{
			return new CompilerError("BCE0037", node.LexicalInfo, name);
		}
		
		public static CompilerError InvalidMacro(Node node, string name)
		{
			return new CompilerError("BCE0038", node.LexicalInfo, name);
		}
		
		public static CompilerError AstMacroMustBeExternal(Node node, string typeName)
		{
			return new CompilerError("BCE0039", node.LexicalInfo, typeName);
		}
		
		public static CompilerError UnableToLoadAssembly(Node node, string name, Exception error)
		{
			return new CompilerError("BCE0041", node.LexicalInfo, error, name);
		}
		
		public static CompilerError InputError(string inputName, Exception error)
		{
			return new CompilerError("BCE0042", new LexicalInfo(inputName), error, inputName, error.Message);
		}
		
		public static CompilerError UnexpectedToken(LexicalInfo lexicalInfo, Exception error, string token)
		{
			return new CompilerError("BCE0043", lexicalInfo, error, token);
		}
		
		public static CompilerError GenericParserError(LexicalInfo lexicalInfo, Exception error)
		{
			return new CompilerError("BCE0044", lexicalInfo, error, error.Message);
		}
		
		public static CompilerError MacroExpansionError(Node node, Exception error)
		{
			return new CompilerError("BCE0045", node.LexicalInfo, error, error.Message);
		}
		
		public static CompilerError OperatorCantBeUsedWithValueType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0046", node.LexicalInfo, operatorName, typeName);
		}
		
		public static CompilerError CantOverrideNonVirtual(Node node, string fullName)
		{
			return new CompilerError("BCE0047", node.LexicalInfo, fullName);
		}
		
		public static CompilerError TypeDoesNotSupportSlicing(Node node, string fullName)
		{
			return new CompilerError("BCE0048", node.LexicalInfo, fullName);
		}
		
		public static CompilerError LValueExpected(Node node)
		{
			return new CompilerError("BCE0049", node.LexicalInfo);
		}
		
		public static CompilerError InvalidOperatorForType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0050", node.LexicalInfo, operatorName, typeName);
		}
		
		public static CompilerError InvalidOperatorForTypes(Node node, string operatorName, string lhs, string rhs)
		{
			return new CompilerError("BCE0051", node.LexicalInfo, operatorName, lhs, rhs);
		}
		
		public static CompilerError InvalidLen(Node node, string typeName)
		{
			return new CompilerError("BCE0052", node.LexicalInfo, typeName);
		}
		
		public static CompilerError PropertyIsReadOnly(Node node, string propertyName)
		{
			return new CompilerError("BCE0053", node.LexicalInfo, propertyName);
		}
		
		public static CompilerError IsaArgument(Node node)
		{
			return new CompilerError("BCE0054", node.LexicalInfo);
		}
		
		public static CompilerError InternalError(Node node, Exception error)
		{
			return new CompilerError("BCE0055", node.LexicalInfo, error, error.Message);
		}
		
		public static CompilerError FileNotFound(string fname)
		{
			return new CompilerError("BCE0056", new LexicalInfo(fname), fname);
		}
		
		public static CompilerError CantRedefinePrimitive(Node node, string name)
		{
			return new CompilerError("BCE0057", node.LexicalInfo, name);
		}

		/*
		 * 
		 * Deprecated
		public static CompilerError ObjectRequired(Node node)
		{
			return new CompilerError("BCE0058", node.LexicalInfo);
		}
		*/
		
		public static CompilerError InvalidLockMacroArguments(Node node)
		{
			return new CompilerError("BCE0059", node.LexicalInfo);
		}
		
		public static CompilerError NoMethodToOverride(Node node, string signature)
		{
			return new CompilerError("BCE0060", node.LexicalInfo, signature);
		}
		
		public static CompilerError MethodIsNotOverride(Node node, string signature)
		{
			return new CompilerError("BCE0061", node.LexicalInfo, signature);
		}
		
		public static CompilerError CouldNotInferReturnType(Node node, string signature)
		{
			return new CompilerError("BCE0062", node.LexicalInfo, signature);
		}
		
		public static CompilerError NoEnclosingLoop(Node node)
		{
			return new CompilerError("BCE0063", node.LexicalInfo);
		}
		
		public static CompilerError UnknownAttribute(Node node, string attributeName)
		{
			return new CompilerError("BCE0064", node.LexicalInfo, attributeName);
		}
		
		public static CompilerError InvalidIteratorType(Node node, string typeName)
		{
			return new CompilerError("BCE0065", node.LexicalInfo, typeName);
		}
		
		public static CompilerError InvalidNodeForAttribute(LexicalInfo info, string attributeName, string expectedNodeTypes)
		{
			return new CompilerError("BCE0066", info, attributeName, expectedNodeTypes);
		}
		
		public static CompilerError LocalAlreadyExists(Node node, string name)
		{
			return new CompilerError("BCE0067", node.LexicalInfo, name);
		}
		
		public static CompilerError PropertyRequiresParameters(Node node, string name)
		{
			return new CompilerError("BCE0068", node.LexicalInfo, name);
		}
		
		public static CompilerError InterfaceCanOnlyInheritFromInterface(Node node, string interfaceName, string baseType)
		{
			return new CompilerError("BCE0069", node.LexicalInfo, interfaceName, baseType);
		}
		
		public static CompilerError UnresolvedDependency(Node node, string source, string target)
		{
			return new CompilerError("BCE0070", node.LexicalInfo, source, target);
		}
		
		public static CompilerError InheritanceCycle(Node node, string typeName)
		{
			return new CompilerError("BCE0071", node.LexicalInfo, typeName);
		}
		
		public static CompilerError InvalidOverrideReturnType(Node node, string methodName, string expectedReturnType, string actualReturnType)
		{
			return new CompilerError("BCE0072", node.LexicalInfo, methodName, expectedReturnType, actualReturnType);
		}
		
		public static CompilerError AbstractMethodCantHaveBody(Node node, string methodName)
		{
			return new CompilerError("BCE0073", node.LexicalInfo, methodName);
		}
		
		public static CompilerError SelfOutsideMethod(Node node)
		{
			return new CompilerError("BCE0074", node.LexicalInfo);
		}
		
		public static CompilerError NamespaceIsNotAnExpression(Node node, string name)
		{
			return new CompilerError("BCE0075", node.LexicalInfo, name);
		}
		
		public static CompilerError RuntimeMethodBodyMustBeEmpty(Node node, string name)
		{
			return new CompilerError("BCE0076", node.LexicalInfo, name);
		}
		
		public static CompilerError TypeIsNotCallable(Node node, string name)
		{
			return new CompilerError("BCE0077", node.LexicalInfo, name);
		}
		
		public static CompilerError MethodReferenceExpected(Node node)
		{
			return new CompilerError("BCE0078", node.LexicalInfo);
		}
		
		public static CompilerError AddressOfOutsideDelegateConstructor(Node node)
		{
			return new CompilerError("BCE0079", node.LexicalInfo);
		}
		
		public static CompilerError BuiltinCannotBeUsedAsExpression(Node node, string name)
		{
			return new CompilerError("BCE0080", node.LexicalInfo, name);
		}
		
		public static CompilerError ReRaiseOutsideExceptionHandler(Node node)
		{
			return new CompilerError("BCE0081", node.LexicalInfo);
		}
		
		public static CompilerError EventTypeIsNotCallable(Node node, string typeName)
		{
			return new CompilerError("BCE0082", node.LexicalInfo, typeName);
		}
		
		public static CompilerError StaticConstructorMustBePublic(Node node)
		{
			return new CompilerError("BCE0083", node.LexicalInfo);
		}
		
		public static CompilerError StaticConstructorCannotDeclareParameters(Node node)
		{
			return new CompilerError("BCE0084", node.LexicalInfo);
		}
		
		public static CompilerError CantCreateInstanceOfAbstractType(Node node, string typeName)
		{
			return new CompilerError("BCE0085", node.LexicalInfo, typeName);
		}
		
		public static CompilerError CantCreateInstanceOfInterface(Node node, string typeName)
		{
			return new CompilerError("BCE0086", node.LexicalInfo, typeName);
		}
		
		public static CompilerError CantCreateInstanceOfEnum(Node node, string typeName)
		{
			return new CompilerError("BCE0087", node.LexicalInfo, typeName);
		}
		
		public static CompilerError ReservedPrefix(Node node, string prefix)
		{
			return new CompilerError("BCE0088", node.LexicalInfo, prefix);
		}
		
		public static CompilerError MemberNameConflict(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0089", node.LexicalInfo, typeName, memberName);
		}
		
		public static CompilerError DerivedMethodCannotReduceAccess(Node node, string derivedMethod, string superMethod, TypeMemberModifiers derivedAccess, TypeMemberModifiers superAccess)
		{
			return new CompilerError("BCE0090", node.LexicalInfo, derivedMethod, superMethod, superAccess.ToString().ToLower(), derivedAccess.ToString().ToLower());
		}
		
		public static CompilerError EventIsNotAnExpression(Node node, string eventName)
		{
			return new CompilerError("BCE0091", node.LexicalInfo, eventName);
		}
		
		public static CompilerError InvalidRaiseArgument(Node node, string typeName)
		{
			return new CompilerError("BCE0092", node.LexicalInfo, typeName);
		}
		
		public static CompilerError CannotBranchIntoEnsure(Node node)
		{
			return new CompilerError("BCE0093", node.LexicalInfo);
		}
		
		public static CompilerError CannotBranchIntoExcept(Node node)
		{
			return new CompilerError("BCE0094", node.LexicalInfo);
		}
		
		public static CompilerError NoSuchLabel(Node node, string label)
		{
			return new CompilerError("BCE0095", node.LexicalInfo, label);
		}
		
		public static CompilerError LabelAlreadyDefined(Node node, string methodName, string label)
		{
			return new CompilerError("BCE0096", node.LexicalInfo, methodName, label);
		}
		
		public static CompilerError CannotBranchIntoTry(Node node)
		{
			return new CompilerError("BCE0097", node.LexicalInfo);
		}
		
		public static CompilerError InvalidSwitch(Node node)
		{
			return new CompilerError("BCE0098", node.LexicalInfo);
		}
		
		public static CompilerError YieldInsideTryBlock(Node node)
		{
			return new CompilerError("BCE0099", node.LexicalInfo);
		}
		
		public static CompilerError YieldInsideConstructor(Node node)
		{
			return new CompilerError("BCE0100", node.LexicalInfo);
		}
		
		public static CompilerError InvalidGeneratorReturnType(Node node)
		{
			return new CompilerError("BCE0101", node.LexicalInfo);
		}
		
		public static CompilerError GeneratorCantReturnValue(Node node)
		{
			return new CompilerError("BCE0102", node.LexicalInfo);
		}
		
		public static CompilerError CannotExtendFinalType(Node node, string typeName)
		{
			return new CompilerError("BCE0103", node.LexicalInfo, typeName);
		}
		
		public static CompilerError CantBeMarkedTransient(Node node)
		{
			return new CompilerError("BCE0104", node.LexicalInfo);
		}
		
		public static CompilerError CantBeMarkedAbstract(Node node)
		{
			return new CompilerError("BCE0105", node.LexicalInfo);
		}
		
		public static CompilerError FailedToLoadTypesFromAssembly(string assemblyName, Exception x)
		{
			return new CompilerError("BCE0106", LexicalInfo.Empty, x, assemblyName);
		}
		
		public static CompilerError ValueTypesCannotDeclareParameterlessConstructors(Node node)
		{
			return new CompilerError("BCE0107", node.LexicalInfo);
		}
		
		public static CompilerError ValueTypeFieldsCannotHaveInitializers(Node node)
		{
			return new CompilerError("BCE0108", node.LexicalInfo);
		}
		
		public static CompilerError InvalidArrayRank(Node node, string arrayName, int real, int given)
		{
			return new CompilerError("BCE0109", node.LexicalInfo, arrayName, real, given);
		}
		
		public static CompilerError NotANamespace(Node node, string name)
		{
			return new CompilerError("BCE0110", node.LexicalInfo, name);
		}

		public static CompilerError InvalidDestructorModifier(Node node)
		{
			return new CompilerError("BCE0111", node.LexicalInfo);
		}
		
		public static CompilerError CantHaveDestructorParameters(Node node)
		{
			return new CompilerError("BCE0112", node.LexicalInfo);
		}
		
		public static CompilerError InvalidCharLiteral(Node node, string value)
		{
			return new CompilerError("BCE0113", node.LexicalInfo, value);
		}
		
		public static CompilerError InvalidInterfaceForInterfaceMember(Node node, string value)
		{
			return new CompilerError("BCE0114", node.LexicalInfo, value);
		}

		public static CompilerError InterfaceImplForInvalidInterface(Node node, string iface, string item)
		{
			return new CompilerError("BCE0115", node.LexicalInfo, iface, item);
		}
		
		public static CompilerError ExplicitImplMustNotHaveModifiers(Node node, string iface, string item)
		{
			return new CompilerError("BCE0116", node.LexicalInfo, iface, item);
		}

		public static CompilerError FieldIsReadonly(Node node, string name)
		{
			return new CompilerError("BCE0117", node.LexicalInfo, name);
		}

		public static CompilerError ExplodedExpressionMustBeArray(Node node)
		{
			return new CompilerError("BCE0118", node.LexicalInfo);
		}

		public static CompilerError ExplodeExpressionMustMatchVarArgCall(Node node)
		{
			return new CompilerError("BCE0119", node.LexicalInfo);
		}
		
		public static string ToStringList(System.Collections.IEnumerable names)
		{
			StringBuilder builder = new StringBuilder();
			foreach (object name in names)
			{
				if (builder.Length > 0)
				{
					builder.Append(", ");
				}
				builder.Append(name.ToString());
			}
			return builder.ToString();
		}
		
		public static string ToAssemblyQualifiedNameList(List types)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(((Type)types[0]).AssemblyQualifiedName);
			for (int i=1; i<types.Count; ++i)
			{
				builder.Append(", ");
				builder.Append(((Type)types[i]).AssemblyQualifiedName);
			}
			return builder.ToString();
		}
		
		public static string GetSignature(object[] parameters)
		{
			StringBuilder sb = new StringBuilder("(");
			for (int i=0; i<parameters.Length; ++i)
			{
				if (i>0)
				{
					sb.Append(", ");
				}
				if (null != parameters)
				{
					sb.Append(parameters[i].GetType());
				}
			}
			sb.Append(")");
			return sb.ToString();
		}

		public static string ToNameList(System.Reflection.MemberInfo[] members)
		{
			StringBuilder sb = new StringBuilder();
			for (int i=0; i<members.Length; ++i)
			{
				if (i>0) { sb.Append(", "); }
				sb.Append(members[i].MemberType.ToString());
				sb.Append(" ");
				sb.Append(members[i].Name);
			}
			return sb.ToString();
		}
	}
}
