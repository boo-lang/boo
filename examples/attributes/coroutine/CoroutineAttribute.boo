# @author	Cedric Vivier <cedricv@neonux.com>
#
# [coroutine] AST attribute
#
# Compile with:   booc -t:library -o:Coroutine.dll CoroutineAttribute.dll
#
# Look at tests/testcases/attributes/coroutine-*.boo for usage examples.
#
# Attribute options/arguments:
#
#   Looping: bool   - Does the coroutine loop? (default: true)
#
#   Default: expression
#                   The value to return when the coroutine is terminated (with 
#                   Looping false) or when it is blocking (with Blocking false)
#                   If Default has not been set the coroutine will raise
#                   a CoroutineTerminatedException if invoked after termination.
#
#   DefaultLastValue: bool
#                   When true default value is the last value. (default :false)
#
#   Future: bool    - Compute the future before it is requested (default: false)
#                   NB: 1st call to the coroutine will be synchronous
#
#   Timeout: long   - Timeout (in ms) before a blocking future call will return
#                   Default or raise a CoroutineFutureNotReadyException.
#                   (default: -1 means no timeout)
#
#   ThreadSafe: bool
#                   If true, there will be no lock on the method when Future is
#                   enabled. (default: false)
#
#   Parallel: int	- Number of futures to compute in parallel (default: 1)
#                   UNSUPPORTED		TODO:
#

namespace Coroutine

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast


