using System.Collections;
using System;
using MoonSharp.Interpreter;
using System.Threading.Tasks;

public class LuaCutsceneContext : LuaContext {

    private static readonly string DefinesPath = "Lua/Defines/CutsceneDefines";

    public override IEnumerator RunRoutine(LuaScript script, bool canBlock) {
        if (canBlock && Global.Instance().Maps.Avatar != null) {
            Global.Instance().Maps.Avatar.PauseInput();
        }
        var oldInstance = Global.Instance().InstanceID;
        yield return base.RunRoutine(script, canBlock);
        if (Global.Instance().InstanceID != oldInstance) yield break;
        //if (MapOverlayUI.Instance().textbox.isDisplaying && canBlock) {
        //    yield return MapOverlayUI.Instance().textbox.DisableRoutine();
        //}
        if (canBlock && Global.Instance().Maps.Avatar != null) {
            Global.Instance().Maps.Avatar.UnpauseInput();
        }
    }

    public override void Initialize() {
        base.Initialize();
        LoadDefines(DefinesPath);
    }

    protected override async Task RunRoutineFromLuaInternal(IEnumerator routine) {
        //if (MapOverlayUI.Instance().textbox.isDisplaying) {
        //    MapOverlayUI.Instance().textbox.MarkHiding();
        //    await base.RunRoutineFromLuaInternal(CoUtils.RunSequence(new IEnumerator[] {
        //        MapOverlayUI.Instance().textbox.DisableRoutine(),
        //        routine,
        //    }));
        //} else {
            await base.RunRoutineFromLuaInternal(routine);
        //}
    }

    public async void RunTextboxRoutineFromLua(IEnumerator routine) {
        await base.RunRoutineFromLuaInternal(routine);
    }

    protected void ResumeNextFrame() {
        Global.Instance().StartCoroutine(ResumeRoutine());
    }
    protected IEnumerator ResumeRoutine() {
        yield return null;
        ResumeAwaitedScript();
    }

    protected override void AssignGlobals() {
        base.AssignGlobals();
        lua.Globals["playBGM"] = (Action<DynValue>)PlayBGM;
        lua.Globals["playSound"] = (Action<DynValue>)PlaySound;
        lua.Globals["sceneSwitch"] = (Action<DynValue, DynValue>)SetSwitch;
        lua.Globals["face"] = (Action<DynValue, DynValue>)Face;
        lua.Globals["cs_teleport"] = (Action<DynValue, DynValue, DynValue, DynValue>)TargetTeleport;
        lua.Globals["cs_targetTele"] = (Action<DynValue, DynValue, DynValue, DynValue>)TargetTeleport;
        lua.Globals["cs_fadeOutBGM"] = (Action<DynValue>)FadeOutBGM;
    }

    // === LUA CALLABLE ============================================================================

    private void PlayBGM(DynValue bgmKey) {
        Global.Instance().Audio.PlayBGM(bgmKey.String);
    }

    private void PlaySound(DynValue soundKey) {
        Global.Instance().Audio.PlaySFX(soundKey.String);
    }

    private void TargetTeleport(DynValue mapName, DynValue targetEventName, DynValue facingLua, DynValue rawLua) {
        OrthoDir? facing = null;
        if (!facingLua.IsNil()) facing = OrthoDirExtensions.Parse(facingLua.String);
        var raw = rawLua.IsNil() ? false : rawLua.Boolean;
        RunRoutineFromLua(Global.Instance().Maps.TeleportRoutine(mapName.String, targetEventName.String));
    }

    private void FadeOutBGM(DynValue seconds) {
        RunRoutineFromLua(Global.Instance().Audio.FadeOutRoutine((float)seconds.Number));
    }

    private void Face(DynValue eventName, DynValue dir) {
        var @event = Global.Instance().Maps.ActiveMap.GetEventNamed(eventName.String);
        if (@event == null) {
            Global.Instance().Debug.LogError("Couldn't find face event " + eventName.String);
        } else {
            @event.GetComponent<CharaEvent>().Facing = OrthoDirExtensions.Parse(dir.String);
        }
    }
}
