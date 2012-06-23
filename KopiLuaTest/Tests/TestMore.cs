using NUnit.Framework;
using KopiLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace KopiLuaTest.Tests
{
    [TestFixture]
    public class TestMore
    {
        private static string _scriptPath = System.IO.Path.GetFullPath(@"..\..\LuaTestMore");
        private static HashSet<string> _blacklist = new HashSet<string>
            {
                "104-number.t",
                "201-assign.t",
                "241-standalone.t",
                "301-basic.t",
                "303-package.t",
                "304-string.t",
                "306-math.t",
                "307-io.t",
                "308-os.t",
                "310-stdin.t",
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

        /// <summary>
        /// Runs a lua script from Lua-TestMore and interprets the result
        /// </summary>
        /// <param name="filename">Filename of the TestMore test case to run</param>
        public void RunTestMoreTest(string filename)
        {
            var failureLines = new List<string>();
            bool firstLine = true;

            DataReceivedEventHandler outputHandler = (sender, args) =>
                {
                    string line = args.Data;

                    if (line == null)
                        return;

                    if (firstLine)
                    {
                        firstLine = false;
                        return;
                    }

                    if (line.StartsWith("ok"))
                        return;

                    failureLines.Add(line);
                };


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = filename;
            startInfo.FileName = "Lua.exe";
            startInfo.WorkingDirectory = _scriptPath;

            Process p = new Process();
            p.StartInfo = startInfo;
            p.OutputDataReceived += outputHandler;
            p.ErrorDataReceived += outputHandler;

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();


            if (failureLines.Count > 0)
            {
                string message = "";
                foreach (var line in failureLines)
                    message = message + "      " + line + "\n";
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Check that allocating a string then freeing it doesn't cause the memory 
        /// manager's total allocation size to drift
        /// </summary>
        [Test]
        public void AllocFreeTest()
        {
            var L = Lua.luaL_newstate();

            uint oldUsedMem = Utils.GetUsedMem(L);

            Lua.lua_pushstring(L, "hello world");

            uint newUsedMem = Utils.GetUsedMem(L);
            Assert.AreNotEqual(oldUsedMem, newUsedMem);

            Lua.lua_pop(L, -1);

            uint finalUsedMem = Utils.GetUsedMem(L);
            Assert.AreEqual(oldUsedMem, finalUsedMem);
        }
    }
}
