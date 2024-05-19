using System.Text.Json;
using Silk.NET.Maths;
using Silk.NET.SDL;
using TheAdventure.Models;
using System;
using System.Media;

namespace TheAdventure
{

    public class Engine
    {
        double lung = 0.0001;
        private readonly Dictionary<int, GameObject> _gameObjects = new();
        private readonly Dictionary<string, TileSet> _loadedTileSets = new();

        private Level? _currentLevel;
        private PlayerObject _player;
        string fundal1 = @"music1.wav";
        string fundal2 = @"usa1.wav";
        //private PlayerObject _casa;
        private GameRenderer _renderer;

        //private GameRenderer _rendererFridge;
        private TemporaryGameObject _casa;
        private TemporaryGameObject _dialog;
        private Input _input;

        private DateTimeOffset _lastUpdate = DateTimeOffset.Now;
        private DateTimeOffset _lastPlayerUpdate = DateTimeOffset.Now;
        public static int x = 0;

        public Engine(GameRenderer renderer, Input input)
        {
            _renderer = renderer;

            //_rendererFridge=renderer1;
            _input = input;
            bool Ep = _input.IsEPressed();
            //if(Ep){
            _input.OnMouseClick += (_, coords) => AddBomb(coords.x, coords.y);
            if (x == 0)
            {
                x = 1;
                _input.OnMouseClick += (_, coords) => dialogueFridge(coords.x, coords.y);

            }

            System.Threading.Thread.Sleep(2000);
            _input.OnMouseClick += (_, coords) => typewriting();
            // }
            // System.Threading.Thread.Sleep(1000);
            //_input.OnMouseClick+= (_,coords) => dialogueFridge2(coords.x,coords.y);
            //_input.OnSpa += (_, coords) => AddBomb(coords.x, coords.y);
        }
        public void typewriting()
        {
            PlayMusic2(fundal2);

            System.Threading.Thread.Sleep(3000);
            PlayMusic(fundal1);
        }

        public void InitializeWorld()
        {
            var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var levelContent = File.ReadAllText(Path.Combine("Assets", "terrain.tmj"));

            var level = JsonSerializer.Deserialize<Level>(levelContent, jsonSerializerOptions);
            if (level == null) return;
            foreach (var refTileSet in level.TileSets)
            {
                var tileSetContent = File.ReadAllText(Path.Combine("Assets", refTileSet.Source));
                if (!_loadedTileSets.TryGetValue(refTileSet.Source, out var tileSet))
                {
                    tileSet = JsonSerializer.Deserialize<TileSet>(tileSetContent, jsonSerializerOptions);

                    foreach (var tile in tileSet.Tiles)
                    {
                        var internalTextureId = _renderer.LoadTexture(Path.Combine("Assets", tile.Image), out _);
                        tile.InternalTextureId = internalTextureId;
                    }

                    _loadedTileSets[refTileSet.Source] = tileSet;
                }

                refTileSet.Set = tileSet;
            }

            _currentLevel = level;
            SpriteSheet spriteSheet = new(_renderer, Path.Combine("Assets", "player.png"), 10, 6, 48, 48, (24, 42));
            spriteSheet.Animations["IdleDown"] = new SpriteSheet.Animation()
            {
                StartFrame = (0, 0),
                EndFrame = (0, 5),
                DurationMs = 1000,
                Loop = true
            };
            _player = new PlayerObject(spriteSheet, 100, 100);
            PlayMusic(fundal1);
            //initializare CasaAfara
            SpriteSheet spriteSheet2 = new(_renderer, "Assets/casaAfara.png", 1, 13, 360, 360, (180, 180));

            _casa = new(spriteSheet2, lung, (300, 300));
            _gameObjects.Add(_casa.Id, _casa);
            //finish InitializarecasaAfara
            _renderer.SetWorldBounds(new Rectangle<int>(0, 0, _currentLevel.Width * _currentLevel.TileWidth,
                _currentLevel.Height * _currentLevel.TileHeight));

        }
        public static void PlayMusic(string path)
        {
            System.Media.SoundPlayer musicPlayer = new System.Media.SoundPlayer();
            musicPlayer.SoundLocation = path;
            musicPlayer.PlayLooping();

        }

