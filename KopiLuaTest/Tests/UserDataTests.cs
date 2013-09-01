using NUnit.Framework;
using KopiLua;

namespace KopiLuaTest.Tests
{
    [TestFixture]
    public class UserDataTests
    {
        /// <summary>
        /// Check that allocating a userdata then freeing it doesn't cause the memory 
        /// manager's total allocation size to drift
        /// </summary>
        [Test]
        public void AllocFreeTest()
        {
            var L = Lua.luaL_newstate();
         
            uint oldUsedMem = Utils.GetUsedMem(L);
            
            object userdata = Lua.lua_newuserdata(L, 4);
            
            uint newUsedMem = Utils.GetUsedMem(L);
            Assert.AreNotEqual(oldUsedMem, newUsedMem);
            
            Lua.lua_pop(L, -1);
            
            uint finalUsedMem = Utils.GetUsedMem(L);
            Assert.AreEqual(oldUsedMem, finalUsedMem);
        }
    }
}
