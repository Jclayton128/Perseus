using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    List<ActorMovement> _actorsOnLevel = new List<ActorMovement>();


    public void RegisterNonPlayerActor(ActorMovement newActor)
    {
        if (_actorsOnLevel.Contains(newActor))
        {
            Debug.Log($"already registered this actor: {newActor}");
        }
        else
        {
            _actorsOnLevel.Add(newActor);
        }
    }
}
