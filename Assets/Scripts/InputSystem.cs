using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Ideally these inputs are supposed to working together with the custom inputs user defined. But we dont have custom inputs or multiple different input bindings. So this is hacky for now. 
namespace GTA
{
    public enum eInputAction
    {
        MOVE_FORWARD,
        MOVE_BACKWARD,
        STRAFE_LEFT,
        STRAFE_RIGHT,

        JUMP,
        RELOAD,

        ACTION_1,       // Depends on context 
        ACTION_2,        // Depends on context

        SWITCH_PLAYER_CAMERA,

        TOGGLE_DEBUG_PANEL
    }
}

