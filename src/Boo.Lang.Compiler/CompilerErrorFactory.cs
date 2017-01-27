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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;
using Boo.Lang.Resources;

namespace Boo.Lang.Compiler
{
	public static class CompilerErrorFactory
	{	
		public static CompilerError CustomError(Node anchor, string msg)
		{
			return CustomError(AstUtil.SafeLexicalInfo(anchor), msg);
		}
		
		public static CompilerError CustomError(LexicalInfo lexicalInfo, string msg)
		{
			return new CompilerError(lexicalInfo, msg);
		}
		
		public static CompilerError CustomError(string msg)
		{
			return new CompilerError(msg);
		}
		
		public static CompilerError ClassAlreadyHasBaseType(Node node, string className, IType baseType)
		{
			return Instantiate("BCE0001", node, className, baseType);
		}
		
		public static CompilerError NamedParameterMustBeIdentifier(ExpressionPair pair)
		{
			return Instantiate("BCE0002", pair.First);
		}
		
		public static CompilerError NamedArgumentsNotAllowed(Node node)
		{
			return Instantiate("BCE0003", node);
		}
		
		public static CompilerError AmbiguousReference(ReferenceExpression reference, System.Reflection.MemberInfo[] members)
		{
			return Instantiate("BCE0004", reference, reference.Name, ToNameList(members));
		}
		
		public static CompilerError AmbiguousReference(Node node, string name, IEnumerable<IEntity> entities)
		{
			return Instantiate("BCE0004", node, name, ToStringList(entities));
		}
		
		public static CompilerError UnknownIdentifier(Node node, string name)
		{
			return Instantiate("BCE0005", node, name);
		}
		
		public static CompilerError CantCastToValueType(Node node, IType typeName)
		{
			return Instantiate("BCE0006", node, typeName);
		}

		public static CompilerError NotAPublicFieldOrProperty(Node node, string name, IType type)
		{
			return Instantiate("BCE0007", node, name, type);
		}
		
		public static CompilerError MissingConstructor(Exception error, Node node, Type type, object[] parameters)
		{
			return Instantiate("BCE0008", node, error, type, GetSignature(parameters));
		}
		
		public static CompilerError AttributeApplicationError(Exception error, Ast.Attribute attribute, Type attributeType)
		{
			return Instantiate("BCE0009", attribute, error, attributeType, error.Message);
		}
		
		public static CompilerError AstAttributeMustBeExternal(Node node, IType attributeType)
		{
			return Instantiate("BCE0010", node, attributeType);
		}
		
		public static CompilerError StepExecutionError(Exception error, ICompilerStep step)
		{
			return Instantiate("BCE0011", error, step, error.Message);
		}
		
		public static CompilerError TypeMustImplementICompilerStep(string typeName)
		{
			return Instantiate("BCE0012", LexicalInfo.Empty, typeName);
		}
		
		public static CompilerError AttributeNotFound(string elementName, string attributeName)
		{
			return Instantiate("BCE0013", LexicalInfo.Empty, elementName, attributeName);
		}
		
		public static CompilerError InvalidAssemblySetUp(Node node)
		{
			return Instantiate("BCE0014", node);
		}
		
		public static CompilerError InvalidNode(Node node)
		{
			return Instantiate("BCE0015", node, node);
		}
		
		public static CompilerError MethodArgumentCount(Node node, string name, int count)
		{
			return Instantiate("BCE0016", node, name, count);
		}
		
		public static CompilerError MethodSignature(Node node, IEntity expectedSignature, string actualSignature)
		{
			return Instantiate("BCE0017", node, expectedSignature, actualSignature);
		}
		
		public static CompilerError NameNotType(Node node, string typeName, IEntity whatItIs, string suggestion)
		{
			return Instantiate("BCE0018", node, typeName, whatItIs == null ? "not found" : (object)whatItIs, DidYouMeanOrNull(suggestion));
		}
		
		public static CompilerError MemberNotFound(MemberReferenceExpression node, INamespace @namespace, string suggestion)
		{
			return MemberNotFound(node, node.Name, @namespace, suggestion);
		}

		public static CompilerError MemberNotFound(Node node, string memberName, INamespace @namespace, string suggestion)
		{
			return Instantiate("BCE0019", node, memberName, @namespace, DidYouMeanOrNull(suggestion));
		}

