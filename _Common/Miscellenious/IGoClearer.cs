using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commongame {
    public interface IGoClearer
    {
        void ClearWithDelay(Transform target, float time);
        void Clear(Transform target);
    }
}