[AttributeUsage(AttributeTargets.Method)]
public class CoroutineAttribute(AbstractAstAttribute):

	Looping as BoolLiteralExpression:
		set:
			_looping = value.Value
	_looping = true

	Default as Expression:
		set:
			_default = value
			_defaultSet = true
	_default as Expression = null
	_defaultSet = false

	DefaultLastValue as BoolLiteralExpression:
		set:
			_defaultLastValue = value.Value
	_defaultLastValue = false

	Future as BoolLiteralExpression:
		set:
			_future = value.Value
	_future = false

	Timeout as IntegerLiteralExpression:
		set:
			_timeout = value.Value
	_timeout = -1L

	ThreadSafe as BoolLiteralExpression:
		set:
			_threadSafe = value.Value
	_threadSafe = false

	Parallel as IntegerLiteralExpression: #TODO: compute parallel futures
		set:
			raise NotSupportedException("parallel futures are not yet supported!")
	_parallel = 1


	final ResetEventImplementationTypeName = "System.Threading.AutoResetEvent"
	final ThreadImplementationTypeName = "System.Threading.Thread"

	_m as Method
	_generatorName as string


	public override def Apply(node as Node):
		_m = node as Method
		SanityCheck()

		_generatorName = "__"+_m.Name

		#enumerator type
		if _m.ReturnType is null:
			_m.ReturnType = SimpleTypeReference("System.Object")
		et = GenericTypeReference()
		et.Name = "System.Collections.Generic.IEnumerator"
		et.GenericArguments.Add(_m.ReturnType)

		eRef = GetReferenceForNewField("__enumerator", et)#ref to enumerator

		if _defaultSet or _defaultLastValue:
			f = CreateField("__default", _m.ReturnType)
			if _defaultSet:
				f.Initializer = _default
			_m.DeclaringType.Members.Add(f)
			dRef = ReferenceExpression(f.Name)#ref to default value

		if _future:
			rRef = GetReferenceForNewField("__reset", SimpleTypeReference(ResetEventImplementationTypeName))
			rtRef = GetReferenceForNewField("__resetThread", SimpleTypeReference(ResetEventImplementationTypeName))
			tRef = GetReferenceForNewField("__thread", SimpleTypeReference(ThreadImplementationTypeName))
			f = CreateField("__lock", SimpleTypeReference("System.Object"))
			f.Initializer = [| System.Object() |]
			_m.DeclaringType.Members.Add(f)
			lRef = ReferenceExpression(f.Name)#ref to lock object

		#generator invocation 
		gInvoc = MethodInvocationExpression(MemberReferenceExpression(SelfLiteralExpression(), _generatorName))
		for parameter in _m.Parameters:
			gInvoc.Arguments.Add(ReferenceExpression(parameter.Name))

		facade = _m.Clone() as Method
		facade.IsSynthetic = true

		if not _future:
			facade.Body = [| 
				block:
					if $eRef is null:
						$eRef = $gInvoc
			|].Block
			moveNext = [| $(eRef).MoveNext() |]
			if _looping:
				#$(eRef).Reset() not used to handle new arguments passed in at reset
				b = [|
						block:
							if not $moveNext:
								$eRef = $gInvoc
								$moveNext
					|].Block
				facade.Body.Add(b)
			elif _defaultSet or _defaultLastValue:
				b = [|
						block:
							if not $moveNext:
								return $dRef
					|].Block
				facade.Body.Add(b)
			else:
				b = [|
						block:
							if not $moveNext:
								raise CoroutineTerminatedException()
					|].Block
				facade.Body.Add(b)
			if _defaultLastValue:
				b = [|
						block:
							$dRef = $(eRef).Current
							return $dRef
					|].Block
				facade.Body.Add(b)
			else:
				facade.Body.Add([| return $(eRef).Current |])

		#future
		else:
			t = Method(tRef.Name+"__callable")
			t.IsSynthetic = true
			if _m.IsStatic or _m.DeclaringType isa Module:
				t.Modifiers |= TypeMemberModifiers.Static
			t.Modifiers |= TypeMemberModifiers.Private
			t.Body = [|
						block:
							while true:
								$(rtRef).WaitOne($_timeout, false)
								break if not $(eRef).MoveNext()
								$(rRef).Set()
							$tRef = null
							$(rRef).Set()
					|].Block
			_m.DeclaringType.Members.Add(t)
			callableRef = ReferenceExpression(t.Name)

			if _defaultSet:
				facade.Body = [|
					block:
						future as $(_m.ReturnType) = $dRef
				|].Block
			else:
				facade.Body = [|
					block:
						future as $(_m.ReturnType)
				|].Block
			b = [|
					block:
						:init
						if $eRef is null:
							$eRef = $gInvoc
							$rRef = System.Threading.AutoResetEvent(false)
							$rtRef = System.Threading.AutoResetEvent(true)
							$tRef = Thread($callableRef)
							$(tRef).IsBackground = true								
							$(tRef).Name = "coroutine ${$(facade.Name)}"
							$(tRef).Start()
						if $tRef is not null and $(tRef).IsAlive:
							gotIt = $(rRef).WaitOne($_timeout, false)
				|].Block
			if _looping:
				b2 = [|
						block:
							if $tRef is null or not $(tRef).IsAlive:
								$eRef = null
								goto init
					|].Block
			elif _defaultSet or _defaultLastValue:
				b2 = [|
						block:
							if $tRef is null or not $(tRef).IsAlive:
								return $dRef
					|].Block
			else:
				b2 = [|
						block:
							if $tRef is null or not $(tRef).IsAlive:
								raise CoroutineTerminatedException()
					|].Block
			b.Add(b2)

			if _defaultSet or _defaultLastValue:
				b2 = [|
						block:
							if not gotIt:
								return $dRef
					|].Block
			else:
				b2 = [|
						block:
							if not gotIt:
								raise CoroutineFutureNotReadyException()
					|].Block
			b.Add(b2)

			if _defaultLastValue:
				b.Add([| $dRef = $(eRef).Current |])
			b.Add([| future = $(eRef).Current |])
			b.Add([| $(rtRef).Set() |])

			if not _threadSafe:
				lockBlock = [|
								block:
									lock $lRef:
										$b
							|].Block
				facade.Body.Add(lockBlock)
			else:
				facade.Body.Add(b)
			facade.Body.Add([| return future |])

		#hide the generator implementation
		_m.Name = _generatorName
		_m.Modifiers |= TypeMemberModifiers.Private
		_m.ReturnType = et

		#promote the facade
		_m.DeclaringType.Members.Add(facade)


	private def CreateField(name as string, type as TypeReference) as Field:
		f = Field()
		f.Name = "${name}${_generatorName}${Context.AllocIndex()}"
		f.IsSynthetic = true
		if _m.IsStatic or _m.DeclaringType isa Module:
			f.Modifiers |= TypeMemberModifiers.Static
		f.Modifiers |= TypeMemberModifiers.Private
		f.Type = type
		return f


	private def GetReferenceForNewField(name as string, type as TypeReference) as ReferenceExpression:
		f = CreateField(name, type)
		_m.DeclaringType.Members.Add(f)
		return ReferenceExpression(f.Name)		


	private def SanityCheck():
		if _m is null:
			InvalidNodeForAttribute('Method')
			return
		if _looping and _defaultSet:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "Looping and Default are mutually exclusive"))
			return
		if _defaultSet and _defaultLastValue:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "DefaultLastValue and Default are mutually exclusive"))
			return
		if -1L != _timeout and not _future:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "Timeout requires Future:true"))
			return
		if 1 != _parallel and not _future:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "Parallel requires Future:true"))
			return


class CoroutineTerminatedException(System.Exception):
	pass

class CoroutineFutureNotReadyException(System.Exception):
	pass
