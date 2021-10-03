using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// represents an eval-able piece of Lua, usually from an event field
/// </summary>
public class LuaCondition {

    private LuaContext context;
    private DynValue function;

	public LuaCondition(LuaContext context, DynValue scriptFunction) {
        this.function = scriptFunction;
        this.context = context;
    }

    public DynValue Evaluate() {
        return context.Evaluate(function);
    }
}
