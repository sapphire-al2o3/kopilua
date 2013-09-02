using KopiLua;
using NUnit.Framework;
using NUnitLite.Runner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using KopiLuaTest;

public class Runner : MonoBehaviour
{
	private string _result = "";
	
	void Start ()
	{
		var sb = new StringBuilder();
		using (new CaptureConsole(sb))
		{
			//KopiLuaTest.Program.Execute (new TextUI (w), new string[] { "-noh" });
			new TextUI().Execute (new string[] { "-noh" });
		}
		_result = sb.ToString ();
	}
	
	void OnGUI ()
	{
		GUILayout.TextArea (_result);
	}
}

[TestFixture]
public class Tests
{
	[TestCase("ultrasimple")]
	[TestCase("000-sanity")]
	[TestCase("001-if")]
	[TestCase("002-table")]
	[TestCase("011-while")]
	[TestCase("012-repeat")]
	[TestCase("014-fornum")]
	[TestCase("015-forlist")]
	[TestCase("101-boolean")]
	[TestCase("102-function")]
	[TestCase("103-nil")]
	[TestCase("104-number")]
	[TestCase("105-string")]
	[TestCase("106-table")]
	[TestCase("107-thread")]
	[TestCase("108-userdata")]
	[TestCase("200-examples")]
	[TestCase("201-assign")]
	[TestCase("202-expr")]
	[TestCase("203-lexico")]
	[TestCase("211-scope")]
	[TestCase("212-function")]
	[TestCase("213-closure")]
	[TestCase("214-coroutine")]
	[TestCase("221-table")]
	[TestCase("222-constructor")]
	[TestCase("223-iterator")]
	[TestCase("231-metatable")]
	[TestCase("232-object")]
	////[TestCase("241-standalone")]
	[TestCase("301-basic")]
	[TestCase("303-package")]
	[TestCase("304-string")]
	[TestCase("305-table")]
	[TestCase("306-math")]
	[TestCase("307-io")]
	//[TestCase("308-os")]
	[TestCase("309-debug")]
	////[TestCase("310-stdin")]
	[TestCase("314-regex")]
	public void Test (string filename)
	{
		// Set up the FOpen hook so Lua can read scripts and other data from the Resources/Tests folder,
		// and also write to a virtual filesystem
		UnityFOpen.ResourceRootPath = "Tests";
		KopiLua.Lua.fopenDelegate = UnityFOpen.FOpen;
		
		// Capture console output during the run so we can sweep it for errors afterwards
		var sb = new StringBuilder();
		using (new CaptureConsole(sb))
		{
			var L = Lua.luaL_newstate();
			Lua.luaL_openlibs(L);
			
			// Set up the 'arg' array so some of the scripts know they're running under Unity
			CheckLua(L, Lua.luaL_loadstring(L, "arg={}\narg[-1] = 'unity'\narg[0] = '" + filename + "'"));
			CheckLua(L, Lua.lua_pcall (L, 0, -1, 0));
			
			// Load and execute the script
			CheckLua(L, Lua.luaL_loadfile(L, filename));
			CheckLua(L, Lua.lua_pcall (L, 0, -1, 0));
		}
		
		// Check for errors in the output - anything that doesn't start with "ok" is an error line.
		// Note that we don't get here at all if an earlier assert failed, e.g. there was a Lua 
		// syntax error or an exception was thrown during execution.  It is unfortunate and may be
		// worth fixing.
		
		var errors = new List<string>();
		var lines = sb.ToString().Split(new char[] { '\n' });
		
		int firstError = -1;
		for (int i = 0; i < lines.Length; ++i)
		{
			var line = lines[i];

			if (i == 0)
				continue;
			
			if (line.StartsWith("ok"))
				continue;
			
			if (line.StartsWith("#"))
				continue;
			
			if (line.Length == 0)
				continue;
			
			errors.Add(line);
			if (firstError == -1)
				firstError = i;
		}
		
		if (errors.Count > 0)
		{
			string message = "";
			for (int i = firstError - 10; i <= firstError; ++i)
				if (i >= 0)
					message += "      " + lines[i] + "\n";
			Assert.Fail(message);
		}
	}
	
	// Check a Lua result code and add the error string to the assert if the operation failed
	private void CheckLua(Lua.lua_State L, int luaResult)
	{
		if (luaResult != 0)
			Assert.Fail(Lua.lua_tostring(L, -1).ToString());
	}
}
