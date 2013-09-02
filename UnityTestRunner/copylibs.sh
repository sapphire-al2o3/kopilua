#!/bin/bash
DLLDIR=Assets/DLL
cp -f ../bin/Debug/KopiLua.dll $DLLDIR
cp -f ../bin/Debug/KopiLua.pdb $DLLDIR
cp -f ../bin/Debug/KopiLuaTest.dll $DLLDIR
cp -f ../bin/Debug/KopiLuaTest.pdb $DLLDIR
cp -f ../nunitlite.dll $DLLDIR

cd $DLLDIR
pdb2mdb="/c/Program Files (x86)/Unity4/Editor/Data/Mono/lib/mono/2.0/pdb2mdb.exe"
"$pdb2mdb" KopiLua.dll
"$pdb2mdb" KopiLuaTest.dll