		public static CompilerError InstanceRequired(Node node, IMember member)
		{
			return Instantiate("BCE0020", node, member.DeclaringType, member.Name);
		}
		
		public static CompilerError InvalidNamespace(Import import)
		{
			if (import.AssemblyReference != null)
				return Instantiate("BCE0167", import.Expression, import.Namespace, import.AssemblyReference);
			return Instantiate("BCE0021", import.Expression, import.Namespace);
		}
		
		public static CompilerError IncompatibleExpressionType(Node node, IType expectedType, IType actualType)
		{
			return Instantiate("BCE0022", node, expectedType, actualType);
		}
		
		public static CompilerError NoApropriateOverloadFound(Node node, string signature, string memberName)
		{
			return Instantiate("BCE0023", node, signature, memberName);
		}
		
		public static CompilerError NoApropriateConstructorFound(Node node, IType typeName, string signature)
		{
			return Instantiate("BCE0024", node, typeName, signature);
		}
		
		public static CompilerError InvalidArray(Node node)
		{
			return Instantiate("BCE0025", node);
		}
		
		public static CompilerError BoolExpressionRequired(Node node, IType typeName)
		{
			return Instantiate("BCE0026", node, typeName);
		}
		
		public static CompilerError NoEntryPoint()
		{
			return Instantiate("BCE0028", LexicalInfo.Empty);
		}
		
		public static CompilerError MoreThanOneEntryPoint(Method method)
		{
			return Instantiate("BCE0029", method);
		}
		
		public static CompilerError NotImplemented(Node node, string message)
		{
			return Instantiate("BCE0031", node, message);
		}
		
		public static CompilerError EventArgumentMustBeAMethod(Node node, ITypedEntity eventMember)
		{
			return Instantiate("BCE0032", node, eventMember, eventMember.Type, LanguageAmbiance.CallableKeyword);
		}
		
		public static CompilerError TypeNotAttribute(Node node, IType attributeType)
		{
			return Instantiate("BCE0033", node, attributeType);
		}
		
		public static CompilerError ExpressionMustBeExecutedForItsSideEffects(Node node)
		{
			return Instantiate("BCE0034", node);
		}

		public static CompilerError ConflictWithInheritedMember(Node node, IMember member, IMember baseMember)
		{
			return Instantiate("BCE0035", node, member, baseMember);
		}
		
		public static CompilerError InvalidTypeof(Node node)
		{
			return Instantiate("BCE0036", node);
		}
		
		public static CompilerError UnknownMacro(Node node, string name)
		{
			return Instantiate("BCE0037", node, name);
		}
		
		public static CompilerError InvalidMacro(Node node, IType type)
		{
			return Instantiate("BCE0038", node, type);
		}
		
		public static CompilerError AstMacroMustBeExternal(Node node, IType type)
		{
			return Instantiate("BCE0039", node, type);
		}
		
		public static CompilerError UnableToLoadAssembly(Node node, string name, Exception error)
		{
			return Instantiate("BCE0041", node, error, name);
		}
		
		public static CompilerError InputError(string inputName, Exception error)
		{
			return InputError(new LexicalInfo(inputName), error);
		}

		public static CompilerError InputError(LexicalInfo lexicalInfo, Exception error)
		{
			return Instantiate("BCE0042", lexicalInfo, error, lexicalInfo.FileName, error.Message);
		}

		public static CompilerError UnexpectedToken(LexicalInfo lexicalInfo, Exception error, string token)
		{
			return Instantiate("BCE0043", lexicalInfo, error, token);
		}
		
		public static CompilerError GenericParserError(LexicalInfo lexicalInfo, Exception error)
		{
			return Instantiate("BCE0044", lexicalInfo, error, error.Message);
		}
		
		public static CompilerError MacroExpansionError(Node node, Exception error)
		{
			return Instantiate("BCE0045", node, error, error.Message);
		}

		public static CompilerError MacroExpansionError(Node node)
		{
			return Instantiate("BCE0045", node, StringResources.BooC_InvalidNestedMacroContext);
		}

		public static CompilerError OperatorCantBeUsedWithValueType(Node node, string operatorName, IType typeName)
		{
			return Instantiate("BCE0046", node, operatorName, typeName);
		}
		
