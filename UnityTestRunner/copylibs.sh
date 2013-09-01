#!/bin/bash
cp -f ../bin/Debug/KopiLua.dll Assets
cp -f ../bin/Debug/KopiLua.pdb Assets
cp -f ../bin/Debug/KopiLuaTest.dll Assets
cp -f ../bin/Debug/KopiLuaTest.pdb Assets

cd Assets
pdb2mdb="/c/Program Files (x86)/Unity/Editor/Data/Mono/lib/mono/2.0/pdb2mdb.exe"
"$pdb2mdb" KopiLua.dll
"$pdb2mdb" KopiLuaTest.dll

