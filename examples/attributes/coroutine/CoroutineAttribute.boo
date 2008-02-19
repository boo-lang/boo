#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


# @author	Cedric Vivier <cedricv@neonux.com>
#
# [coroutine] AST attribute
#
# Compile with:   booc -t:library -o:Coroutine.dll CoroutineAttribute.dll
#
# Look at tests/testcases/attributes/coroutine-*.boo for usage examples.
#
# OPTIONS:
#
#   Looping: bool   - Does the coroutine loop? (default: true)
#
#   Default: expression
#                   The value to return whenever the coroutine is terminated 
#                   (Looping:false) or when Timeout has expired.
#                   If Default has not been set the coroutine will raise
#                   the corresponding exception (see 'Exceptions' below)
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
#
# EXCEPTIONS:
#
#   CoroutineTerminatedException:
#                   This exception is raised whenever the coroutine has finished
#                   processing and Looping is false
#
#   CoroutineFutureNotReadyException
#                  This exception is raised whenever a Timeout is set and the 
#                  result of the coroutine is not yet available.
#

namespace Coroutine

import System
import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem


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
		return if not SanityCheck()

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
		
		#annotate the facade as a coroutine
		facade.Annotate("boo.coroutine", null)

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


	private def SanityCheck() as bool:
		if _m is null:
			InvalidNodeForAttribute('Method')
			return false
		
		#check arguments
		if _looping and _defaultSet and not _future:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "Looping and Default are mutually exclusive in a non-future context"))
			return false
		if _defaultSet and _defaultLastValue:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "DefaultLastValue and Default are mutually exclusive"))
			return false
		if -1L != _timeout and not _future:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "Timeout requires Future:true"))
			return false
		if 1 != _parallel and not _future:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "Parallel requires Future:true"))
			return false
		
		#check if there is at least one yield in the method
		finder = YieldFinder()
		finder.Visit(_m)
		if not finder.Found:
			Errors.Add(CompilerErrorFactory.CustomError(self.LexicalInfo, "There is no yield statement in the coroutine"))
			return false

		return true
		

	#HACK: should be a simple way to restart the bind attribute step (?)
	public def SetCompilerContext(context as CompilerContext):
		_context = context



class CoroutineTerminatedException(System.Exception):
	pass

class CoroutineFutureNotReadyException(System.Exception):
	pass


#
#	SPAWN
#
#	FIXME: move into their own files blablabla...
#
#

interface ISpawnable:
	def Execute() as bool


static class CoroutineSchedulerManager:

	Coroutines:
		get:
			#FIXME: synchronized dictionary(?)
			return _coroutines
	_coroutines = List[of ISpawnable]()
	Slices:
		get:
			return _slices
	_slices = Dictionary[of ISpawnable, int]()

	Scheduler as ICoroutineScheduler:
		get:
			if _scheduler is null:
				_scheduler = DeadlineCoroutineScheduler()
			return _scheduler
		set:
			if _scheduler and _scheduler.IsRunning:
				raise InvalidOperationException("Cannot change the scheduler while it is running.")
			_scheduler = value
	_scheduler as ICoroutineScheduler = null


interface ICoroutineScheduler:
	IsRunning as bool:
		get
	def JoinStart():
		pass


class DeadlineCoroutineScheduler(ICoroutineScheduler):

	_lock_IsRunning = object()

	IsRunning as bool:
		get:
			lock _lock_IsRunning:
				return _isRunning
	_isRunning = false

	def JoinStart():
		lock _lock_IsRunning:
			if _isRunning:
				raise InvalidOperationException("Scheduler is already running.")
			_isRunning = true
		coroutines = CoroutineSchedulerManager.Coroutines #FIXME: sync copy
		slices = CoroutineSchedulerManager.Slices
		try:
			:runSlices
			toRemove as List[of ISpawnable] = null
			while 0 != coroutines.Count:
				for c in coroutines:
					s = slices[c]
					try:
						if 1 == s:
							c.Execute()
						else:
							for i in range(0, s):
								c.Execute()
					except e as CoroutineTerminatedException:
						if toRemove is null:
							toRemove = List[of ISpawnable]()
						toRemove.Add(c)
					except e as CoroutineFutureNotReadyException:
						pass
				if toRemove is not null:
					for c in toRemove:
						coroutines.Remove(c)
					goto runSlices
		ensure:
			lock _lock_IsRunning:
				_isRunning = false



def GetSpawnable(spawnable as MethodInvocationExpression, nss as NameResolutionService, context as CompilerContext):
	if spawnable is null:
		raise ArgumentException("first argument must be a ISpawnable instance")

	spawnClass = nss.Resolve((spawnable.Target as ReferenceExpression).Name, EntityType.Type) as InternalClass
	if spawnClass is null:
		raise ArgumentException("spawn is supported on internal types only")

	ifaces = spawnClass.GetInterfaces()
	foundISpawnable = false
	for iface in ifaces:
		if "Coroutine.ISpawnable" == iface.FullName:
			foundISpawnable = true
			break
	if not foundISpawnable:
		raise ArgumentException("spawn first argument must implement ISpawnable")

	classDef = spawnClass.Node as ClassDefinition
	execute = classDef.Members["Execute"] as Method
	if not execute.ContainsAnnotation("boo.coroutine"):
		YieldInserter().Visit(execute)
		execute.ToCodeString()
		astAttr = CoroutineAttribute(Looping: BoolLiteralExpression(false))
		astAttr.SetCompilerContext(context)#HACK: should restart the step somehow
		astAttr.Apply(execute)

	return spawnable




internal class YieldFinder(DepthFirstVisitor):
	[property(Found)]
	_found = false
	override def OnYieldStatement(node as YieldStatement):
		_found = true

internal class YieldInserter(DepthFirstVisitor):
	[property(Inserted)]
	_inserted = 0
	override def LeaveBlock(node as Block):
		node.Add(YieldStatement())


#
# spawn macro
# Usage: spawn [ISpawnable_instance[, slices]]	(slices=1 by default)
# Use spawn with no argument to launch execution of your spawnables.
#
macro spawn:
	slices = 1
	if 0 == len(spawn.Arguments):
		return [|
					block:
						CoroutineSchedulerManager.Scheduler.JoinStart()
				|].Block
	if 1 <= len(spawn.Arguments):
		spawnable = GetSpawnable(spawn.Arguments[0], NameResolutionService, Context)
	if 2 <= len(spawn.Arguments):
		if not spawn.Arguments[1] isa IntegerLiteralExpression:
			raise ArgumentException("second argument 'slices' must be an integer literal")
		slices = (spawn.Arguments[1] as IntegerLiteralExpression).Value
	if 3 <= len(spawn.Arguments):
		raise ArgumentException("Usage is:  spawn [ISpawnable instance, [slices]]")
	return [|
				block:
					tmp = $spawnable
					CoroutineSchedulerManager.Coroutines.Add(tmp)
					CoroutineSchedulerManager.Slices.Add(tmp, $slices)
			|].Block

