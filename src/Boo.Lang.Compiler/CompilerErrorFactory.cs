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
	
	public static class CompilerErrorFactory
	{	
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
			return new CompilerError("BCE0001", AstUtil.SafeLexicalInfo(node), className, baseType);
		}
		
		public static CompilerError NamedParameterMustBeIdentifier(ExpressionPair pair)
		{
			return new CompilerError("BCE0002", AstUtil.SafeLexicalInfo(pair.First));
		}
		
		public static CompilerError NamedArgumentsNotAllowed(Node node)
		{
			return new CompilerError("BCE0003", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError AmbiguousReference(ReferenceExpression reference, System.Reflection.MemberInfo[] members)
		{
			return new CompilerError("BCE0004",
									  AstUtil.SafeLexicalInfo(reference),
									  reference.Name,
									  ToNameList(members)
									  );
		}
		
		public static CompilerError AmbiguousReference(Node node, string name, System.Collections.IEnumerable names)
		{
			return new CompilerError("BCE0004", AstUtil.SafeLexicalInfo(node), name, ToStringList(names));
		}
		
		public static CompilerError UnknownIdentifier(Node node, string name)
		{
			return new CompilerError("BCE0005", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError CantCastToValueType(Node node, string typeName)
		{
			return new CompilerError("BCE0006", AstUtil.SafeLexicalInfo(node), typeName);
		}

		public static CompilerError NotAPublicFieldOrProperty(Node node, string name, string typeName)
		{
			return new CompilerError("BCE0007", AstUtil.SafeLexicalInfo(node), typeName, name);
		}
		
		public static CompilerError MissingConstructor(Exception error, Node node, Type type, object[] parameters)
		{
			return new CompilerError("BCE0008", AstUtil.SafeLexicalInfo(node), error, type, GetSignature(parameters));
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
			return new CompilerError("BCE0010", AstUtil.SafeLexicalInfo(node), attributeType);
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
			return new CompilerError("BCE0014", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidNode(Node node)
		{
			return new CompilerError("BCE0015", AstUtil.SafeLexicalInfo(node), node);
		}
		
		public static CompilerError MethodArgumentCount(Node node, string name, int count)
		{
			return new CompilerError("BCE0016", AstUtil.SafeLexicalInfo(node), name, count);
		}
		
		public static CompilerError MethodSignature(Node node, string expectedSignature, string actualSignature)
		{
			return new CompilerError("BCE0017", AstUtil.SafeLexicalInfo(node), expectedSignature, actualSignature);
		}
		
		public static CompilerError NameNotType(Node node, string typeName, string whatItIs, string suggestion)
		{
			return new CompilerError("BCE0018", AstUtil.SafeLexicalInfo(node), typeName, whatItIs, DidYouMeanOrNull(suggestion));
		}
		
		public static CompilerError MemberNotFound(MemberReferenceExpression node, string namespace_, string suggestion)
		{
			return new CompilerError("BCE0019", AstUtil.SafeLexicalInfo(node), node.Name, namespace_, DidYouMeanOrNull(suggestion));
		}

		public static CompilerError InstanceRequired(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0020", AstUtil.SafeLexicalInfo(node), typeName, memberName);
		}
		
		public static CompilerError InvalidNamespace(Import import)
		{
			if (import.AssemblyReference != null)
				return new CompilerError("BCE0167", import.LexicalInfo, import.Namespace, import.AssemblyReference);
			return new CompilerError("BCE0021", import.LexicalInfo, import.Namespace);
		}
		
		public static CompilerError IncompatibleExpressionType(Node node, string expectedType, string actualType)
		{
			return new CompilerError("BCE0022", AstUtil.SafeLexicalInfo(node), expectedType, actualType);
		}
		
		public static CompilerError NoApropriateOverloadFound(Node node, string signature, string memberName)
		{
			return new CompilerError("BCE0023", AstUtil.SafeLexicalInfo(node), signature, memberName);
		}
		
		public static CompilerError NoApropriateConstructorFound(Node node, string typeName, string signature)
		{
			return new CompilerError("BCE0024", AstUtil.SafeLexicalInfo(node), typeName, signature);
		}
		
		public static CompilerError InvalidArray(Node node)
		{
			return new CompilerError("BCE0025", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError BoolExpressionRequired(Node node, string typeName)
		{
			return new CompilerError("BCE0026", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError NoEntryPoint()
		{
			return new CompilerError("BCE0028", LexicalInfo.Empty);
		}
		
		public static CompilerError MoreThanOneEntryPoint(Method method)
		{
			return new CompilerError("BCE0029", AstUtil.SafeLexicalInfo(method));
		}
		
		public static CompilerError NotImplemented(Node node, string message)
		{
			return new CompilerError("BCE0031", AstUtil.SafeLexicalInfo(node), message);
		}
		
		public static CompilerError EventArgumentMustBeAMethod(Node node, string eventName, string eventType)
		{
			return new CompilerError("BCE0032", AstUtil.SafeLexicalInfo(node), eventName, eventType);
		}
		
		public static CompilerError TypeNotAttribute(Node node, string attributeType)
		{
			return new CompilerError("BCE0033", AstUtil.SafeLexicalInfo(node), attributeType);
		}
		
		public static CompilerError ExpressionMustBeExecutedForItsSideEffects(Node node)
		{
			return new CompilerError("BCE0034", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError ConflictWithInheritedMember(Node node, string member, string baseMember)
		{
			return new CompilerError("BCE0035", AstUtil.SafeLexicalInfo(node), member, baseMember);
		}
		
		public static CompilerError InvalidTypeof(Node node)
		{
			return new CompilerError("BCE0036", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError UnknownMacro(Node node, string name)
		{
			return new CompilerError("BCE0037", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError InvalidMacro(Node node, string name)
		{
			return new CompilerError("BCE0038", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError AstMacroMustBeExternal(Node node, string typeName)
		{
			return new CompilerError("BCE0039", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError UnableToLoadAssembly(Node node, string name, Exception error)
		{
			return new CompilerError("BCE0041", AstUtil.SafeLexicalInfo(node), error, name);
		}
		
		public static CompilerError InputError(string inputName, Exception error)
		{
			return InputError(new LexicalInfo(inputName), error);
		}

		public static CompilerError InputError(LexicalInfo lexicalInfo, Exception error)
		{
			return new CompilerError("BCE0042", lexicalInfo, error, lexicalInfo.FileName, error.Message);
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
			return new CompilerError("BCE0045", AstUtil.SafeLexicalInfo(node), error, error.Message);
		}

		public static CompilerError MacroExpansionError(Node node)
		{
			return new CompilerError("BCE0045", AstUtil.SafeLexicalInfo(node), ResourceManager.Format("BooC.InvalidNestedMacroContext"));
		}

		public static CompilerError OperatorCantBeUsedWithValueType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0046", AstUtil.SafeLexicalInfo(node), operatorName, typeName);
		}
		
		public static CompilerError CantOverrideNonVirtual(Node node, string fullName)
		{
			return new CompilerError("BCE0047", AstUtil.SafeLexicalInfo(node), fullName);
		}
		
		public static CompilerError TypeDoesNotSupportSlicing(Node node, string fullName)
		{
			return new CompilerError("BCE0048", AstUtil.SafeLexicalInfo(node), fullName);
		}
		
		public static CompilerError LValueExpected(Node node)
		{
			return new CompilerError("BCE0049", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidOperatorForType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0050", AstUtil.SafeLexicalInfo(node), operatorName, typeName);
		}
		
		public static CompilerError InvalidOperatorForTypes(Node node, string operatorName, string lhs, string rhs)
		{
			return new CompilerError("BCE0051", AstUtil.SafeLexicalInfo(node), operatorName, lhs, rhs);
		}
		
		public static CompilerError InvalidLen(Node node, string typeName)
		{
			return new CompilerError("BCE0052", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError PropertyIsReadOnly(Node node, string propertyName)
		{
			return new CompilerError("BCE0053", AstUtil.SafeLexicalInfo(node), propertyName);
		}
		
		public static CompilerError IsaArgument(Node node)
		{
			return new CompilerError("BCE0054", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError InternalError(Node node, Exception error)
		{
			string message = error != null ? error.Message : string.Empty;
			return InternalError(node, message, error);
		}

		public static CompilerError InternalError(Node node, string message, Exception cause)
		{
			return new CompilerError("BCE0055", AstUtil.SafeLexicalInfo(node), cause, message);
		}

		public static CompilerError FileNotFound(string fname)
		{
			return new CompilerError("BCE0056", new LexicalInfo(fname), fname);
		}
		
		public static CompilerError CantRedefinePrimitive(Node node, string name)
		{
			return new CompilerError("BCE0057", AstUtil.SafeLexicalInfo(node), name);
		}

		public static CompilerError SelfIsNotValidInStaticMember(Node node)
		{
			return new CompilerError("BCE0058", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidLockMacroArguments(Node node)
		{
			return new CompilerError("BCE0059", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError NoMethodToOverride(Node node, string signature, bool incompatibleSignature)
		{
			return new CompilerError("BCE0060", AstUtil.SafeLexicalInfo(node), signature,
				incompatibleSignature ? ResourceManager.Format("BCE0060.IncompatibleSignature") : null);
		}
		
		public static CompilerError NoMethodToOverride(Node node, string signature, string suggestion)
		{
			return new CompilerError("BCE0060", AstUtil.SafeLexicalInfo(node), signature, DidYouMeanOrNull(suggestion));
		}
		
		public static CompilerError MethodIsNotOverride(Node node, string signature)
		{
			return new CompilerError("BCE0061", AstUtil.SafeLexicalInfo(node), signature);
		}
		
		public static CompilerError CouldNotInferReturnType(Node node, string signature)
		{
			return new CompilerError("BCE0062", AstUtil.SafeLexicalInfo(node), signature);
		}
		
		public static CompilerError NoEnclosingLoop(Node node)
		{
			return new CompilerError("BCE0063", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError UnknownAttribute(Node node, string attributeName, string suggestion)
		{
			return new CompilerError("BCE0064", AstUtil.SafeLexicalInfo(node), attributeName, DidYouMeanOrNull(suggestion));
		}
		
		public static CompilerError InvalidIteratorType(Node node, IType type)
		{
			return new CompilerError("BCE0065", AstUtil.SafeLexicalInfo(node), type);
		}
		
		public static CompilerError InvalidNodeForAttribute(LexicalInfo info, string attributeName, string expectedNodeTypes)
		{
			return new CompilerError("BCE0066", info, attributeName, expectedNodeTypes);
		}
		
		public static CompilerError LocalAlreadyExists(Node node, string name)
		{
			return new CompilerError("BCE0067", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError PropertyRequiresParameters(Node node, string name)
		{
			return new CompilerError("BCE0068", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError InterfaceCanOnlyInheritFromInterface(Node node, string interfaceName, string baseType)
		{
			return new CompilerError("BCE0069", AstUtil.SafeLexicalInfo(node), interfaceName, baseType);
		}
		
		public static CompilerError UnresolvedDependency(Node node, string source, string target)
		{
			return new CompilerError("BCE0070", AstUtil.SafeLexicalInfo(node), source, target);
		}
		
		public static CompilerError InheritanceCycle(Node node, string typeName)
		{
			return new CompilerError("BCE0071", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError InvalidOverrideReturnType(Node node, string methodName, string expectedReturnType, string actualReturnType)
		{
			return new CompilerError("BCE0072", AstUtil.SafeLexicalInfo(node), methodName, expectedReturnType, actualReturnType);
		}
		
		public static CompilerError AbstractMethodCantHaveBody(Node node, string methodName)
		{
			return new CompilerError("BCE0073", AstUtil.SafeLexicalInfo(node), methodName);
		}
		
		public static CompilerError SelfOutsideMethod(Node node)
		{
			return new CompilerError("BCE0074", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError NamespaceIsNotAnExpression(Node node, string name)
		{
			return new CompilerError("BCE0075", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError RuntimeMethodBodyMustBeEmpty(Node node, string name)
		{
			return new CompilerError("BCE0076", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError TypeIsNotCallable(Node node, string name)
		{
			return new CompilerError("BCE0077", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError MethodReferenceExpected(Node node)
		{
			return new CompilerError("BCE0078", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError AddressOfOutsideDelegateConstructor(Node node)
		{
			return new CompilerError("BCE0079", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError BuiltinCannotBeUsedAsExpression(Node node, string name)
		{
			return new CompilerError("BCE0080", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError ReRaiseOutsideExceptionHandler(Node node)
		{
			return new CompilerError("BCE0081", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError EventTypeIsNotCallable(Node node, string typeName)
		{
			return new CompilerError("BCE0082", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError StaticConstructorMustBePrivate(Node node)
		{
			return new CompilerError("BCE0083", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError StaticConstructorCannotDeclareParameters(Node node)
		{
			return new CompilerError("BCE0084", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CantCreateInstanceOfAbstractType(Node node, string typeName)
		{
			return new CompilerError("BCE0085", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CantCreateInstanceOfInterface(Node node, string typeName)
		{
			return new CompilerError("BCE0086", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CantCreateInstanceOfEnum(Node node, string typeName)
		{
			return new CompilerError("BCE0087", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError ReservedPrefix(Node node, string prefix)
		{
			return new CompilerError("BCE0088", AstUtil.SafeLexicalInfo(node), prefix);
		}
		
		public static CompilerError MemberNameConflict(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0089", AstUtil.SafeLexicalInfo(node), typeName, memberName);
		}
		
		public static CompilerError DerivedMethodCannotReduceAccess(Node node, string derivedMethod, string superMethod, TypeMemberModifiers derivedAccess, TypeMemberModifiers superAccess)
		{
			return new CompilerError("BCE0090", AstUtil.SafeLexicalInfo(node), derivedMethod, superMethod, superAccess.ToString().ToLower(), derivedAccess.ToString().ToLower());
		}
		
		public static CompilerError EventIsNotAnExpression(Node node, string eventName)
		{
			return new CompilerError("BCE0091", AstUtil.SafeLexicalInfo(node), eventName);
		}
		
		public static CompilerError InvalidRaiseArgument(Node node, string typeName)
		{
			return new CompilerError("BCE0092", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CannotBranchIntoEnsure(Node node)
		{
			return new CompilerError("BCE0093", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CannotBranchIntoExcept(Node node)
		{
			return new CompilerError("BCE0094", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError NoSuchLabel(Node node, string label)
		{
			return new CompilerError("BCE0095", AstUtil.SafeLexicalInfo(node), label);
		}
		
		public static CompilerError LabelAlreadyDefined(Node node, string methodName, string label)
		{
			return new CompilerError("BCE0096", AstUtil.SafeLexicalInfo(node), methodName, label);
		}
		
		public static CompilerError CannotBranchIntoTry(Node node)
		{
			return new CompilerError("BCE0097", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidSwitch(Node node)
		{
			return new CompilerError("BCE0098", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError YieldInsideTryExceptOrEnsureBlock(Node node)
		{
			return new CompilerError("BCE0099", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError YieldInsideConstructor(Node node)
		{
			return new CompilerError("BCE0100", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidGeneratorReturnType(TypeReference type)
		{
			return new CompilerError("BCE0101", AstUtil.SafeLexicalInfo(type), type);
		}

		public static CompilerError GeneratorCantReturnValue(Node node)
		{
			return new CompilerError("BCE0102", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CannotExtendFinalType(Node node, string typeName)
		{
			return new CompilerError("BCE0103", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError CantBeMarkedTransient(Node node)
		{
			return new CompilerError("BCE0104", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CantBeMarkedAbstract(Node node)
		{
			return new CompilerError("BCE0105", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError FailedToLoadTypesFromAssembly(string assemblyName, Exception x)
		{
			return new CompilerError("BCE0106", LexicalInfo.Empty, x, assemblyName);
		}
		
		public static CompilerError ValueTypesCannotDeclareParameterlessConstructors(Node node)
		{
			return new CompilerError("BCE0107", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError ValueTypeFieldsCannotHaveInitializers(Node node)
		{
			return new CompilerError("BCE0108", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidArrayRank(Node node, string arrayName, int real, int given)
		{
			return new CompilerError("BCE0109", AstUtil.SafeLexicalInfo(node), arrayName, real, given);
		}
		
		public static CompilerError NotANamespace(Node node, string name)
		{
			return new CompilerError("BCE0110", AstUtil.SafeLexicalInfo(node), name);
		}

		public static CompilerError InvalidDestructorModifier(Node node)
		{
			return new CompilerError("BCE0111", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CantHaveDestructorParameters(Node node)
		{
			return new CompilerError("BCE0112", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidCharLiteral(Node node, string value)
		{
			return new CompilerError("BCE0113", AstUtil.SafeLexicalInfo(node), value);
		}
		
		public static CompilerError InvalidInterfaceForInterfaceMember(Node node, string value)
		{
			return new CompilerError("BCE0114", AstUtil.SafeLexicalInfo(node), value);
		}

		public static CompilerError InterfaceImplForInvalidInterface(Node node, string iface, string item)
		{
			return new CompilerError("BCE0115", AstUtil.SafeLexicalInfo(node), iface, item);
		}
		
		public static CompilerError ExplicitImplMustNotHaveModifiers(Node node, string iface, string item)
		{
			return new CompilerError("BCE0116", AstUtil.SafeLexicalInfo(node), iface, item);
		}

		public static CompilerError FieldIsReadonly(Node node, string name)
		{
			return new CompilerError("BCE0117", AstUtil.SafeLexicalInfo(node), name);
		}

		public static CompilerError ExplodedExpressionMustBeArray(Node node)
		{
			return new CompilerError("BCE0118", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError ExplodeExpressionMustMatchVarArgCall(Node node)
		{
			return new CompilerError("BCE0119", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError UnaccessibleMember(Node node, string name)
		{
			return new CompilerError("BCE0120", AstUtil.SafeLexicalInfo(node), name);
		}

		public static CompilerError InvalidSuper(Node node)
		{
			return new CompilerError("BCE0121", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError ValueTypeCantHaveAbstractMember(Node node, string typeName, string memberName)
		{
			return new CompilerError("BCE0122", AstUtil.SafeLexicalInfo(node), typeName, memberName);
		}

		public static CompilerError InvalidParameterType(Node node, string typeName)
		{
			return new CompilerError("BCE0123", AstUtil.SafeLexicalInfo(node), typeName, string.Empty);
		}

		public static CompilerError InvalidGenericParameterType(Node node, IType type)
		{
			return new CompilerError("BCE0123", AstUtil.SafeLexicalInfo(node), type, "generic ");
		}

		public static CompilerError InvalidFieldType(Node node, string typeName)
		{
			return new CompilerError("BCE0124", AstUtil.SafeLexicalInfo(node), typeName);
		}

		public static CompilerError InvalidDeclarationType(Node node, string typeName)
		{
			return new CompilerError("BCE0125", AstUtil.SafeLexicalInfo(node), typeName);
		}

		public static CompilerError InvalidExpressionType(Node node, string typeName)
		{
			return new CompilerError("BCE0126", AstUtil.SafeLexicalInfo(node), typeName);
		}
		
		public static CompilerError RefArgTakesLValue(Node node)
		{
			return new CompilerError("BCE0127", AstUtil.SafeLexicalInfo(node), node.ToString());
		}

		public static CompilerError InvalidTryStatement(Node node)
		{
			return new CompilerError("BCE0128", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError InvalidExtensionDefinition(Node node)
		{
			return new CompilerError("BCE0129", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CantBeMarkedPartial(Node node)
		{
			return new CompilerError("BCE0130", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError InvalidCombinationOfModifiers(Node node, string name, string modifiers)
		{
			return new CompilerError("BCE0131", AstUtil.SafeLexicalInfo(node), name, modifiers);
		}
		
		public static CompilerError NamespaceAlreadyContainsMember(Node node, string container, string member)
		{
			return new CompilerError("BCE0132", AstUtil.SafeLexicalInfo(node), container, member);
		}

		public static CompilerError InvalidEntryPoint(Node node)
		{
			return new CompilerError("BCE0133", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError CannotReturnValue(Method node)
		{
			return new CompilerError("BCE0134", AstUtil.SafeLexicalInfo(node), node);
		}
		
		public static CompilerError InvalidName(Node node, string name)
		{
			return new CompilerError("BCE0135", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerError ColonInsteadOfEquals(Node node)
		{
			return new CompilerError("BCE0136", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError PropertyIsWriteOnly(Node node, string propertyName)
		{
			return new CompilerError("BCE0137", AstUtil.SafeLexicalInfo(node), propertyName);
		}

		public static CompilerError NotAGenericDefinition(Node node, string name)
		{
			return new CompilerError("BCE0138", AstUtil.SafeLexicalInfo(node), name);
		}

		public static CompilerError GenericDefinitionArgumentCount(Node node, string name, int expectedCount)
		{
			return new CompilerError("BCE0139", AstUtil.SafeLexicalInfo(node), name, expectedCount);
		}
				
		public static CompilerError YieldTypeDoesNotMatchReturnType(Node node, string yieldType, string returnType)
		{
			return new CompilerError("BCE0140", AstUtil.SafeLexicalInfo(node), yieldType, returnType);
		}
		
		public static CompilerError DuplicateParameterName(Node node, string parameter, string method)
		{
			return new CompilerError("BCE0141", AstUtil.SafeLexicalInfo(node), parameter, method);
		}
		
		public static CompilerError ValueTypeParameterCannotUseDefaultAttribute(Node node, string parameter)
		{
			string method = (node is Method) ? ((Method) node).Name : ((Property) node).Name;
			return new CompilerError("BCE0142", AstUtil.SafeLexicalInfo(node), parameter, method);
		}
		
		public static CompilerError CantReturnFromEnsure(Node node)
		{
			return new CompilerError("BCE0143", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError Obsolete(Node node, string memberName, string message)
		{
			return new CompilerError("BCE0144", AstUtil.SafeLexicalInfo(node), memberName, message);
		}

		public static CompilerError InvalidExceptArgument(Node node, string exceptionType)
		{
			return new CompilerError("BCE0145", AstUtil.SafeLexicalInfo(node), exceptionType);
		}
		
		public static CompilerError GenericArgumentMustBeReferenceType(Node node, IGenericParameter parameter, IType argument)
		{
			return new CompilerError("BCE0146", AstUtil.SafeLexicalInfo(node), argument, parameter, parameter.DeclaringEntity);
		}

		public static CompilerError GenericArgumentMustBeValueType(Node node, IGenericParameter parameter, IType argument)
		{
			return new CompilerError("BCE0147", AstUtil.SafeLexicalInfo(node), argument, parameter, parameter.DeclaringEntity);
		}

		public static CompilerError GenericArgumentMustHaveDefaultConstructor(Node node, IGenericParameter parameter, IType argument)
		{
			return new CompilerError("BCE0148", AstUtil.SafeLexicalInfo(node), argument, parameter, parameter.DeclaringEntity);
		}

		public static CompilerError GenericArgumentMustHaveBaseType(Node node, IGenericParameter parameter, IType argument, IType baseType)
		{
			return new CompilerError("BCE0149", AstUtil.SafeLexicalInfo(node), argument, baseType, parameter, parameter.DeclaringEntity);
		}
		
		public static CompilerError CantBeMarkedFinal(Node node)
		{
			return new CompilerError("BCE0150", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError CantBeMarkedStatic(Node node)
		{
			return new CompilerError("BCE0151", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerError ConstructorCantBePolymorphic(Node node, string memberName)
		{
			return new CompilerError("BCE0152", AstUtil.SafeLexicalInfo(node), memberName);
		}

		public static CompilerError InvalidAttributeTarget(Node node, Type attrType, AttributeTargets validOn)
		{
			return new CompilerError("BCE0153", AstUtil.SafeLexicalInfo(node), attrType, validOn);
		}

		public static CompilerError MultipleAttributeUsage(Node node, Type attrType)
		{
			return new CompilerError("BCE0154", AstUtil.SafeLexicalInfo(node), attrType);
		}

		public static CompilerError CannotCreateAnInstanceOfGenericParameterWithoutDefaultConstructorConstraint(Node node, string type)
		{
			return new CompilerError("BCE0155", AstUtil.SafeLexicalInfo(node), type);
		}

		public static CompilerError EventCanOnlyBeInvokedFromWithinDeclaringClass(Node node, IEvent ev)
		{
			return new CompilerError("BCE0156", AstUtil.SafeLexicalInfo(node), ev.Name, ev.DeclaringType);
		}
		
		public static CompilerError GenericTypesMustBeConstructedToBeInstantiated(Node node)
		{
			return new CompilerError("BCE0157", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerError InstanceMethodInvocationBeforeInitialization(Constructor ctor, MemberReferenceExpression mre)
		{
			return new CompilerError("BCE0158", AstUtil.SafeLexicalInfo(mre), mre.Name);
		}

		public static CompilerError StructAndClassConstraintsConflict(GenericParameterDeclaration gpd)
		{
			return new CompilerError("BCE0159", AstUtil.SafeLexicalInfo(gpd), gpd.Name);
		}

		public static CompilerError StructAndConstructorConstraintsConflict(GenericParameterDeclaration gpd)
		{
			return new CompilerError("BCE0160", AstUtil.SafeLexicalInfo(gpd), gpd.Name);
		}

		public static CompilerError TypeConstraintConflictsWithSpecialConstraint(GenericParameterDeclaration gpd, TypeReference type, string constraint)
		{
			return new CompilerError("BCE0161", AstUtil.SafeLexicalInfo(type), gpd.Name, type, constraint);
		}

		public static CompilerError InvalidTypeConstraint(GenericParameterDeclaration gpd, TypeReference type)
		{
			return new CompilerError("BCE0162", AstUtil.SafeLexicalInfo(type), gpd.Name, type);
		}

		public static CompilerError MultipleBaseTypeConstraints(GenericParameterDeclaration gpd, TypeReference type, TypeReference other)
		{
			return new CompilerError("BCE0163", AstUtil.SafeLexicalInfo(type), gpd.Name, type, other);
		}

		public static CompilerError CannotInferGenericMethodArguments(MethodInvocationExpression node, IMethod method)
		{
			return new CompilerError("BCE0164", AstUtil.SafeLexicalInfo(node), method);
		}

		public static CompilerError ExceptionAlreadyHandled(ExceptionHandler dupe, ExceptionHandler previous)
		{
			return new CompilerError("BCE0165", AstUtil.SafeLexicalInfo(dupe.Declaration), dupe.Declaration.Type, previous.Declaration.Type, AstUtil.SafePositionOnlyLexicalInfo(previous.Declaration));
		}

		public static CompilerError UnknownClassMacroWithFieldHint(MacroStatement node, string name)
		{
			return new CompilerError("BCE0166", AstUtil.SafeLexicalInfo(node), name);
		}

		public static CompilerError PointerIncompatibleType(Node node, IType type)
		{
			return new CompilerError("BCE0168", AstUtil.SafeLexicalInfo(node), type);
		}

		public static CompilerError NotAMemberOfExplicitInterface(TypeMember member, IType type)
		{
			return new CompilerError("BCE0169", AstUtil.SafeLexicalInfo(member), member.Name, type);
		}

		public static CompilerError EnumMemberMustBeConstant(EnumMember member)
		{
			return new CompilerError("BCE0170", AstUtil.SafeLexicalInfo(member), member.FullName);
		}

		public static CompilerError ConstantCannotBeConverted(Node node, IType type)
		{
			return new CompilerError("BCE0171", AstUtil.SafeLexicalInfo(node), node, type);
		}

		public static CompilerError InterfaceImplementationMustBePublicOrExplicit(TypeMember node, IMember member)
		{
			return new CompilerError("BCE0172", AstUtil.SafeLexicalInfo(node), member.DeclaringType);
		}

		public static CompilerError InvalidRegexOption(RELiteralExpression node, char option)
		{
			return new CompilerError("BCE0173", AstUtil.SafeLexicalInfo(node), option);
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

		private static string DidYouMeanOrNull(string suggestion)
		{
			return (null != suggestion)
				? ResourceManager.Format("BooC.DidYouMean", suggestion)
				: null;
		}
	}
}
