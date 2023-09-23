using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SapperChain
{
    internal class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance { get; private set; }
        public SpriteManager SpriteManager;
        [Space]
        [SerializeField] private GameObject _board;
        [Space]
        [SerializeField] private Tile _tilePrefab;
        private Tile[,] _tiles;
        public Tile[,] Tiles => _tiles;

        public static int X;
        public static int Y;
        public static int Bombs;
        public Vector2Int Size { private set; get; }
        public int BombCount { private set; get; }

        private void Awake()
        {
            Instance = this;
            Genarate(new(X, Y), Bombs);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Tile._flagMode = !Tile._flagMode;
            }
        }
        private void Genarate(Vector2Int size, int bombCount)
        {
            if (size.x * size.y - 9 < bombCount)
                return;
            Tile.Restart();
            Size = size;
            BombCount = bombCount;
            _tiles = new Tile[Size.x, Size.y];
            float lenght = _tilePrefab.GetComponent<SpriteRenderer>().size.x;
            Vector2 offset = new((Size.x - 1) / 2f * lenght, (Size.y - 1) / 2f * lenght);
            
            for (int y = 0; y < Size.y; y++) 
            {
                for (int x = 0; x < Size.x; x++)
                {
                    _tiles[x, y] = Instantiate(_tilePrefab, new Vector3(lenght * x - offset.x, -lenght * y + offset.y, 0), Quaternion.identity, _board.transform);
                    _tiles[x, y].SetPosition(x, y);
                    _tiles[x, y].name = $"tile {x}:{y}";
                }
            }
        }
        public void GenerateValues(Vector2Int firstPosition)
        {
            List<Vector2Int> blockedTiles = new();
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (firstPosition.x + x < 0 || firstPosition.x + x >= Size.x || firstPosition.y + y < 0 || firstPosition.y + y >= Size.y)
                        continue;
                    blockedTiles.Add(new(firstPosition.x + x, firstPosition.y + y));
                }
            }

            Vector2Int[] bombs = new Vector2Int[BombCount];
            List<List<Vector2Int>> bombsChains = new();
            for (int i = 0; i < bombs.Length; i++)
            {
                Vector2Int randomPos;
                do
                {
                    randomPos = new Vector2Int(Random.Range(0, Size.x), Random.Range(0, Size.y));
                }
                while (bombs.Contains(randomPos) || blockedTiles.Contains(randomPos));
                bombs[i] = randomPos;

                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        if (x == 0 && y == 0)
                        {
                            _tiles[bombs[i].x, bombs[i].y].SetNum(-1);
                            continue;
                        }
                        if (!Check(bombs[i].x + x, bombs[i].y + y))
                            continue;

                        _tiles[bombs[i].x + x, bombs[i].y + y].SetDelta(1);
                    }
                }
            }
            for (int i = 0; i < bombs.Length; i++)
            {
                _tiles[bombs[i].x, bombs[i].y].AddChain();
            }
        }

        public static bool Check(int x, int y)
        {
            if (x < 0 || x >= Instance.Size.x || y < 0 || y >= Instance.Size.y)
                return false;
            return true;
        }
    }
}