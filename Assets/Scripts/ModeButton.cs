using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SapperChain
{
    internal class ModeButton : MonoBehaviour
    {
        private bool _mode;
        [SerializeField] private Image _modeButton;
        [SerializeField] private Sprite _on;
        [SerializeField] private Sprite _off;
        private void Start()
        {
            _mode = PlayerPrefs.GetInt("Mode", 0) == 1;
            ChangeMode();
        }
        public void ChangeMode()
        {
            _mode = !_mode;
            if (_mode) 
            {
                _modeButton.sprite = _on;
            }
            else
            {
                _modeButton.sprite = _off;
            }
            PlayerPrefs.SetInt("Mode", _mode ? 0 : 1);
            PlayerPrefs.Save();
            Tile.ChangeMode(_mode);
        }
    }
}