using UnityEngine;

namespace SmallHedge.SoundManager
{
    [CreateAssetMenu(fileName = "NewSoundsCollection", menuName = "SmallHedge/Sounds Collection")]
    public class SoundsSO : ScriptableObject
    {
        // 現在它與 SoundList 在同一個命名空間，絕對能找到
        public SoundList[] sounds; 
    }
}