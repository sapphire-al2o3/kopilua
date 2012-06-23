using NUnit.Framework;
using KopiLua;

namespace KopiLuaTest.Tests
{
    [TestFixture]
    public class StringTests
    {
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
