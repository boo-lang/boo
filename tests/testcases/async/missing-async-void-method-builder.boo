//update this with appropriate Boo errors
"""
(4,16): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
(4,20): error CS0518: Predefined type 'System.Runtime.CompilerServices.AsyncVoidMethodBuilder' is not defined or imported
(4,20): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create'
(4,20): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext'
(4,20): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine'


"""

class C:
    [async] def M():
        pass
