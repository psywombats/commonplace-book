using UnityEngine;
using System.Collections;
using System;

public abstract class FadeComponent : MonoBehaviour {

    public const string DefaultTransitionTag = "default";

    public IEnumerator TransitionRoutine(TransitionData transition, Action intermediate = null) {
        yield return StartCoroutine(FadeRoutine(transition.GetFadeOut()));
        intermediate?.Invoke();
        yield return StartCoroutine(FadeRoutine(transition.GetFadeIn(), true));
    }

    public abstract IEnumerator FadeRoutine(FadeData fade, bool invert = false, float timeMult = 1.0f);

    public IEnumerator FadeOutRoutine(string tag = DefaultTransitionTag) {
        TransitionData data = IndexDatabase.Instance().Transitions.GetData(tag);
        yield return FadeRoutine(data.GetFadeOut());
    }

    public IEnumerator FadeInRoutine(string tag = DefaultTransitionTag, float timeMult = 1) {
        TransitionData data = IndexDatabase.Instance().Transitions.GetData(tag);
        yield return FadeRoutine(data.GetFadeIn(), true, timeMult);
    }
}
