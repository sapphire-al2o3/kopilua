#!/bin/bash
DLLDIR=Assets/DLL
cp -f ../bin/Release/KopiLua.dll $DLLDIR
cp -f ../bin/Release/KopiLua.pdb $DLLDIR
cp -f ../bin/Release/KopiLuaTest.dll $DLLDIR
cp -f ../bin/Release/KopiLuaTest.pdb $DLLDIR
cp -f ../nunitlite.dll $DLLDIR

cd $DLLDIR
unityeditorexe=`cat "/proc/registry/HKEY_CURRENT_USER/Software/Unity Technologies/Unity Editor 3.x/Location/@"`
unityeditordir=`cygpath -a "$unityeditorexe"/..`
pdb2mdb="${unityeditordir}Data/MonoBleedingEdge/lib/mono/4.0/pdb2mdb.exe"
"$pdb2mdb" KopiLua.dll
"$pdb2mdb" KopiLuaTest.dll

