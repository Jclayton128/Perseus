using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerSeeking
{
    public float  PlayerRange { get; }

    public void ReportPlayer(Vector2 playerPosition, Vector2 playerVelocity);
}
