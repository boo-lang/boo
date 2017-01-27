"""
Hello, World!
"""

import System.Threading
import System.Threading.Tasks

[async] public static def F(a as string) as Task:
	await(Task.Factory.StartNew({ System.Console.WriteLine(a) }))

F('Hello, World!').Wait()