		public static CompilerError CantOverrideNonVirtual(Node node, IMethod method)
		{
			return Instantiate("BCE0047", node, method);
		}
		
		public static CompilerError TypeDoesNotSupportSlicing(Node node, IType fullName)
		{
			return Instantiate("BCE0048", node, fullName);
		}
		
		public static CompilerError LValueExpected(Node node)
		{
			return Instantiate("BCE0049", node, StripSurroundingParens(node.ToCodeString()));
		}

		public static CompilerError InvalidOperatorForType(Node node, string operatorName, IType typeName)
		{
			return Instantiate("BCE0050", node, operatorName, typeName);
		}
		
		public static CompilerError InvalidOperatorForTypes(Node node, string operatorName, IType lhs, IType rhs)
		{
			return Instantiate("BCE0051", node, operatorName, lhs, rhs);
		}
		
		public static CompilerError InvalidLen(Node node, IType typeName)
		{
			return Instantiate("BCE0052", node, typeName);
		}
		
		public static CompilerError PropertyIsReadOnly(Node node, IProperty property)
		{
			return Instantiate("BCE0053", node, property);
		}
		
		public static CompilerError IsaArgument(Node node)
		{
			return Instantiate("BCE0054", node, LanguageAmbiance.IsaKeyword);
		}

		public static CompilerError InternalError(Node node, Exception error)
		{
			string message = error != null ? error.Message : string.Empty;
			return InternalError(node, message, error);
		}

		public static CompilerError InternalError(Node node, string message, Exception cause)
		{
			return Instantiate("BCE0055", node, cause, message);
		}

		public static CompilerError FileNotFound(string fname)
		{
			return Instantiate("BCE0056", new LexicalInfo(fname), fname);
		}
		
		public static CompilerError CantRedefinePrimitive(Node node, string name)
		{
			return Instantiate("BCE0057", node, name);
		}

		public static CompilerError SelfIsNotValidInStaticMember(Node node)
		{	
			return Instantiate("BCE0058", node, SelfKeyword);
		}

		private static string SelfKeyword
		{
			get { return LanguageAmbiance.SelfKeyword; }
		}

		private static LanguageAmbiance LanguageAmbiance
		{
			get { return My<LanguageAmbiance>.Instance; }
		}

		public static CompilerError InvalidLockMacroArguments(Node node)
		{
			return Instantiate("BCE0059", node);
		}

		public static CompilerError NoMethodToOverride(Node node, IMethod signature, bool incompatibleSignature)
		{
			return Instantiate("BCE0060", node, signature,
				incompatibleSignature ? StringResources.BCE0060_IncompatibleSignature : null);
		}
		
		public static CompilerError NoMethodToOverride(Node node, IMethod signature, string suggestion)
		{
			return Instantiate("BCE0060", node, signature, DidYouMeanOrNull(suggestion));
		}
		
		public static CompilerError MethodIsNotOverride(Node node, IMethod method)
		{
			return Instantiate("BCE0061", node, method);
		}
		
		public static CompilerError CouldNotInferReturnType(Node node, string signature)
		{
			return Instantiate("BCE0062", node, signature);
		}
		
		public static CompilerError NoEnclosingLoop(Node node)
		{
			return Instantiate("BCE0063", node);
		}
		
		public static CompilerError UnknownAttribute(Node node, string attributeName, string suggestion)
		{
			return Instantiate("BCE0064", node, attributeName, DidYouMeanOrNull(suggestion));
		}
		
		public static CompilerError InvalidIteratorType(Node node, IType type)
		{
			return Instantiate("BCE0065", node, type);
		}
		
		public static CompilerError InvalidNodeForAttribute(Node node, string attributeName, string expectedNodeTypes)
		{
			return Instantiate("BCE0066", node, attributeName, expectedNodeTypes);
		}
		
		public static CompilerError LocalAlreadyExists(Node node, string name)
		{
			return Instantiate("BCE0067", node, name);
		}
		
		public static CompilerError PropertyRequiresParameters(Node node, IEntity name)
		{
			return Instantiate("BCE0068", node, name);
		}
		
