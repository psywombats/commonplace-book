using MoonSharp.Interpreter;
using System;
using UnityEngine;

[MoonSharpUserData]
public class LuaMapEvent {

    public DynValue luaValue { get; private set; }

    private MapEvent mapEvent;
    private LuaContext context;

    public LuaMapEvent(MapEvent mapEvent) {
        this.mapEvent = mapEvent;
        context = Global.Instance().Maps.Context;
        luaValue = context.CreateObject();
    }

    // meant to be called with the key/value of a lualike property on a Tiled object
    // accepts nil and zero-length as no-ops
    [MoonSharpHidden]
    public void Set(string name, string luaChunk) {
        if (luaChunk != null && luaChunk.Length > 0) {
            luaValue.Table.Set(name, context.Load(luaChunk));
        }
    }

    [MoonSharpHidden]
    public void Run(string eventName, Action callback = null) {
        DynValue function = luaValue.Table.Get(eventName);
        if (function == DynValue.Nil) {
            callback?.Invoke();
        } else {
            LuaScript script = new LuaScript(context, function);
            Global.Instance().StartCoroutine(CoUtils.RunWithCallback(script.RunRoutine(true), callback));
        }
    }

    [MoonSharpHidden]
    public DynValue Evaluate(string propertyName) {
        DynValue function = luaValue.Table.Get(propertyName);
        if (function == DynValue.Nil) {
            return DynValue.Nil;
        } else {
            return context.Evaluate(function);
        }
    }

    [MoonSharpHidden]
    public bool EvaluateBool(string propertyName, bool defaultValue = false) {
        DynValue result = Evaluate(propertyName);
        if (result == DynValue.Nil) {
            return defaultValue;
        } else {
            return result.Boolean;
        }
    }

    // === CALLED BY LUA === 

    public void face(string directionName) {
        mapEvent.GetComponent<CharaEvent>().Facing = OrthoDirExtensions.Parse(directionName);
    }

    public void faceToward(LuaMapEvent other) {
        mapEvent.GetComponent<CharaEvent>().Facing = mapEvent.DirectionTo(other.mapEvent);
    }

    public int x() {
        return mapEvent.position.x;
    }

    public int y() {
        return mapEvent.position.y;
    }

    public void debuglog() {
        Debug.Log("Debug: " + mapEvent.name);
    }
}
