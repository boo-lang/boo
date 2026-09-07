namespace Boo.Lang.Lsp.Tests

import Boo.Lang.Lsp.Protocol

# NUnit wants an answer rather than a task, so a test waits here and nowhere
# else. GetResult rather than Wait, so what surfaces is the exception the
# server raised and not an AggregateException wrapping it.

[Extension] def Read(this as MessageStream) as string:
	return this.ReadAsync().GetAwaiter().GetResult()

[Extension] def Write(this as MessageStream, message as string):
	this.WriteAsync(message).GetAwaiter().GetResult()

[Extension] def Listen(this as Connection):
	this.ListenAsync().GetAwaiter().GetResult()
	# Both phases, the same two a session runs.
	this.DrainAsync().GetAwaiter().GetResult()
