using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SapperChain
{
    internal class Menu : MonoBehaviour
    {
        [SerializeField] private InputField _sizeX;
        [SerializeField] private InputField _sizeY;
        [SerializeField] private InputField _bombs;

        private void Start()
        {
            _sizeX.onEndEdit.AddListener(SetSizeX);
            _sizeY.onEndEdit.AddListener(SetSizeY);
            _bombs.onEndEdit.AddListener(SetBombs);

            SetSizeX(PlayerPrefs.GetInt("SizeX", 10).ToString());
            SetSizeY(PlayerPrefs.GetInt("SizeY", 10).ToString());
            SetBombs(PlayerPrefs.GetInt("Bombs", 10).ToString());
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
        public void SetBombs(string input)
        {
            int value = int.Parse(input);
            if (value < 0)
            {
                value = -value;
            }
            BoardManager.Bombs = value;
            _bombs.text = value.ToString();
            FixBombs();
        }
        public void FixBombs()
        {
            if (BoardManager.Bombs < 1)
            {
                BoardManager.Bombs = 1;
            }
            else if (BoardManager.Bombs > (BoardManager.X * BoardManager.Y) - 9)
            {
                BoardManager.Bombs = BoardManager.X * BoardManager.Y - 9;
                _bombs.text = (BoardManager.X * BoardManager.Y - 9).ToString();
            }
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