		public static CompilerError InterfaceCanOnlyInheritFromInterface(Node node, IType interfaceType, IType baseType)
		{
			return Instantiate("BCE0069", node, interfaceType, baseType);
		}
		
		public static CompilerError UnresolvedDependency(Node node, IEntity source, IEntity target)
		{
			return Instantiate("BCE0070", node, source, target);
		}
		
		public static CompilerError InheritanceCycle(Node node, IType type)
		{
			return Instantiate("BCE0071", node, type);
		}
		
		public static CompilerError InvalidOverrideReturnType(Node node, IMethod method, IType expectedReturnType, IType actualReturnType)
		{
			return Instantiate("BCE0072", node, method, expectedReturnType, actualReturnType);
		}
		
		public static CompilerError AbstractMethodCantHaveBody(Node node, IMethod method)
		{
			return Instantiate("BCE0073", node, method);
		}
		
		public static CompilerError SelfOutsideMethod(Node node)
		{
			return Instantiate("BCE0074", node, SelfKeyword);
		}
		
		public static CompilerError NamespaceIsNotAnExpression(Node node, string name)
		{
			return Instantiate("BCE0075", node, name);
		}
		
		public static CompilerError RuntimeMethodBodyMustBeEmpty(Node node, IMethod method)
		{
			return Instantiate("BCE0076", node, method);
		}
		
		public static CompilerError TypeIsNotCallable(Node node, IType name)
		{
			return Instantiate("BCE0077", node, name);
		}
		
		public static CompilerError MethodReferenceExpected(Node node)
		{
			return Instantiate("BCE0078", node);
		}
		
		public static CompilerError AddressOfOutsideDelegateConstructor(Node node)
		{
			return Instantiate("BCE0079", node);
		}
		
		public static CompilerError BuiltinCannotBeUsedAsExpression(Node node, string name)
		{
			return Instantiate("BCE0080", node, name);
		}
		
		public static CompilerError ReRaiseOutsideExceptionHandler(Node node)
		{
			return Instantiate("BCE0081", node, LanguageAmbiance.RaiseKeyword);
		}
		
		public static CompilerError EventTypeIsNotCallable(Node node, IType typeName)
		{
			return Instantiate("BCE0082", node, typeName, LanguageAmbiance.CallableKeyword);
		}
		
		public static CompilerError StaticConstructorMustBePrivate(Node node)
		{
			return Instantiate("BCE0083", node);
		}
		
		public static CompilerError StaticConstructorCannotDeclareParameters(Node node)
		{
			return Instantiate("BCE0084", node);
		}
		
		public static CompilerError CantCreateInstanceOfAbstractType(Node node, IType typeName)
		{
			return Instantiate("BCE0085", node, typeName);
		}
		
		public static CompilerError CantCreateInstanceOfInterface(Node node, IType typeName)
		{
			return Instantiate("BCE0086", node, typeName);
		}
		
		public static CompilerError CantCreateInstanceOfEnum(Node node, IType typeName)
		{
			return Instantiate("BCE0087", node, typeName);
		}
		
		public static CompilerError MemberNameConflict(Node node, IType declaringType, string memberName)
		{
			return Instantiate("BCE0089", node, declaringType, memberName);
		}
		
		public static CompilerError DerivedMethodCannotReduceAccess(Node node, IMethod derivedMethod, IMethod superMethod, TypeMemberModifiers derivedAccess, TypeMemberModifiers superAccess)
		{
			return Instantiate("BCE0090", node, derivedMethod, superMethod, superAccess.ToString().ToLower(), derivedAccess.ToString().ToLower());
		}
		
		public static CompilerError EventIsNotAnExpression(Node node, IEntity eventMember)
		{
			return Instantiate("BCE0091", node, eventMember);
		}
		
		public static CompilerError InvalidRaiseArgument(Node node, IType typeName)
		{
			return Instantiate("BCE0092", node, typeName, LanguageAmbiance.RaiseKeyword);
		}
		
		public static CompilerError CannotBranchIntoEnsure(Node node)
		{
			return Instantiate("BCE0093", node, LanguageAmbiance.EnsureKeyword);
		}
		
		public static CompilerError CannotBranchIntoExcept(Node node)
		{
			return Instantiate("BCE0094", node);
		}
		
