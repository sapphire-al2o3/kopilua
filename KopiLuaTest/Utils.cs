using KopiLua;

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

    }
}
