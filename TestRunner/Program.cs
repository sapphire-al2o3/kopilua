using NUnitLite.Runner;

namespace TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            KopiLuaTest.Program.Execute(new TextUI(new ConsoleWriter()), args);
        }
    }
}
