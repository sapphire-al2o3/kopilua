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
	public void Test (string filename)
	{
		var r = Resources.Load ("Tests/" + filename);
		string s = r.ToString ();
		
		if (s.StartsWith ("#!")) {
			int pos = s.IndexOf ("\n");
			if (pos >= 0)
				s = s.Substring (pos + 1);
		}
		
		var sb = new StringBuilder();
		using (new CaptureConsole(sb))
		{
			var L = Lua.luaL_newstate ();
			Lua.luaL_openlibs (L);
			
			int loadstringResult = Lua.luaL_loadstring (L, s);
			Assert.AreEqual (0, loadstringResult);
	
			int pcallResult = Lua.lua_pcall (L, 0, -1, 0);
			Assert.AreEqual (0, pcallResult);
		}
		
		var errors = new List<string>();
		
		bool first = true;
		foreach (var line in sb.ToString().Split(new char[] { '\n' }))
		{
			if (first)
			{
				first = false;
				continue;
			}
			
			if (line.StartsWith("ok"))
				continue;
			
			if (line.Length == 0)
				continue;
			
			errors.Add(line);
		}
		
		if (errors.Count > 0)
		{
			string message = "";
			foreach (var line in errors)
				message += "      " + line + "\n";
			Assert.Fail(message);
		}
	}
}
