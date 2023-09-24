using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SapperChain
{
    internal class Options : MonoBehaviour
    {
        public static Options Instance { get; private set; }
        private static float _timer;
        [SerializeField] private Animator _animator;
        [Space]
        [SerializeField] private Text _timerText;
        private static int _flagCount;
        [SerializeField] private Text _bombsText;
        [SerializeField] GameObject _end;
        [SerializeField] Text _winText;
        [SerializeField] InputManager _inputManager;

        private bool _nowShowed;

        public static void StartGame() => _timer = 0;
        public static void RestartGame()
        {
            _timer = -1;
            _flagCount = 0;
        }

        private void Start()
        {
            Instance = this;
            Tile.Flag += OnFlag;
            _bombsText.text = $"{_flagCount}/{BoardManager.Bombs}";
        }

        private void OnFlag(bool set)
        {
            if (set)
                _flagCount++;
            else
                _flagCount--;
            _bombsText.text = $"{_flagCount}/{BoardManager.Bombs}";
        }

        private void Update()
        {
            if (_timer == -1)
                return;
            _timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(_timer / 60);
            int seconds = Mathf.FloorToInt(_timer % 60);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void ShowMore()
        {
            if (!_animator.enabled)
                _animator.enabled = true;
            _nowShowed = !_nowShowed;
            _animator.SetBool("Enabled", _nowShowed);
        }
        public void Restart()
        {
            SceneManager.LoadScene(1);
        }

        public void ToMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void EndGame(bool win)
        {
            _end.SetActive(true);
            _timer = -1;
            if (!win)
                _winText.text = "Lose";
            else
                _winText.text = "Win";
            _inputManager.enabled = false;
        }

        private void OnDisable()
        {
            Tile.Flag -= OnFlag;
        }
    }
}