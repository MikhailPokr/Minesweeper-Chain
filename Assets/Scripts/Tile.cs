using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SapperChain
{
    internal class Tile : MonoBehaviour
    {
        private static bool _flagMode = true;
        private static bool _firstMove = true;
        private Vector2Int _position;

        private int _chainIndex = 0;

        [SerializeField] private Mark _mark1;
        [SerializeField] private Mark _mark2;
        [SerializeField] private Mark _mark3;
        [SerializeField] private Mark _mark4;

        private SpriteRenderer _spriteRenderer;

        private bool _open = false;
        private bool _flag = false;
        private int _num = 0;
        public bool ItsBomb => _num == -1;

        public delegate void FlagHandler(bool set);
        public static event FlagHandler Flag;

        public void SetPosition(int x, int y)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _position = new(x, y);
            InputManager.Click += OnClick;
        }

        public static void ChangeMode() => _flagMode = !_flagMode;
        public static void ChangeMode(bool on) => _flagMode = on;

        private void OnClick(bool hold, Vector2 pos)
        {
            if (Vector2.Distance(transform.position, pos) > _spriteRenderer.size.x / 2)
                return;

            if (_firstMove)
            {
                _firstMove = false;
                BoardManager.Instance.GenerateValues(_position);
                Open(true);
                Options.StartGame();
                return;
            }

            if ((_flagMode && hold) || (!_flagMode && !hold))
            {
                if (_flag)
                    return;
                if (!_open)
                    Open();
                else
                    OpenNear();
            }
            else if ((_flagMode && !hold) || (!_flagMode && hold))
            {
                if (!_open)
                    SetFlag(!_flag);
                else
                    OpenNear();
            }
        }

        public void Open(bool open = true)
        {
            if (_open == open)
                return;
            _open = open;
            if (_num == 0)
                OpenNear();
            if (_num > 0)
            {
                List<Tile> bombs = new List<Tile>();
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        if (x == 0 && y == 0 || !BoardManager.Check(_position.x + x, _position.y + y))
                            continue;
                        Tile tile = BoardManager.Instance.Tiles[_position.x + x, _position.y + y];
                        if (tile.ItsBomb)
                            bombs.Add(tile);
                    }
                }

                List<List<Tile>> list = new List<List<Tile>>();
                bool[] visited = new bool[bombs.Count];

                for (int i = 0; i < bombs.Count; i++)
                {
                    if (visited[i])
                        continue;

                    List<Tile> bombGroup = new List<Tile>();
                    DFS(bombs, visited, i, bombGroup);
                    list.Add(bombGroup);
                }

                switch (list.Count)
                {
                    case 1:
                        _mark1.Set(list[0][0]._chainIndex);
                        break;
                    case 2:
                        _mark2.Set(list[1][0]._chainIndex);
                        goto case 1;
                    case 3:
                        _mark3.Set(list[2][0]._chainIndex);
                        goto case 2;
                    case 4:
                        _mark4.Set(list[3][0]._chainIndex);
                        goto case 3;
                }
            }

            if (_num == -1)
            {
                Options.Instance.EndGame(false);
            }

            UpdateView();
        }

        private void DFS(List<Tile> bombs, bool[] visited, int currentIndex, List<Tile> bombGroup)
        {
            visited[currentIndex] = true;
            bombGroup.Add(bombs[currentIndex]);

            for (int j = 0; j < bombs.Count; j++)
            {
                if (!visited[j] && Vector2.Distance(bombs[currentIndex]._position, bombs[j]._position) == 1)
                {
                    DFS(bombs, visited, j, bombGroup);
                }
            }
        }

        public void SetFlag(bool flag)
        {
            _flag = flag;
            Flag?.Invoke(flag);
            UpdateView();
        }
        public void SetDelta(int delta)
        {
            if (_num == -1)
                return;
            SetNum(_num + delta);
        }
        public void SetNum(int num)
        {
            _num = num;
        }
        public void OpenNear()
        {
            List<Tile> near = new List<Tile>();
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0 || !BoardManager.Check(_position.x + x, _position.y + y))
                        continue;
                    near.Add(BoardManager.Instance.Tiles[_position.x + x, _position.y + y]);
                }
            }
            if (near.Count(x => x._flag) == _num)
            {
                foreach (Tile tile in near) 
                {
                    if (tile._flag)
                        continue;
                    tile.Open();
                }
            }
        }
        public void AddChain()
        {
            if (_num != -1) 
                return;
            _chainIndex++;
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (Mathf.Abs(x + y) != 1 || !BoardManager.Check(_position.x + x, _position.y + y))
                        continue;
                    Tile tile = BoardManager.Instance.Tiles[_position.x + x, _position.y + y];
                    if (!tile.ItsBomb || tile._chainIndex == _chainIndex)
                        continue;
                    tile.AddChain();
                }
            }
        }

        public static void Restart()
        {
            _firstMove = true;
        }

        public bool Check() => (_flag && _num == -1) || _open;
        public void UpdateView()
        {
            if (_flag)
            {
                _spriteRenderer.sprite = BoardManager.Instance.SpriteManager.Flag;
            }
            else if (!_open)
            {
                _spriteRenderer.sprite = BoardManager.Instance.SpriteManager.Close;
            }
            else
            {
                if (_num == -1)
                {
                    _spriteRenderer.sprite = BoardManager.Instance.SpriteManager.Bomb;
                    return;
                }
                _spriteRenderer.sprite = BoardManager.Instance.SpriteManager.Numbers[_num];
            }

            if (BoardManager.Instance.CheckWin())
            {
                Options.Instance.EndGame(true);
            }
        }
        private void OnDisable()
        {
            InputManager.Click -= OnClick;
        }
    }
}