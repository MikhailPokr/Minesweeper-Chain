using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SapperChain
{
    internal class Menu : MonoBehaviour
    {
        [SerializeField] private InputField _sizeX;
        [SerializeField] private InputField _sizeY;
        [SerializeField] private Text _bombs;
        private float _bombsPercent;

        private void Start()
        {
            _sizeX.onEndEdit.AddListener(SetSizeX);
            _sizeY.onEndEdit.AddListener(SetSizeY);

            SetSizeX(PlayerPrefs.GetInt("SizeX", 10).ToString());
            SetSizeY(PlayerPrefs.GetInt("SizeY", 10).ToString());
            SetBombPercent(PlayerPrefs.GetFloat("Bombs", 0.15f));
        }
        public void SetSizeX(string input)
        {
            int value = int.Parse(input);
            if (value < 0)
            {
                value = -value;
            }
            if (value < 4)
            {
                value = 4;
            }
            BoardManager.X = value;
            _sizeX.text = value.ToString();
            FixBombs();
        }
        public void SetSizeY(string input)
        {
            int value = int.Parse(input);
            if (value < 0)
            {
                value = -value;
            }
            if (value < 4)
            {
                value = 4;
            }
            BoardManager.Y = value;
            _sizeY.text = value.ToString();
            FixBombs();
        }
        public void SetBombPercent(float value)
        {
            _bombsPercent = value;
            FixBombs();
        }
        private void FixBombs()
        {
            BoardManager.Bombs = Mathf.RoundToInt(BoardManager.X * BoardManager.Y * _bombsPercent);
            _bombs.text = BoardManager.Bombs.ToString();
        }

        public void StartGame()
        {
            PlayerPrefs.SetInt("SizeX", BoardManager.X);
            PlayerPrefs.SetInt("SizeY", BoardManager.Y);
            PlayerPrefs.SetInt("Bombs", BoardManager.Bombs);
            PlayerPrefs.Save();

            SceneManager.LoadScene(1);
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}