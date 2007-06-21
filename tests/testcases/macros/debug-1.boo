"""
But you do see this...
1 2 3
<debug>
2+2: 4
"""
import System
import System.IO
import System.Diagnostics

debug "You don't see this..."

Debug.Listeners.Add(TextWriterTraceListener(Console.Out))

debug "But you do see this..."
debug 1, 2, 3
debug
debug "2+2:", 4
