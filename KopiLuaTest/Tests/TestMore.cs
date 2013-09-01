using NUnit.Framework;
using KopiLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KopiLuaTest.Tests
{
    [TestFixture]
    public class TestMore
    {
        private static string _scriptPath = System.IO.Path.GetFullPath(@"..\..\LuaTestMore");
        private static HashSet<string> _blacklist = new HashSet<string>
            {
                "241-standalone.t",
                "308-os.t",
#if false
                "104-number.t",
                "201-assign.t",
                "301-basic.t",
                "303-package.t",
                "304-string.t",
                "306-math.t",
                "307-io.t",
                "310-stdin.t",
#endif
            };

        /// <summary>
        /// Enumerate the TestMore test cases, minus the ones in the blacklist.
        /// </summary>
        public IEnumerable<string> EnumerateTestMoreFiles()
        {
            foreach (var fileInfo in new DirectoryInfo(_scriptPath).GetFiles("*.t"))
            {
                string name = fileInfo.Name;
                if (!_blacklist.Contains(name))
                    yield return fileInfo.Name;
            }
        }

        /// <summary>
        /// Run the TestMore-style tests that tend to pass
        /// </summary>
        [Test, TestCaseSource("EnumerateTestMoreFiles")]
        //[Explicit]
        public void RunPassingTestMoreTest(string filename)
        {
            RunTestMoreTest(filename);
        }

        /// <summary>
        /// Run the TestMore-style tests that tend to fail
        /// </summary>
        [Test, TestCaseSource("_blacklist")]
        [Explicit]
        public void RunFailingTestMoreTest(string filename)
        {
            RunTestMoreTest(filename);
        }

        public void AssertLuaResult(Lua.lua_State L, int result)
        {
            if (result != 0)
            {
                Utils.DumpStack(L);
                Assert.Fail(GetLuaError(L));
            }
        }

        public string GetLuaError(Lua.lua_State L)
        {
            if (Lua.lua_gettop(L) == 0)
                return "(no error message)";
         
            var s = Lua.lua_tostring(L, -1);
            if (s == null)
                return "(null error message)";

            return s.ToString();
        }

        public void LuaDoString(Lua.lua_State L, string s)
        {
            AssertLuaResult(L, Lua.luaL_loadstring(L, s));
            AssertLuaResult(L, Lua.lua_pcall(L, 0, -1, 0));
        }

        public void LuaDoString(Lua.lua_State L, string s, params object[] args)
        {
            LuaDoString(L, string.Format(s, args));
        }

        /// <summary>
        /// Runs a lua script from Lua-TestMore and interprets the result
        /// </summary>
        /// <param name="filename">Filename of the TestMore test case to run</param>
        //[TestCase("ultrasimple.txt")]
        //[TestCase("require.txt")]
        //[TestCase("001-if.t")]
        //[TestCase("014-fornum.t")]
        //[TestCase("103-nil.t")]
        //[TestCase("201-assign.t")]
        //[TestCase("212-function.t")]
        //[TestCase("304-string.t")]
        //[TestCase("305-table.t")]
        public void RunTestMoreTest(string filename)
        {
            var errorStream = new MemoryStream();
            Lua.stderr = errorStream;

            var sb = new StringBuilder();
            using (new CaptureConsole(sb))
            {
                var L = Lua.luaL_newstate();
                Lua.luaL_openlibs(L);

                LuaDoString(L, "arg = {}");
                LuaDoString(L, "arg[0] = '..\\\\..\\\\LuaTestMore\\\\{0}'", filename);
                LuaDoString(L, "package.path = package.path .. ';..\\\\..\\\\LuaTestMore\\\\?.lua'");

                AssertLuaResult(L, Lua.luaL_loadfile(L, _scriptPath + "\\" + filename));
                
                int result = Lua.lua_pcall(L, 0, -1, 0);
                if (result != 0)
                {
                    string errorMessage;

                    switch (result)
                    {
                        case Lua.LUA_ERRMEM:
                            errorMessage = "Out of memory";
                            break;

                        default:
                            errorMessage = result.ToString() + " " + GetLuaError(L);
                            break;
                    }
                     
                    var bytes = new UTF8Encoding().GetBytes(errorMessage);
                    errorStream.Write(bytes, 0, bytes.Length);
                }
            }

            var errors = new List<string>();

            errorStream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(errorStream);
            while (!reader.EndOfStream)
            {
                errors.Add(reader.ReadLine());
            }

            if (errors.Count > 0)
            {
                string message = "";
                foreach (var line in errors)
                    message += "      " + line + "\n";
                Assert.Fail(message);
            }
        }

        public class ConsoleWriterStream : Stream
        {
            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                System.Console.Out.Flush();
            }

            public override long Length
            {
                get { throw new NotImplementedException(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                System.Console.Out.Write(Encoding.UTF8.GetString(buffer, offset, count));
            }
        }

        private NUnitListener _nunitListener = new NUnitListener();

        [SetUp]
        public void DisableAssertDialogs()
        {
            //Debug.Listeners.Clear();
            //Debug.Listeners.Add(_nunitListener);
            Lua.stdin = new MemoryStream();
            Lua.stdout = new ConsoleWriterStream();
            Lua.stderr = new MemoryStream();
        }

        private class NUnitListener : DefaultTraceListener
        {
            public override void Fail(string message)
            {
                Assert.Fail(message);
            }

            public override void Fail(string message, string detailMessage)
            {
                Assert.Fail("{0} - {1}", message, detailMessage);
            }
        }
    }
}
