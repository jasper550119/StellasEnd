using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmallHedge.SoundManager
{
    public class PlayFootstep : MonoBehaviour
    {
        public void Play()
        {
            SoundManager.PlaySound(SoundType.Run);
        }   
    }
}