		public static CompilerError NoSuchLabel(Node node, string label)
		{
			return Instantiate("BCE0095", node, label);
		}
		
		public static CompilerError LabelAlreadyDefined(Node node, IMethod method, string label)
		{
			return Instantiate("BCE0096", node, method, label);
		}
		
		public static CompilerError CannotBranchIntoTry(Node node)
		{
			return Instantiate("BCE0097", node, LanguageAmbiance.TryKeyword);
		}
		
		public static CompilerError InvalidSwitch(Node node)
		{
			return Instantiate("BCE0098", node);
		}
		
		public static CompilerError YieldInsideTryExceptOrEnsureBlock(Node node)
		{
			return Instantiate("BCE0099", node, LanguageAmbiance.TryKeyword, LanguageAmbiance.ExceptKeyword, LanguageAmbiance.EnsureKeyword);
		}
		
		public static CompilerError YieldInsideConstructor(Node node)
		{
			return Instantiate("BCE0100", node);
		}
		
		public static CompilerError InvalidGeneratorReturnType(Node node, IType type)
		{
			return Instantiate("BCE0101", node, type, LanguageAmbiance.DefaultGeneratorTypeFor(type.DisplayName()));
		}

		public static CompilerError GeneratorCantReturnValue(Node node)
		{
			return Instantiate("BCE0102", node);
		}
		
		public static CompilerError CannotExtendFinalType(Node node, IType typeName)
		{
			return Instantiate("BCE0103", node, typeName);
		}
		
		public static CompilerError CantBeMarkedTransient(Node node)
		{
			return Instantiate("BCE0104", node);
		}
		
		public static CompilerError CantBeMarkedAbstract(Node node)
		{
			return Instantiate("BCE0105", node);
		}
		
		public static CompilerError FailedToLoadTypesFromAssembly(string assemblyName, Exception x)
		{
			return Instantiate("BCE0106", LexicalInfo.Empty, x, assemblyName);
		}
		
		public static CompilerError ValueTypesCannotDeclareParameterlessConstructors(Node node)
		{
			return Instantiate("BCE0107", node);
		}
		
		public static CompilerError ValueTypeFieldsCannotHaveInitializers(Node node)
		{
			return Instantiate("BCE0108", node);
		}
		
		public static CompilerError InvalidArrayRank(Node node, string arrayName, int real, int given)
		{
			return Instantiate("BCE0109", node, arrayName, real, given);
		}
		
		public static CompilerError NotANamespace(Node node, IEntity entity)
		{
			return Instantiate("BCE0110", node, entity);
		}

		public static CompilerError InvalidDestructorModifier(Node node)
		{
			return Instantiate("BCE0111", node);
		}
		
		public static CompilerError CantHaveDestructorParameters(Node node)
		{
			return Instantiate("BCE0112", node);
		}
		
		public static CompilerError InvalidCharLiteral(Node node, string value)
		{
			return Instantiate("BCE0113", node, value);
		}
		
		public static CompilerError InvalidInterfaceForInterfaceMember(Node node, string value)
		{
			return Instantiate("BCE0114", node, value);
		}

		public static CompilerError InterfaceImplForInvalidInterface(Node node, string iface, string item)
		{
			return Instantiate("BCE0115", node, iface, item);
		}
		
		public static CompilerError ExplicitImplMustNotHaveModifiers(Node node, string iface, string item)
		{
			return Instantiate("BCE0116", node, iface, item);
		}

		public static CompilerError FieldIsReadonly(Node node, string name)
		{
			return Instantiate("BCE0117", node, name);
		}

		public static CompilerError ExplodedExpressionMustBeArray(Node node)
		{
			return Instantiate("BCE0118", node);
		}

		public static CompilerError ExplodeExpressionMustMatchVarArgCall(Node node)
		{
			return Instantiate("BCE0119", node, LanguageAmbiance.CallableKeyword);
		}

		public static CompilerError UnaccessibleMember(Node node, IAccessibleMember name)
		{
			return Instantiate("BCE0120", node, name);
		}

		public static CompilerError InvalidSuper(Node node)
		{
			return Instantiate("BCE0121", node);
		}

		public static CompilerError ValueTypeCantHaveAbstractMember(Node node, IType type, IMember abstractMember)
		{
			return Instantiate("BCE0122", node, type, abstractMember);
		}

