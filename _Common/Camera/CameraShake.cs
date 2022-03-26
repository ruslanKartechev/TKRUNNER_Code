using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commongame
{
    public class CameraShake : ShakingHandler
    {
        public void SetTarget(Transform _target)
        {
            target = _target;
        }

    }
}