        public void ProcessFrame()
        {
            var currentTime = DateTimeOffset.Now;
            var secsSinceLastFrame = (currentTime - _lastUpdate).TotalSeconds;
            _lastUpdate = currentTime;

            bool up = _input.IsUpPressed();
            bool down = _input.IsDownPressed();
            bool left = _input.IsLeftPressed();
            bool right = _input.IsRightPressed();
            int x = _player.GetPosition().X;
            int y = _player.GetPosition().Y;
            _player.UpdatePlayerPosition(up ? 1.0 : 0.0, down ? 1.0 : 0.0, left ? 1.0 : 0.0, right ? 1.0 : 0.0,
                _currentLevel.Width * _currentLevel.TileWidth, _currentLevel.Height * _currentLevel.TileHeight,
                secsSinceLastFrame);

            if (x != _player.GetPosition().X || y != _player.GetPosition().Y)
            {



                if (_player.GetPosition().X > 170 && _player.GetPosition().X < 400 && _player.GetPosition().Y > 190 && _player.GetPosition().Y < 400)
                {


                    if (!(_player.GetPosition().X > 170 && _player.GetPosition().X < 230 && _player.GetPosition().Y > 190 && _player.GetPosition().Y < 230))
                    {

                        SpriteSheet spriteSheet2 = new(_renderer, "Assets/casaInauntru.png", 1, 13, 360, 360, (180, 180));

                        _casa = new(spriteSheet2, lung, (300, 300));
                        _gameObjects.Add(_casa.Id, _casa);
                    }

                    else
                    {
                        SpriteSheet spriteSheet2 = new(_renderer, "Assets/casaAfara.png", 1, 13, 360, 360, (180, 180));
                        _casa = new(spriteSheet2, lung, (300, 300));
                        _gameObjects.Add(_casa.Id, _casa);

                    }

                }
                else
                {
                    //initializare CasaAfara
                    SpriteSheet spriteSheet2 = new(_renderer, "Assets/casaAfara.png", 1, 13, 360, 360, (180, 180));
                    _casa = new(spriteSheet2, lung, (300, 300));
                    _gameObjects.Add(_casa.Id, _casa);
                }
                Console.WriteLine(_player.GetPosition().X + "  , " + _player.GetPosition().Y);
                /*
               if(_player.GetPosition().X > 350 && _player.GetPosition().X<360 && _player.GetPosition().Y > 315 && _player.GetPosition().Y < 325 ){
                                SpriteSheet spriteSheet3 = new(_renderer, "Assets/dia.png", 2, 10, 497, 190, (250,50));
                                _dialog=new(spriteSheet3,lung, (400,450));
                                _gameObjects.Add(_dialog.Id, _dialog);
                     } 
               */

                var itemsToRemove = new List<int>();
                itemsToRemove.AddRange(GetAllTemporaryGameObjects().Where(gameObject => gameObject.IsExpired)
                    .Select(gameObject => gameObject.Id).ToList());

                foreach (var gameObject in itemsToRemove)
                {
                    _gameObjects.Remove(gameObject);
                }
            }

        }
        public void dialogueFridge(int x, int y)
        {

            if (_player.GetPosition().X > 340 && _player.GetPosition().X < 370 && _player.GetPosition().Y > 300 && _player.GetPosition().Y < 330)
            {
                SpriteSheet spriteSheet3 = new(_renderer, "Assets/Dialog1.png", 2, 10, 497, 150, (250, 50));
                _dialog = new(spriteSheet3, 2, (400, 450));
                _gameObjects.Add(_dialog.Id, _dialog);
                //System.Threading.Thread.Sleep(3000);

                //System.Threading.Thread.Sleep(3000);

                //dialogueFridge2(x,y);
                //System.Threading.Thread.Sleep(3000);
            }
        }
        public static void PlayMusic2(string path)
        {
            System.Media.SoundPlayer musicPlayer = new System.Media.SoundPlayer();
            musicPlayer.SoundLocation = path;
            musicPlayer.Play();
        }

