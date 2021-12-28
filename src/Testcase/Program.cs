using System;

namespace Testcase
{
    public class Foo { }

    public class Bar : Foo
    {
        virtual public void Run()
        {
            Console.WriteLine("Bar.Run");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var foos = new Foo[] { new Bar(), new Bar() };
            foreach (Bar foo in foos)
                foo.Run();
        }
    }
}
