#region license
// Copyright (c) 2026 the Boo contributors
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

namespace Boo.Lang.Lsp.Server

import System
import System.Threading
import System.Threading.Channels
import System.Threading.Tasks
import Boo.Lang.Lsp.Workspace

callable AnalysisRequested(document as TextDocument)

class AnalysisWorker:
"""
Runs compilation on one background task, off the message loop.

Binding a project costs hundreds of milliseconds once it is large enough, and
the compiler is not reentrant, so compilation happens here, one document at a
time, while the message loop stays free to read the next keystroke.

A submission waits out a debounce before it is compiled, so a burst of
keystrokes costs one compile rather than one per character.
"""

	_analyze as AnalysisRequested
	_debounce as int
	_queue = AnalysisQueue()

	# The channel only says work is waiting; the queue says which.
	_wakeup = Channel.CreateBounded[of bool](
		BoundedChannelOptions(1, FullMode: BoundedChannelFullMode.DropOldest, SingleReader: true))
	_idle = ManualResetEventSlim(true)
	_stopping as CancellationTokenSource
	_loop as Task

	def constructor(analyze as AnalysisRequested, debounceMilliseconds as int):
		_analyze = analyze
		_debounce = debounceMilliseconds

	IsRunning as bool:
		get: return _loop is not null

	def Start():
		return if _loop is not null
		_stopping = CancellationTokenSource()
		_loop = Task.Run({ Loop(_stopping.Token) })

	def Stop():
		return if _loop is null
		_stopping.Cancel()
		_wakeup.Writer.TryComplete()
		try:
			_loop.Wait(2000)
		except as AggregateException:
			# Cancelling is how it is asked to stop; that is not a failure.
			pass
		ensure:
			_stopping.Dispose()
			_stopping = null
			_loop = null

	def Submit(document as TextDocument):
		_idle.Reset()
		_queue.Submit(document)
		_wakeup.Writer.TryWrite(true)

	def Withdraw(uri as string):
		_queue.Withdraw(uri)

	def WaitForIdle(timeoutMilliseconds as int) as bool:
	"""Waits for the queue to empty, which shutting down does before it stops."""
		return _idle.Wait(timeoutMilliseconds)

	private def Loop(stopping as CancellationToken):
		try:
			while _wakeup.Reader.WaitToReadAsync(stopping).AsTask().Result:
				bool_ as bool
				while _wakeup.Reader.TryRead(bool_):
					pass

				# Let a burst of keystrokes settle before paying for a compile.
				Task.Delay(_debounce, stopping).Wait()

				for document in _queue.Drain():
					break if stopping.IsCancellationRequested
					Analyze(document)

				_idle.Set() if _queue.Count == 0
		except as OperationCanceledException:
			pass
		except as AggregateException:
			pass
		ensure:
			_idle.Set()

	private def Analyze(document as TextDocument):
		try:
			_analyze(document)
		except e as Exception:
			# One bad document must not take the worker down with it.
			Console.Error.WriteLine("boo-ls: analyzing ${document.Uri} failed: ${e}")
