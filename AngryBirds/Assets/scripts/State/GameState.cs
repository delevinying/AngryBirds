using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState{
    public enum SlingshotState {
           Idle,
           UserPulling,
           BirdFlying,
			ReSet
    }

    public enum GamingState {
        Start,
        BirdMovingToSlingout,
        Playing,
        Won,
        Lost
    }

    public enum BirdState
    {
        BeforeThrown,
        Thrown
    }
}
