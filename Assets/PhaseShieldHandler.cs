using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseShieldHandler : MonoBehaviour
{
    Transform _transformToFollow;
   public void SetFollowTarget(Transform target)
    {
        _transformToFollow = target;
    }

    private void Update()
    {
        if (_transformToFollow)
        {
            transform.position = _transformToFollow.position;
            transform.rotation = _transformToFollow.rotation;
        }
    }
}
