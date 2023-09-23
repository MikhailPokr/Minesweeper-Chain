using UnityEngine;

namespace SapperChain
{
    internal class Mark : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        public void Set(int chain)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            gameObject.SetActive(true);
            if (chain > 8)
                chain = 8;
            _spriteRenderer.sprite = BoardManager.Instance.SpriteManager.ChainMarks[chain - 1];
        }
    }
}