		public static CompilerError InvalidParameterType(Node node, IType typeName)
		{
			return Instantiate("BCE0123", node, typeName, string.Empty);
		}

		public static CompilerError InvalidGenericParameterType(Node node, IType type)
		{
			return Instantiate("BCE0123", node, type, "generic ");
		}

		public static CompilerError InvalidFieldType(Node node, IType typeName)
		{
			return Instantiate("BCE0124", node, typeName);
		}

		public static CompilerError InvalidDeclarationType(Node node, IType type)
		{
			return Instantiate("BCE0125", node, type);
		}

		public static CompilerError InvalidExpressionType(Node node, IType type)
		{
			return Instantiate("BCE0126", node, type);
		}
		
		public static CompilerError RefArgTakesLValue(Node node)
		{
			return Instantiate("BCE0127", node, node.ToCodeString());
		}

		public static CompilerError InvalidTryStatement(Node node)
		{
			return Instantiate("BCE0128", node, LanguageAmbiance.TryKeyword, LanguageAmbiance.ExceptKeyword, LanguageAmbiance.FailureKeyword, LanguageAmbiance.EnsureKeyword);
		}

		public static CompilerError InvalidExtensionDefinition(Node node)
		{
			return Instantiate("BCE0129", node);
		}
		
		public static CompilerError CantBeMarkedPartial(Node node)
		{
			return Instantiate("BCE0130", node);
		}
		
		public static CompilerError InvalidCombinationOfModifiers(Node node, IEntity member, string modifiers)
		{
			return Instantiate("BCE0131", node, member, modifiers);
		}
		
		public static CompilerError NamespaceAlreadyContainsMember(Node node, string container, string member)
		{
			return Instantiate("BCE0132", node, container, member);
		}

		public static CompilerError InvalidEntryPoint(Node node)
		{
			return Instantiate("BCE0133", node);
		}

		public static CompilerError CannotReturnValue(Method node)
		{
			return Instantiate("BCE0134", node, node);
		}
		
		public static CompilerError InvalidName(Node node, string name)
		{
			return Instantiate("BCE0135", node, name);
		}
		
		public static CompilerError ColonInsteadOfEquals(Node node)
		{
			return Instantiate("BCE0136", node);
		}
		
		public static CompilerError PropertyIsWriteOnly(Node node, IEntity property)
		{
			return Instantiate("BCE0137", node, property);
		}

		public static CompilerError NotAGenericDefinition(Node node, string name)
		{
			return Instantiate("BCE0138", node, name);
		}

		public static CompilerError GenericDefinitionArgumentCount(Node node, IEntity genericDefinition, int expectedCount)
		{
			return Instantiate("BCE0139", node, genericDefinition, expectedCount);
		}
				
		public static CompilerError YieldTypeDoesNotMatchReturnType(Node node, IType yieldType, IType returnType)
		{
			return Instantiate("BCE0140", node, yieldType, returnType);
		}
		
		public static CompilerError DuplicateParameterName(Node node, string parameter, IMethod method)
		{
			return Instantiate("BCE0141", node, parameter, method);
		}
		
		public static CompilerError ValueTypeParameterCannotUseDefaultAttribute(Node node, string parameter)
		{
			string method = (node is Method) ? ((Method) node).Name : ((Property) node).Name;
			return Instantiate("BCE0142", node, parameter, method);
		}
		
		public static CompilerError CantReturnFromEnsure(Node node)
		{
			return Instantiate("BCE0143", node, LanguageAmbiance.EnsureKeyword);
		}
		
		public static CompilerError Obsolete(Node node, IMember member, string message)
		{
			return Instantiate("BCE0144", node, member, message);
		}

		public static CompilerError InvalidExceptArgument(Node node, IType exceptionType)
		{
			return Instantiate("BCE0145", node, exceptionType, LanguageAmbiance.ExceptKeyword);
		}
		
		public static CompilerError GenericArgumentMustBeReferenceType(Node node, IGenericParameter parameter, IType argument)
		{
			return Instantiate("BCE0146", node, argument, parameter, parameter.DeclaringEntity);
		}

