using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Wraps a function and turns it into an input listener
/// </summary>
public class AnonymousListener : IInputListener {

    private Func<InputManager.Command, InputManager.Event, bool> eventResponder;

    public AnonymousListener(Func<InputManager.Command, InputManager.Event, bool> eventResponder) {
        this.eventResponder = eventResponder;
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        return eventResponder(command, eventType);
    }
}
