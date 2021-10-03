using UnityEngine;
using System.Collections;

public interface IInputListener {

    // returns true if the command was parsed and further parsing should stop
    bool OnCommand(InputManager.Command command, InputManager.Event eventType);

}