		public static CompilerError GenericArgumentMustBeValueType(Node node, IGenericParameter parameter, IType argument)
		{
			return Instantiate("BCE0147", node, argument, parameter, parameter.DeclaringEntity);
		}

		public static CompilerError GenericArgumentMustHaveDefaultConstructor(Node node, IGenericParameter parameter, IType argument)
		{
			return Instantiate("BCE0148", node, argument, parameter, parameter.DeclaringEntity);
		}

		public static CompilerError GenericArgumentMustHaveBaseType(Node node, IGenericParameter parameter, IType argument, IType baseType)
		{
			return Instantiate("BCE0149", node, argument, baseType, parameter, parameter.DeclaringEntity);
		}
		
		public static CompilerError CantBeMarkedFinal(Node node)
		{
			return Instantiate("BCE0150", node);
		}
		
		public static CompilerError CantBeMarkedStatic(Node node)
		{
			return Instantiate("BCE0151", node);
		}
		
		public static CompilerError ConstructorCantBePolymorphic(Node node, IMethod ctor)
		{
			return Instantiate("BCE0152", node, ctor);
		}

		public static CompilerError InvalidAttributeTarget(Node node, Type attrType, AttributeTargets validOn)
		{
			return Instantiate("BCE0153", node, attrType, validOn);
		}

		public static CompilerError MultipleAttributeUsage(Node node, Type attrType)
		{
			return Instantiate("BCE0154", node, attrType);
		}

		public static CompilerError CannotCreateAnInstanceOfGenericParameterWithoutDefaultConstructorConstraint(Node node, IType type)
		{
			return Instantiate("BCE0155", node, type);
		}

		public static CompilerError EventCanOnlyBeInvokedFromWithinDeclaringClass(Node node, IEvent ev)
		{
			return Instantiate("BCE0156", node, ev.Name, ev.DeclaringType);
		}
		
		public static CompilerError GenericTypesMustBeConstructedToBeInstantiated(Node node)
		{
			return Instantiate("BCE0157", node);
		}

		public static CompilerError InstanceMethodInvocationBeforeInitialization(Constructor ctor, MemberReferenceExpression mre)
		{
			return Instantiate("BCE0158", mre, mre.Name, SelfKeyword);
		}

		public static CompilerError StructAndClassConstraintsConflict(GenericParameterDeclaration gpd)
		{
			return Instantiate("BCE0159", gpd, gpd.Name);
		}

		public static CompilerError StructAndConstructorConstraintsConflict(GenericParameterDeclaration gpd)
		{
			return Instantiate("BCE0160", gpd, gpd.Name);
		}

		public static CompilerError TypeConstraintConflictsWithSpecialConstraint(GenericParameterDeclaration gpd, TypeReference type, string constraint)
		{
			return Instantiate("BCE0161", type, gpd.Name, type, constraint);
		}

		public static CompilerError InvalidTypeConstraint(GenericParameterDeclaration gpd, TypeReference type)
		{
			return Instantiate("BCE0162", type, gpd.Name, type);
		}

		public static CompilerError MultipleBaseTypeConstraints(GenericParameterDeclaration gpd, TypeReference type, TypeReference other)
		{
			return Instantiate("BCE0163", type, gpd.Name, type, other);
		}

		public static CompilerError CannotInferGenericMethodArguments(Node node, IMethod method)
		{
			return Instantiate("BCE0164", node, method);
		}

		public static CompilerError ExceptionAlreadyHandled(ExceptionHandler dupe, ExceptionHandler previous)
		{
			return Instantiate("BCE0165",
				dupe.Declaration, dupe.Declaration.Type, previous.Declaration.Type,
				AstUtil.SafePositionOnlyLexicalInfo(previous.Declaration), LanguageAmbiance.ExceptKeyword);
		}

		public static CompilerError UnknownClassMacroWithFieldHint(MacroStatement node, string name)
		{
			return Instantiate("BCE0166", node, name);
		}

		public static CompilerError PointerIncompatibleType(Node node, IType type)
		{
			return Instantiate("BCE0168", node, type);
		}

		public static CompilerError NotAMemberOfExplicitInterface(TypeMember member, IType type)
		{
			return Instantiate("BCE0169", member, member.Name, type);
		}

