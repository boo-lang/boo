import System.Threading.Tasks
import System

static class Program:
    [async] def M(input as int) as Task:
        var value = 1
        __switch__(value, c0, c1)
        :c0
        return
        :c1
        return

def Main():
    Program.M(0).Wait()
