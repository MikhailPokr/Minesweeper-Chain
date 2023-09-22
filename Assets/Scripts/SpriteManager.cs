using UnityEngine;

namespace SapperChain
{
    [CreateAssetMenu(fileName = "TileSprites")]
    internal class SpriteManager : ScriptableObject
    {
        public Sprite Close;
        public Sprite Bomb;
        public Sprite Flag;
        public Sprite[] Numbers;
        [Space]
        public Sprite[] ChainMarks;
        public Sprite[] Chains;
    }
}

