using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A set of functions for chaining together coroutines like they were promises
/// </summary>
/// <remarks>
/// Can we get rid of this and replac it with tasks finally?
/// </remarks>
public static class CoUtils {
    
    public static IEnumerator RunAfterDelay(float delayInSeconds, Action toRun) {
        yield return new WaitForSeconds(delayInSeconds);
        toRun();
    }

    public static IEnumerator Delay(float delayInSeconds, IEnumerator toRun) {
        yield return Wait(delayInSeconds);
        yield return toRun;
    }

    public static IEnumerator RunParallel(MonoBehaviour runner, params IEnumerator[] routines) {
        return RunParallel(routines, runner);
    }
    public static IEnumerator RunParallel(IEnumerable<IEnumerator> coroutines, MonoBehaviour runner) {
        int running = coroutines.Count();
        foreach (IEnumerator coroutine in coroutines) {
            runner.StartCoroutine(RunWithCallback(coroutine, () => {
                running -= 1;
            }));
        }
        while (running > 0) {
            yield return null;
        }
    }

    public static IEnumerator RunSequence(IEnumerable<IEnumerator> coroutines) {
        foreach (IEnumerator routine in coroutines) {
            yield return routine;
        }
    }

    public static IEnumerator RunWithCallback(IEnumerator coroutine, Action toRun) {
        yield return coroutine;
        toRun?.Invoke();
    }

    public static IEnumerator Wait(float seconds) {
        yield return new WaitForSeconds(seconds);
    }

    public static IEnumerator RunTween(Tween tween) {
        bool done = false;
        tween.Play().onComplete = () => {
            done = true;
        };
        while (!done) {
            yield return null;
        }
    }

    // resizes the vector via a step function that moves [step] pixels at a time
    public static IEnumerator StepResize(Action<Vector2> setter, Vector2 at, Vector2 to, int step, float duration) {
        float elapsed = 0.0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            Vector2 delta = (to - at) * (elapsed / duration);
            delta = delta / step;
            delta.x = Mathf.Floor(delta.x);
            delta.y = Mathf.Floor(delta.y);
            delta *= step;
            setter(at + delta);
            yield return null;
        }
        setter(to);
    }

    // resizes the vector via a step function that moves time in [step] distinct increments
    public static IEnumerator StepResize2(Action<Vector2> setter, Vector2 at, Vector2 to, int step, float duration) {
        float elapsed = 0.0f;
        while (elapsed < duration) {
            elapsed += Time.fixedDeltaTime;
            var t = Mathf.Floor((elapsed / duration) * step) / step;
            if (t > 1) t = 1;
            setter(to * t + at * (1f - t));
            yield return null;
        }
        setter(to);
    }

    public static IEnumerator TaskAsRoutine(Task task) {
        var oldInstance = Global.Instance().InstanceID;
        while (!task.IsCompleted && Global.Instance().InstanceID == oldInstance) {
            yield return null;
        }
        if (Global.Instance().InstanceID != oldInstance) {
            // we reset
        } else if (task.Exception != null) {
            throw task.Exception;
        }
    }

    public async static Task RoutineAsTask(IEnumerator routine) {
        await routine;
    }
}
