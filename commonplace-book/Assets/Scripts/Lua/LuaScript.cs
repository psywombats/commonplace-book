using MoonSharp.Interpreter;
using System.Collections;
using UnityEngine;
using Coroutine = MoonSharp.Interpreter.Coroutine;

/// <summary>
/// represents an runnable piece of Lua, usually from an event field
/// </summary>
public class LuaScript {
    
    protected LuaContext context;

    public Coroutine scriptRoutine { get; private set;  }
    public bool done { get; private set; }

    private string name;

    public LuaScript(LuaContext context, string scriptString) {
        this.context = context;

        string fullScript = "return function()\n" + scriptString + "\nend";
        scriptRoutine = context.CreateScript(fullScript);
        name = scriptString.Substring(0, Mathf.Min(80, scriptString.Length));
    }

    public LuaScript(LuaContext context, DynValue function) {
        this.context = context;
        scriptRoutine = context.lua.CreateCoroutine(function).Coroutine;
        name = function.ToString();
    }

    public IEnumerator RunRoutine(bool canBlock) {
        done = false;
        yield return context.RunRoutine(this, canBlock);
        done = true;
    }

    public override string ToString() {
        return name != null ? name : base.ToString();
    }
}
