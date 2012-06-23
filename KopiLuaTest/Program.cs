using NUnitLite.Runner;

namespace KopiLuaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new TextUI(new ConsoleWriter()).Execute(args);
        }
    }
}
