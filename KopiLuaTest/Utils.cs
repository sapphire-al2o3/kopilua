using KopiLua;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace KopiLuaTest
{
    public static class Utils
    {
        /// <summary>
        /// Get the amount of memory used, according to the Lua memory manager
        /// </summary>
        /// <returns>Number of bytes used</returns>
        public static uint GetUsedMem(Lua.lua_State L)
        {
            // Perform a full GC pass first
            Lua.luaC_fullgc(L);

            return L.l_G.totalbytes;
        }

        public static void DumpStack(Lua.lua_State L)
        {
            for (int i = -Lua.lua_gettop(L); i < 0; ++i)
            {
                string s = "?";
                int t = Lua.lua_type(L, i);
                switch (t)
                {
                    case Lua.LUA_TSTRING:
                        s = Lua.lua_tostring(L, i).ToString();
                        break;
                    case Lua.LUA_TBOOLEAN:
                        s = Lua.lua_toboolean(L, i) != 0 ? "true" : "false";
                        break;
                    case Lua.LUA_TNUMBER:
                        s = Lua.lua_tonumber(L, i).ToString();
                        break;
                }
                Debug.WriteLine(string.Format("{0}: {1} {2}", i, Lua.lua_typename(L, t), s));
            }
        }
    }

    /// <summary>
    /// Capture the System.Console output temporarily
    /// </summary>
    public class CaptureConsole : IDisposable
    {
        private TextWriter _oldTextWriter;
        private TextWriter _newTextWriter;

        public CaptureConsole(StringBuilder sb)
            : this(new StringWriter(sb))
        {
        }

        public CaptureConsole(TextWriter writer)
        {
            _oldTextWriter = System.Console.Out;
            _newTextWriter = writer;
            System.Console.SetOut(_newTextWriter);
        }

        public void Dispose()
        {
            System.Console.SetOut(_oldTextWriter);
            _newTextWriter.Dispose();
        }
    }
}
