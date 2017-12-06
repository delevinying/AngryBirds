using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState{
    public enum SlingshotState {
           Idle,
           UserPulling,
           BirdFlying
    }

    public enum GamingState {
        Start,
        BirdMovingToSlingout,
        Playing,
        Won,
        Lost
    }

    public enum BulletState
    {
		Idle,
        BeforeThrown,
        Thrown
    }
}