        public void dialogueFridge2(int x, int y)
        {
            if (_player.GetPosition().X > 340 && _player.GetPosition().X < 370 && _player.GetPosition().Y > 300 && _player.GetPosition().Y < 330)
            {
                SpriteSheet spriteSheet3 = new(_renderer, "Assets/Dialog2.png", 2, 10, 497, 150, (250, 50));
                _dialog = new(spriteSheet3, 2, (400, 450));
                _gameObjects.Add(_dialog.Id, _dialog);
                //System.Threading.Thread.Sleep(3000);

                //System.Threading.Thread.Sleep(3000);

                //dialogueFridge2(x,y);
                //System.Threading.Thread.Sleep(3000);
            }
        }

        public void RenderFrame()
        {
            _renderer.SetDrawColor(0, 0, 0, 255);
            _renderer.ClearScreen();

            _renderer.CameraLookAt(_player.Position.X, _player.Position.Y);

            RenderTerrain();
            RenderAllObjects();

            _renderer.PresentFrame();
        }

        private Tile? GetTile(int id)
        {
            if (_currentLevel == null) return null;
            foreach (var tileSet in _currentLevel.TileSets)
            {
                foreach (var tile in tileSet.Set.Tiles)
                {
                    if (tile.Id == id)
                    {
                        return tile;
                    }
                }
            }

            return null;
        }

        private void RenderTerrain()
        {
            if (_currentLevel == null) return;
            for (var layer = 0; layer < _currentLevel.Layers.Length; ++layer)
            {
                var cLayer = _currentLevel.Layers[layer];

                for (var i = 0; i < _currentLevel.Width; ++i)
                {
                    for (var j = 0; j < _currentLevel.Height; ++j)
                    {
                        var cTileId = cLayer.Data[j * cLayer.Width + i] - 1;
                        var cTile = GetTile(cTileId);
                        if (cTile == null) continue;

                        var src = new Rectangle<int>(0, 0, cTile.ImageWidth, cTile.ImageHeight);
                        var dst = new Rectangle<int>(i * cTile.ImageWidth, j * cTile.ImageHeight, cTile.ImageWidth,
                            cTile.ImageHeight);

                        _renderer.RenderTexture(cTile.InternalTextureId, src, dst);
                    }
                }
            }
        }

        private IEnumerable<RenderableGameObject> GetAllRenderableObjects()
        {
            foreach (var gameObject in _gameObjects.Values)
            {
                if (gameObject is RenderableGameObject renderableGameObject)
                {
                    yield return renderableGameObject;
                }
            }
        }

        private IEnumerable<TemporaryGameObject> GetAllTemporaryGameObjects()
        {
            foreach (var gameObject in _gameObjects.Values)
            {
                if (gameObject is TemporaryGameObject temporaryGameObject)
                {
                    yield return temporaryGameObject;
                }
            }
        }

        private void RenderAllObjects()
        {
            foreach (var gameObject in GetAllRenderableObjects())
            {
                gameObject.Render(_renderer);
            }

            _player.Render(_renderer);
        }

        private void AddBomb(int x, int y)
        {
            var translated = _renderer.TranslateFromScreenToWorldCoordinates(x, y);
            SpriteSheet spriteSheet = new(_renderer, "BombExploding.png", 1, 13, 32, 64, (16, 48));
            spriteSheet.Animations["Explode"] = new SpriteSheet.Animation()
            {
                StartFrame = (0, 0),
                EndFrame = (0, 12),
                DurationMs = 2000,
                Loop = false
            };
            spriteSheet.ActivateAnimation("Explode");
            TemporaryGameObject bomb = new(spriteSheet, 2.1, (translated.X, translated.Y));
            _gameObjects.Add(bomb.Id, bomb);
            /* 
         SpriteSheet spriteSheet2 = new(_renderer, "Assets/casaAfara.png", 1, 13, 360, 360, (16, 48));

         TemporaryGameObject bomb2=new(spriteSheet2,100, (translated.X, translated.Y));
         _gameObjects.Add(bomb2.Id, bomb2);
         */

        }
    }
}