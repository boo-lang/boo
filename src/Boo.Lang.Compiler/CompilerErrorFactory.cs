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

using Boo.Lang.Compiler.TypeSystem;

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
		
		public static CompilerError CustomError(LexicalInfo lexicalInfo, string msg)
		{
			return new CompilerError(lexicalInfo, msg);
		}
		
		public static CompilerError CustomError(string msg)
		{
			return new CompilerError(msg);
		}
		
		public static CompilerError ClassAlreadyHasBaseType(Node node, string className, string baseType)
		{
			return new CompilerError("BCE0001", SafeLexicalInfo(node), className, baseType);
		}
		
		public static CompilerError NamedParameterMustBeIdentifier(ExpressionPair pair)
		{
			return new CompilerError("BCE0002", SafeLexicalInfo(pair.First));
		}
		
		public static CompilerError NamedArgumentsNotAllowed(Node node)
		{
			return new CompilerError("BCE0003", SafeLexicalInfo(node));
		}
		
		public static CompilerError AmbiguousReference(ReferenceExpression reference, System.Reflection.MemberInfo[] members)
		{
			return new CompilerError("BCE0004",
									  SafeLexicalInfo(reference),
									  reference.Name,
									  ToNameList(members)
									  );
		}
		
		public static CompilerError AmbiguousReference(Node node, string name, System.Collections.IEnumerable names)
		{
			return new CompilerError("BCE0004", SafeLexicalInfo(node), name, ToStringList(names));
		}
		
		public static CompilerError UnknownIdentifier(Node node, string name)
		{
			return new CompilerError("BCE0005", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError CantCastToValueType(Node node, string typeName)
		{
			return new CompilerError("BCE0006", SafeLexicalInfo(node), typeName);
		}

		public static CompilerError NotAPublicFieldOrProperty(Node node, string name, string typeName)
		{
			return new CompilerError("BCE0007", SafeLexicalInfo(node), typeName, name);
		}
		
		public static CompilerError MissingConstructor(Exception error, Node node, Type type, object[] parameters)
		{
			return new CompilerError("BCE0008", SafeLexicalInfo(node), error, type, GetSignature(parameters));
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
			return new CompilerError("BCE0010", SafeLexicalInfo(node), attributeType);
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
			return new CompilerError("BCE0014", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidNode(Node node)
		{
			return new CompilerError("BCE0015", SafeLexicalInfo(node), node);
		}
		
		public static CompilerError MethodArgumentCount(Node node, string name, int count)
		{
			return new CompilerError("BCE0016", SafeLexicalInfo(node), name, count);
		}
		
		public static CompilerError MethodSignature(Node node, string expectedSignature, string actualSignature)
		{
			return new CompilerError("BCE0017", SafeLexicalInfo(node), expectedSignature, actualSignature);
		}
		
		public static CompilerError NameNotType(Node node, string name)
		{
			return new CompilerError("BCE0018", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError MemberNotFound(MemberReferenceExpression node, string namespace_)
		{
			return new CompilerError("BCE0019", SafeLexicalInfo(node), node.Name, namespace_);
		}

		public static CompilerError InstanceRequired(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0020", SafeLexicalInfo(node), typeName, memberName);
		}
		
		public static CompilerError InvalidNamespace(Import import)
		{
			return new CompilerError("BCE0021", import.LexicalInfo, import.Namespace);
		}
		
		public static CompilerError IncompatibleExpressionType(Node node, string expectedType, string actualType)
		{
			return new CompilerError("BCE0022", SafeLexicalInfo(node), expectedType, actualType);
		}
		
		public static CompilerError NoApropriateOverloadFound(Node node, string signature, string memberName)
		{
			return new CompilerError("BCE0023", SafeLexicalInfo(node), signature, memberName);
		}
		
		public static CompilerError NoApropriateConstructorFound(Node node, string typeName, string signature)
		{
			return new CompilerError("BCE0024", SafeLexicalInfo(node), typeName, signature);
		}
		
		public static CompilerError InvalidArray(Node node)
		{
			return new CompilerError("BCE0025", SafeLexicalInfo(node));
		}
		
		public static CompilerError BoolExpressionRequired(Node node, string typeName)
		{
			return new CompilerError("BCE0026", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError NoEntryPoint()
		{
			return new CompilerError("BCE0028", LexicalInfo.Empty);
		}
		
		public static CompilerError MoreThanOneEntryPoint(Method method)
		{
			return new CompilerError("BCE0029", SafeLexicalInfo(method));
		}
		
		public static CompilerError NotImplemented(Node node, string message)
		{
			return new CompilerError("BCE0031", SafeLexicalInfo(node), message);
		}
		
		public static CompilerError EventArgumentMustBeAMethod(Node node, string eventName, string eventType)
		{
			return new CompilerError("BCE0032", SafeLexicalInfo(node), eventName, eventType);
		}
		
		public static CompilerError TypeNotAttribute(Node node, string attributeType)
		{
			return new CompilerError("BCE0033", SafeLexicalInfo(node), attributeType);
		}
		
		public static CompilerError ExpressionMustBeExecutedForItsSideEffects(Node node)
		{
			return new CompilerError("BCE0034", SafeLexicalInfo(node));
		}

		public static CompilerError ConflictWithInheritedMember(Node node, string member, string baseMember)
		{
			return new CompilerError("BCE0035", SafeLexicalInfo(node), member, baseMember);
		}
		
		public static CompilerError InvalidTypeof(Node node)
		{
			return new CompilerError("BCE0036", SafeLexicalInfo(node));
		}
		
		public static CompilerError UnknownMacro(Node node, string name)
		{
			return new CompilerError("BCE0037", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError InvalidMacro(Node node, string name)
		{
			return new CompilerError("BCE0038", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError AstMacroMustBeExternal(Node node, string typeName)
		{
			return new CompilerError("BCE0039", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError UnableToLoadAssembly(Node node, string name, Exception error)
		{
			return new CompilerError("BCE0041", SafeLexicalInfo(node), error, name);
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
			return new CompilerError("BCE0045", SafeLexicalInfo(node), error, error.Message);
		}
		
		public static CompilerError OperatorCantBeUsedWithValueType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0046", SafeLexicalInfo(node), operatorName, typeName);
		}
		
		public static CompilerError CantOverrideNonVirtual(Node node, string fullName)
		{
			return new CompilerError("BCE0047", SafeLexicalInfo(node), fullName);
		}
		
		public static CompilerError TypeDoesNotSupportSlicing(Node node, string fullName)
		{
			return new CompilerError("BCE0048", SafeLexicalInfo(node), fullName);
		}
		
		public static CompilerError LValueExpected(Node node)
		{
			return new CompilerError("BCE0049", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidOperatorForType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0050", SafeLexicalInfo(node), operatorName, typeName);
		}
		
		public static CompilerError InvalidOperatorForTypes(Node node, string operatorName, string lhs, string rhs)
		{
			return new CompilerError("BCE0051", SafeLexicalInfo(node), operatorName, lhs, rhs);
		}
		
		public static CompilerError InvalidLen(Node node, string typeName)
		{
			return new CompilerError("BCE0052", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError PropertyIsReadOnly(Node node, string propertyName)
		{
			return new CompilerError("BCE0053", SafeLexicalInfo(node), propertyName);
		}
		
		public static CompilerError IsaArgument(Node node)
		{
			return new CompilerError("BCE0054", SafeLexicalInfo(node));
		}

		public static CompilerError InternalError(Node node, Exception error)
		{
			string message = error != null ? error.Message : string.Empty;
			return InternalError(node, message, error);
		}

		public static CompilerError InternalError(Node node, string message, Exception cause)
		{
			return new CompilerError("BCE0055", SafeLexicalInfo(node), cause, message);
		}

		public static CompilerError FileNotFound(string fname)
		{
			return new CompilerError("BCE0056", new LexicalInfo(fname), fname);
		}
		
		public static CompilerError CantRedefinePrimitive(Node node, string name)
		{
			return new CompilerError("BCE0057", SafeLexicalInfo(node), name);
		}

		public static CompilerError ObjectRequired(Node node)
		{
			return new CompilerError("BCE0058", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidLockMacroArguments(Node node)
		{
			return new CompilerError("BCE0059", SafeLexicalInfo(node));
		}
		
		public static CompilerError NoMethodToOverride(Node node, string signature)
		{
			return new CompilerError("BCE0060", SafeLexicalInfo(node), signature);
		}
		
		public static CompilerError MethodIsNotOverride(Node node, string signature)
		{
			return new CompilerError("BCE0061", SafeLexicalInfo(node), signature);
		}
		
		public static CompilerError CouldNotInferReturnType(Node node, string signature)
		{
			return new CompilerError("BCE0062", SafeLexicalInfo(node), signature);
		}
		
		public static CompilerError NoEnclosingLoop(Node node)
		{
			return new CompilerError("BCE0063", SafeLexicalInfo(node));
		}
		
		public static CompilerError UnknownAttribute(Node node, string attributeName)
		{
			return new CompilerError("BCE0064", SafeLexicalInfo(node), attributeName);
		}
		
		public static CompilerError InvalidIteratorType(Node node, string typeName)
		{
			return new CompilerError("BCE0065", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError InvalidNodeForAttribute(LexicalInfo info, string attributeName, string expectedNodeTypes)
		{
			return new CompilerError("BCE0066", info, attributeName, expectedNodeTypes);
		}
		
		public static CompilerError LocalAlreadyExists(Node node, string name)
		{
			return new CompilerError("BCE0067", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError PropertyRequiresParameters(Node node, string name)
		{
			return new CompilerError("BCE0068", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError InterfaceCanOnlyInheritFromInterface(Node node, string interfaceName, string baseType)
		{
			return new CompilerError("BCE0069", SafeLexicalInfo(node), interfaceName, baseType);
		}
		
		public static CompilerError UnresolvedDependency(Node node, string source, string target)
		{
			return new CompilerError("BCE0070", SafeLexicalInfo(node), source, target);
		}
		
		public static CompilerError InheritanceCycle(Node node, string typeName)
		{
			return new CompilerError("BCE0071", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError InvalidOverrideReturnType(Node node, string methodName, string expectedReturnType, string actualReturnType)
		{
			return new CompilerError("BCE0072", SafeLexicalInfo(node), methodName, expectedReturnType, actualReturnType);
		}
		
		public static CompilerError AbstractMethodCantHaveBody(Node node, string methodName)
		{
			return new CompilerError("BCE0073", SafeLexicalInfo(node), methodName);
		}
		
		public static CompilerError SelfOutsideMethod(Node node)
		{
			return new CompilerError("BCE0074", SafeLexicalInfo(node));
		}
		
		public static CompilerError NamespaceIsNotAnExpression(Node node, string name)
		{
			return new CompilerError("BCE0075", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError RuntimeMethodBodyMustBeEmpty(Node node, string name)
		{
			return new CompilerError("BCE0076", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError TypeIsNotCallable(Node node, string name)
		{
			return new CompilerError("BCE0077", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError MethodReferenceExpected(Node node)
		{
			return new CompilerError("BCE0078", SafeLexicalInfo(node));
		}
		
		public static CompilerError AddressOfOutsideDelegateConstructor(Node node)
		{
			return new CompilerError("BCE0079", SafeLexicalInfo(node));
		}
		
		public static CompilerError BuiltinCannotBeUsedAsExpression(Node node, string name)
		{
			return new CompilerError("BCE0080", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError ReRaiseOutsideExceptionHandler(Node node)
		{
			return new CompilerError("BCE0081", SafeLexicalInfo(node));
		}
		
		public static CompilerError EventTypeIsNotCallable(Node node, string typeName)
		{
			return new CompilerError("BCE0082", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError StaticConstructorMustBePublic(Node node)
		{
			return new CompilerError("BCE0083", SafeLexicalInfo(node));
		}
		
		public static CompilerError StaticConstructorCannotDeclareParameters(Node node)
		{
			return new CompilerError("BCE0084", SafeLexicalInfo(node));
		}
		
		public static CompilerError CantCreateInstanceOfAbstractType(Node node, string typeName)
		{
			return new CompilerError("BCE0085", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CantCreateInstanceOfInterface(Node node, string typeName)
		{
			return new CompilerError("BCE0086", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CantCreateInstanceOfEnum(Node node, string typeName)
		{
			return new CompilerError("BCE0087", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError ReservedPrefix(Node node, string prefix)
		{
			return new CompilerError("BCE0088", SafeLexicalInfo(node), prefix);
		}
		
		public static CompilerError MemberNameConflict(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0089", SafeLexicalInfo(node), typeName, memberName);
		}
		
		public static CompilerError DerivedMethodCannotReduceAccess(Node node, string derivedMethod, string superMethod, TypeMemberModifiers derivedAccess, TypeMemberModifiers superAccess)
		{
			return new CompilerError("BCE0090", SafeLexicalInfo(node), derivedMethod, superMethod, superAccess.ToString().ToLower(), derivedAccess.ToString().ToLower());
		}
		
		public static CompilerError EventIsNotAnExpression(Node node, string eventName)
		{
			return new CompilerError("BCE0091", SafeLexicalInfo(node), eventName);
		}
		
		public static CompilerError InvalidRaiseArgument(Node node, string typeName)
		{
			return new CompilerError("BCE0092", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CannotBranchIntoEnsure(Node node)
		{
			return new CompilerError("BCE0093", SafeLexicalInfo(node));
		}
		
		public static CompilerError CannotBranchIntoExcept(Node node)
		{
			return new CompilerError("BCE0094", SafeLexicalInfo(node));
		}
		
		public static CompilerError NoSuchLabel(Node node, string label)
		{
			return new CompilerError("BCE0095", SafeLexicalInfo(node), label);
		}
		
		public static CompilerError LabelAlreadyDefined(Node node, string methodName, string label)
		{
			return new CompilerError("BCE0096", SafeLexicalInfo(node), methodName, label);
		}
		
		public static CompilerError CannotBranchIntoTry(Node node)
		{
			return new CompilerError("BCE0097", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidSwitch(Node node)
		{
			return new CompilerError("BCE0098", SafeLexicalInfo(node));
		}
		
		public static CompilerError YieldInsideTryBlock(Node node)
		{
			return new CompilerError("BCE0099", SafeLexicalInfo(node));
		}
		
		public static CompilerError YieldInsideConstructor(Node node)
		{
			return new CompilerError("BCE0100", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidGeneratorReturnType(Node node)
		{
			return new CompilerError("BCE0101", SafeLexicalInfo(node));
		}

		public static CompilerError GeneratorCantReturnValue(Node node)
		{
			return new CompilerError("BCE0102", SafeLexicalInfo(node));
		}
		
		public static CompilerError CannotExtendFinalType(Node node, string typeName)
		{
			return new CompilerError("BCE0103", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CantBeMarkedTransient(Node node)
		{
			return new CompilerError("BCE0104", SafeLexicalInfo(node));
		}
		
		public static CompilerError CantBeMarkedAbstract(Node node)
		{
			return new CompilerError("BCE0105", SafeLexicalInfo(node));
		}
		
		public static CompilerError FailedToLoadTypesFromAssembly(string assemblyName, Exception x)
		{
			return new CompilerError("BCE0106", LexicalInfo.Empty, x, assemblyName);
		}
		
		public static CompilerError ValueTypesCannotDeclareParameterlessConstructors(Node node)
		{
			return new CompilerError("BCE0107", SafeLexicalInfo(node));
		}
		
		public static CompilerError ValueTypeFieldsCannotHaveInitializers(Node node)
		{
			return new CompilerError("BCE0108", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidArrayRank(Node node, string arrayName, int real, int given)
		{
			return new CompilerError("BCE0109", SafeLexicalInfo(node), arrayName, real, given);
		}
		
		public static CompilerError NotANamespace(Node node, string name)
		{
			return new CompilerError("BCE0110", SafeLexicalInfo(node), name);
		}

		public static CompilerError InvalidDestructorModifier(Node node)
		{
			return new CompilerError("BCE0111", SafeLexicalInfo(node));
		}
		
		public static CompilerError CantHaveDestructorParameters(Node node)
		{
			return new CompilerError("BCE0112", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidCharLiteral(Node node, string value)
		{
			return new CompilerError("BCE0113", SafeLexicalInfo(node), value);
		}
		
		public static CompilerError InvalidInterfaceForInterfaceMember(Node node, string value)
		{
			return new CompilerError("BCE0114", SafeLexicalInfo(node), value);
		}

		public static CompilerError InterfaceImplForInvalidInterface(Node node, string iface, string item)
		{
			return new CompilerError("BCE0115", SafeLexicalInfo(node), iface, item);
		}
		
		public static CompilerError ExplicitImplMustNotHaveModifiers(Node node, string iface, string item)
		{
			return new CompilerError("BCE0116", SafeLexicalInfo(node), iface, item);
		}

		public static CompilerError FieldIsReadonly(Node node, string name)
		{
			return new CompilerError("BCE0117", SafeLexicalInfo(node), name);
		}

		public static CompilerError ExplodedExpressionMustBeArray(Node node)
		{
			return new CompilerError("BCE0118", SafeLexicalInfo(node));
		}

		public static CompilerError ExplodeExpressionMustMatchVarArgCall(Node node)
		{
			return new CompilerError("BCE0119", SafeLexicalInfo(node));
		}

		public static CompilerError UnaccessibleMember(Node node, string name)
		{
			return new CompilerError("BCE0120", SafeLexicalInfo(node), name);
		}

		public static CompilerError InvalidSuper(Node node)
		{
			return new CompilerError("BCE0121", SafeLexicalInfo(node));
		}

		public static CompilerError ValueTypeCantHaveAbstractMember(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0122", SafeLexicalInfo(node), typeName, memberName);
		}

		public static CompilerError InvalidParameterType(Node node, string typeName)
		{
			return new CompilerError("BCE0123", SafeLexicalInfo(node), typeName);
		}

		public static CompilerError InvalidFieldType(Node node, string typeName)
		{
			return new CompilerError("BCE0124", SafeLexicalInfo(node), typeName);
		}

		public static CompilerError InvalidDeclarationType(Node node, string typeName)
		{
			return new CompilerError("BCE0125", SafeLexicalInfo(node), typeName);
		}

		public static CompilerError InvalidExpressionType(Node node, string typeName)
		{
			return new CompilerError("BCE0126", SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError RefArgTakesLValue(Node node)
		{
			return new CompilerError("BCE0127", SafeLexicalInfo(node), node.ToString());
		}

		public static CompilerError InvalidTryStatement(Node node)
		{
			return new CompilerError("BCE0128", SafeLexicalInfo(node));
		}

		public static CompilerError InvalidExtensionDefinition(Node node)
		{
			return new CompilerError("BCE0129", SafeLexicalInfo(node));
		}
		
		public static CompilerError CantBeMarkedPartial(Node node)
		{
			return new CompilerError("BCE0130", SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidCombinationOfModifiers(Node node, string name, string modifiers)
		{
			return new CompilerError("BCE0131", SafeLexicalInfo(node), name, modifiers);
		}
		
		public static CompilerError NamespaceAlreadyContainsMember(Node node, string container, string member)
		{
			return new CompilerError("BCE0132", SafeLexicalInfo(node), container, member);
		}

		public static CompilerError InvalidEntryPoint(Node node)
		{
			return new CompilerError("BCE0133", SafeLexicalInfo(node));
		}

		public static CompilerError CannotReturnValue(Method node)
		{
			return new CompilerError("BCE0134", SafeLexicalInfo(node), node);
		}
		
		public static CompilerError InvalidName(Node node, string name)
		{
			return new CompilerError("BCE0135", SafeLexicalInfo(node), name);
		}
		
		public static CompilerError ColonInsteadOfEquals(Node node)
		{
			return new CompilerError("BCE0136", SafeLexicalInfo(node));
		}
		
		public static CompilerError PropertyIsWriteOnly(Node node, string propertyName)
		{
			return new CompilerError("BCE0137", SafeLexicalInfo(node), propertyName);
		}

		public static CompilerError NotAGenericDefinition(Node node, string name)
		{
			return new CompilerError("BCE0138", SafeLexicalInfo(node), name);
		}

		public static CompilerError GenericDefinitionArgumentCount(Node node, string name, int expectedCount)
		{
			return new CompilerError("BCE0139", SafeLexicalInfo(node), name, expectedCount);
		}
				
		public static CompilerError YieldTypeDoesNotMatchReturnType(Node node, string yieldType, string returnType)
		{
			return new CompilerError("BCE0140", SafeLexicalInfo(node), yieldType, returnType);
		}
		
		public static CompilerError DuplicateParameterName(Node node, string parameter, string method)
		{
			return new CompilerError("BCE0141", SafeLexicalInfo(node), parameter, method);
		}
		
		public static CompilerError ValueTypeParameterCannotUseDefaultAttribute(Node node, string parameter)
		{
			string method = (null != node as Method) ? (node as Method).Name : (node as Property).Name;  
			return new CompilerError("BCE0142", SafeLexicalInfo(node), parameter, method);
		}
		
		public static CompilerError CantReturnFromEnsure(Node node)
		{
			return new CompilerError("BCE0143", SafeLexicalInfo(node));
		}
		
		public static CompilerError Obsolete(Node node, string memberName, string message)
		{
			return new CompilerError("BCE0144", SafeLexicalInfo(node), memberName, message);
		}

		public static CompilerError InvalidExceptArgument(Node node, string exceptionType)
		{
			return new CompilerError("BCE0145", SafeLexicalInfo(node), exceptionType);
		}
		
		public static CompilerError GenericArgumentMustBeReferenceType(Node node, IGenericParameter parameter, IType argument)
		{
			return new CompilerError("BCE0146", SafeLexicalInfo(node), argument, parameter);
		}

		public static CompilerError GenericArgumentMustBeValueType(Node node, IGenericParameter parameter, IType argument)
		{
			return new CompilerError("BCE0147", SafeLexicalInfo(node), argument, parameter);
		}

		public static CompilerError GenericArgumentMustHaveDefaultConstructor(Node node, IGenericParameter parameter, IType argument)
		{
			return new CompilerError("BCE0148", SafeLexicalInfo(node), argument, parameter);
		}

		public static CompilerError GenericArgumentMustHaveBaseType(Node node, IGenericParameter parameter, IType argument, IType baseType)
		{
			return new CompilerError("BCE0149", SafeLexicalInfo(node), argument, parameter, baseType);
		}
		
		public static CompilerError CantBeMarkedFinal(Node node)
		{
			return new CompilerError("BCE0150", SafeLexicalInfo(node));
		}
		
		public static CompilerError CantBeMarkedStatic(Node node)
		{
			return new CompilerError("BCE0151", SafeLexicalInfo(node));
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

		private static LexicalInfo SafeLexicalInfo(Node node)
		{
			if (null == node) return Boo.Lang.Compiler.Ast.LexicalInfo.Empty;
			LexicalInfo info = node.LexicalInfo;
			if (info.IsValid) return info;
			return SafeLexicalInfo(node.ParentNode);
		}

	}
}