		public static CompilerError EnumMemberMustBeConstant(EnumMember member)
		{
			return Instantiate("BCE0170", member, member.FullName);
		}

		public static CompilerError ConstantCannotBeConverted(Node node, IType type)
		{
			return Instantiate("BCE0171", node, node, type);
		}

		public static CompilerError InterfaceImplementationMustBePublicOrExplicit(TypeMember node, IMember member)
		{
			return Instantiate("BCE0172", node, member.DeclaringType);
		}

		public static CompilerError InvalidRegexOption(RELiteralExpression node, char option)
		{
			return Instantiate("BCE0173", node, option);
		}

		public static CompilerError InvalidTypeForExplicitMember(Node node, IType type)
		{
			return Instantiate("BCE0174", node, type);
		}

		public static CompilerError NestedTypeCannotExtendEnclosingType(Node node, IType nestedType, IType enclosingType)
		{
			return Instantiate("BCE0175", node, nestedType, enclosingType);
		}

		public static CompilerError IncompatiblePartialDefinition(Node node, string typeName, string expectedType, string actualType)
		{
			return Instantiate("BCE0176", node, typeName, expectedType, actualType);
		}
		
		public static CompilerError TypeExpected(Node node)
		{
			return Instantiate("BCE0177", node);
		}

        public static CompilerError InvalidAsyncType(TypeReference tr)
        {
            return Instantiate("BCE0178", tr);
        }

        public static CompilerError InvalidAwaitType(Expression e)
        {
            return Instantiate("BCE0179", e);
        }

        public static CompilerError RestrictedAwaitType(Node n, IType t)
        {
            return Instantiate("BCE0180", n, t);
        }

	    public static CompilerError UnsafeReturnInAsync(Expression e)
	    {
	        return Instantiate("BCE0181", e);
	    }

		public static CompilerError MissingGetAwaiter(Expression e)
		{
			return Instantiate("BCE0182", e.LexicalInfo, e.ExpressionType);
		}

        public static CompilerError Instantiate(string code, Exception error, params object[] args)
		{
			return new CompilerError(code, error, args);
		}

		public static CompilerError Instantiate(string code, Node anchor, Exception error, params object[] args)
		{
			return Instantiate(code, AstUtil.SafeLexicalInfo(anchor), error, args);
		}

		private static CompilerError Instantiate(string code, LexicalInfo location, Exception error, params object[] args)
		{
			return new CompilerError(code, location, error, args);
		}

		public static CompilerError Instantiate(string code, Node anchor, params object[] args)
		{
			return Instantiate(code, AstUtil.SafeLexicalInfo(anchor), args);
		}

		private static CompilerError Instantiate(string code, LexicalInfo location, params object[] args)
		{
			return new CompilerError(code, location, Array.ConvertAll<object, string>(args, DisplayStringFor));
		}

		internal static string DisplayStringFor(object o)
		{
			if (o == null) return "";

			var entity = o as IEntity;
			return entity != null ? entity.DisplayName() : o.ToString();
		}

		public static string ToStringList(System.Collections.IEnumerable names)
		{
			return Builtins.join(names.Cast<object>().Select(o => DisplayStringFor(o)).OrderBy(_ => _), ", ");
		}
		
		public static string ToAssemblyQualifiedNameList(List types)
		{
			var builder = new StringBuilder();
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
			var sb = new StringBuilder("(");
			for (int i=0; i<parameters.Length; ++i)
			{
				if (i > 0) sb.Append(", ");
				sb.Append(parameters[i].GetType());
			}
			sb.Append(")");
			return sb.ToString();
		}

		public static string ToNameList(System.Reflection.MemberInfo[] members)
		{
			var sb = new StringBuilder();
			for (int i=0; i<members.Length; ++i)
			{
				if (i > 0) sb.Append(", ");
				sb.Append(members[i].MemberType.ToString());
				sb.Append(" ");
				sb.Append(members[i].Name);
			}
			return sb.ToString();
		}

		private static string DidYouMeanOrNull(string suggestion)
		{
			return (null != suggestion)
				? string.Format(StringResources.BooC_DidYouMean, suggestion)
				: null;
		}

		private static string StripSurroundingParens(string code)
		{
			return code.StartsWith("(") ? code.Substring(1, code.Length - 2) : code;
		}
	}
}
