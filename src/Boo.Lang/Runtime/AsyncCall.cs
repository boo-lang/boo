#region license
// Copyright (c) the Boo contributors
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
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Boo.Lang.Runtime;

/// <summary>
/// A callable invoked on another thread, behind BeginInvoke and EndInvoke.
/// </summary>
/// <remarks>
/// The CLR used to implement those two on every delegate, over remoting.
/// .NET dropped remoting and left them throwing PlatformNotSupportedException,
/// so the compiler emits bodies that come here instead. The call runs on the
/// thread pool; EndInvoke waits for it, hands back what it returned and
/// rethrows what it threw.
/// </remarks>
public sealed class AsyncCall : IAsyncResult
{
	private readonly Delegate _target;
	private readonly object[] _arguments;
	private readonly object _state;
	private readonly AsyncCallback _callback;
	private readonly ManualResetEvent _done = new ManualResetEvent(false);

	private object _result;
	private Exception _error;
	private volatile bool _completed;
	private volatile bool _endInvokeCalled;

	private AsyncCall(Delegate target, object[] arguments, AsyncCallback callback, object state)
	{
		_target = target;
		_arguments = arguments;
		_callback = callback;
		_state = state;
	}

	public static IAsyncResult Begin(Delegate target, object[] arguments, AsyncCallback callback, object state)
	{
		if (null == target)
			throw new ArgumentNullException("target");

		var call = new AsyncCall(target, arguments, callback, state);
		ThreadPool.QueueUserWorkItem(_ => call.Run());
		return call;
	}

	/// <summary>
	/// Waits for the call to finish and returns its result, or rethrows.
	/// </summary>
	public static object End(IAsyncResult result)
	{
		var call = Expect(result);
		call._done.WaitOne();
		call._endInvokeCalled = true;
		if (null != call._error)
			ExceptionDispatchInfo.Capture(call._error).Throw();
		return call._result;
	}

	/// <summary>
	/// The callable the call was started on.
	/// </summary>
	public static Delegate Target(IAsyncResult result)
	{
		return Expect(result)._target;
	}

	/// <summary>
	/// The arguments the call was given, with any ref parameters holding the
	/// values it left behind.
	/// </summary>
	public static object[] Arguments(IAsyncResult result)
	{
		return Expect(result)._arguments;
	}

	private static AsyncCall Expect(IAsyncResult result)
	{
		if (null == result)
			throw new ArgumentNullException("result");

		var call = result as AsyncCall;
		if (null == call)
			throw new ArgumentException("The result did not come from this callable's BeginInvoke.", "result");
		return call;
	}

	private void Run()
	{
		try
		{
			// DynamicInvoke writes ref parameters back into the array.
			_result = _target.DynamicInvoke(_arguments);
		}
		catch (TargetInvocationException x)
		{
			_error = x.InnerException ?? x;
		}
		catch (Exception x)
		{
			_error = x;
		}
		finally
		{
			_completed = true;
			_done.Set();
		}

		if (null != _callback)
			_callback(this);
	}

	// Public rather than explicit implementations: Boo reaches these by name
	// through duck typing, which only sees a type's own public members. The
	// remoting AsyncResult these replace exposed them the same way.

	public object AsyncState
	{
		get { return _state; }
	}

	public WaitHandle AsyncWaitHandle
	{
		get { return _done; }
	}

	public bool CompletedSynchronously
	{
		get { return false; }
	}

	public bool IsCompleted
	{
		get { return _completed; }
	}

	/// <summary>
	/// Whether the result has been collected with EndInvoke.
	/// </summary>
	public bool EndInvokeCalled
	{
		get { return _endInvokeCalled; }
	}
}
