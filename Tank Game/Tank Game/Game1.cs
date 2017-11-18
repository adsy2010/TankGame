using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Tank_Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GameComponentCollection spriteOrder;
        SpriteBatch spriteBatch;
        SpriteFont ArialFont;
        int ViewPortHeight;
        int ViewPortWidth;

        Color BG = Color.Beige;

        Texture2D[] Ammo = new Texture2D[3];
        Texture2D[] Tanks = new Texture2D[3]; //change for more tanks
        Texture2D[] Button = new Texture2D[3]; //change for more buttons
        Texture2D SplashScreen;
        Texture2D Hole;

        Vector2[] StartPos = new Vector2[5];
        float[] Angles = new float[5];

        public enum AmmoType { shell_3mm, shell_8mm, shell_12mm }
        public enum TankType { Medium, Heavy, Destroyer }
        TankType SelectedTank;
        string SelectedTankName;
        bool confirmed = false;
        Tank Player;
        Tank Computer;

        public enum ScreenPositions { Centre, Top, Bottom, Left, Right }
        public enum GameState { Splash, MainMenu, SelectionMenu, TankConfirm, InGame, GameOver, Pause }
        GameState currentState = GameState.Splash;

        GamePadState oldstate;
        KeyboardState oldkeys;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            SetupFullScreen();
            UpdateViewPortSize();
            Content.RootDirectory = "Content";
            Player = new Tank(graphics.GraphicsDevice);
            Computer = new Tank(graphics.GraphicsDevice);
            CreateStartPositions();
        }

        void UpdateViewPortSize()
        {
            ViewPortHeight = graphics.GraphicsDevice.Viewport.Height;
            ViewPortWidth = graphics.GraphicsDevice.Viewport.Width;
        }
        void SetupFullScreen()
        {

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        void CreateStartPositions()
        {
            StartPos[(int)ScreenPositions.Left]     = new Vector2(40, ViewPortHeight / 2.0f);
            StartPos[(int)ScreenPositions.Right]    = new Vector2(ViewPortWidth - 40, ViewPortHeight / 2.0f);
            StartPos[(int)ScreenPositions.Top]      = new Vector2(ViewPortWidth / 2.0f, 40);
            StartPos[(int)ScreenPositions.Bottom]   = new Vector2(ViewPortWidth / 2.0f, ViewPortHeight - 140);
            StartPos[(int)ScreenPositions.Centre]   = new Vector2(ViewPortWidth / 2.0f, ViewPortHeight / 2.0f);

            Angles[(int)ScreenPositions.Left]   = 0.0f;
            Angles[(int)ScreenPositions.Bottom] = 270.0f;
            Angles[(int)ScreenPositions.Centre] = 270.0f;
            Angles[(int)ScreenPositions.Top]    = 90.0f;
            Angles[(int)ScreenPositions.Right]  = 180.0f;

       }
        void TankData(Tank CurrentTank, string name, int topSpeed, int reverseSpeed, float engineBraking, 
            float rotationSpeed, float weight, float horsePower, int hitPoints, AmmoType ammotype)
        {
            //Sets all tank data
            CurrentTank.name = name;
            CurrentTank.SetTopSpeed(topSpeed);
            CurrentTank.SetReverseSpeed(reverseSpeed);
            CurrentTank.SetEngineBraking(engineBraking);
            CurrentTank.SetRotationSpeed(rotationSpeed);
            CurrentTank.SetWeight(weight);
            CurrentTank.CalculateAccelleration(horsePower);
            CurrentTank.hp = hitPoints;
            CurrentTank.ammo.ammo = (int)ammotype;
        }
        void AmmoData(Tank CurrentTank, string name, int amount, float range, float speed, float reload, float power)
        {
            CurrentTank.ammo.ammoStore = amount;
            CurrentTank.ammo.ammoRange = range;
            CurrentTank.ammo.ammoSpeed = speed;

            CurrentTank.ammo.fireDelay = reload;
            CurrentTank.ammo.power = power;
        }

        void SetupSelectedTank(TankType t, Tank p, ScreenPositions Start)
        {                        
            //----Tank----
           //name, topspeed, reversespeed, enginebraking, rotationspeed, 
           //weight, horsepower, hitpoints
            
            //----Ammo----
            //name, storage, range, speed, reload
            
            

            switch (t)
            {
                case TankType.Destroyer: 
                    {

                        TankData(p, "Destroyer", 37, 12, 0.03f, 0.04f, 1500.0f, 320.0f, 200, AmmoType.shell_12mm);
                        AmmoData(p, "12mm Shell", 88, 200.0f, 4.0f, 2.0f, 1.5f);
                    }
                break;

                case TankType.Heavy:
                    {
                        TankData(p, "Heavy Tank", 20, 8, 0.04f, 0.01f, 3300.0f, 400.0f, 450, AmmoType.shell_8mm);
                        AmmoData(p, "8mm Shell", 68, 250.0f, 5.3f, 1.5f, 1.0f);
                    }
                break;

                case TankType.Medium:
                    {
                        TankData(p, "Medium Tank", 30, 10, 0.05f, 0.025f, 2500.0f, 350.0f, 320, AmmoType.shell_3mm);
                        AmmoData(p, "3mm Shell", 150, 150.0f, 6.9f, 0.6f, 0.4f);
                    }
                break;
            }
            currentState = GameState.TankConfirm;

            p.ResetPosition(StartPos[(int)Start]);
            p.SetRotationAngle(Angles[(int)Start]);
            p.ResetSpeed();
            
        }

        //Drawing items on screen
        void DrawString(string str, Vector2 pos)
        {
            Vector2 FontOrigin = ArialFont.MeasureString(str) / 2;
            spriteBatch.DrawString(ArialFont, str, pos, Color.LightGreen, 
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.0f);
        }
        void DrawString(string str, Vector2 pos, float size)
        {
            Vector2 FontOrigin = ArialFont.MeasureString(str) / 2;
            spriteBatch.DrawString(ArialFont, str, pos, Color.LightGreen,
    0, FontOrigin, size, SpriteEffects.None, 0.0f);
        }
        void DrawString(string str, Vector2 pos, float size, Color color)
        {
            Vector2 FontOrigin = ArialFont.MeasureString(str) / 2;
            spriteBatch.DrawString(ArialFont, str, pos, color,
    0, FontOrigin, size, SpriteEffects.None, 0.0f);
        }
        void DrawString(string str, Vector2 pos, ScreenPositions origin)
        {
            Vector2 FontOrigin;
            FontOrigin = ArialFont.MeasureString(str) / 2;
            //for text alignment
            switch (origin)
            {
                case ScreenPositions.Right:
                    FontOrigin = ArialFont.MeasureString(str);
                    //FontOrigin.X = FontOrigin.X;
                    break;                
                case ScreenPositions.Left:
                    FontOrigin = ArialFont.MeasureString(str);
                    FontOrigin.X = 0;
                    break;
                case ScreenPositions.Centre:
                    FontOrigin = ArialFont.MeasureString(str) / 2;
                    break;
            }
            

            spriteBatch.DrawString(ArialFont, str, pos, Color.LightGreen, 
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.0f);
        }
        void DrawString(string str, Vector2 pos, ScreenPositions origin, Color color)
        {
            Vector2 FontOrigin;
            FontOrigin = ArialFont.MeasureString(str) / 2;
            //for text alignment
            switch (origin)
            {
                case ScreenPositions.Right:
                    FontOrigin = ArialFont.MeasureString(str);
                    //FontOrigin.X = FontOrigin.X;
                    break;
                case ScreenPositions.Left:
                    FontOrigin = ArialFont.MeasureString(str);
                    FontOrigin.X = 0;
                    break;
                case ScreenPositions.Centre:
                    FontOrigin = ArialFont.MeasureString(str) / 2;
                    break;
            }


            spriteBatch.DrawString(ArialFont, str, pos, color, 
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.0f);
        }

        void DrawObject(Texture2D sprite, Vector2 pos, float size)
        {
            Vector2 spritePos = new Vector2((float)sprite.Width / 2.0f, (float)sprite.Height / 2.0f);
            spriteBatch.Draw(sprite, pos, null, Color.White, 0.0f, spritePos, size, SpriteEffects.None, 1.0f);
        }
        void DrawObject(Texture2D sprite, Rectangle rect, Color col)
        {

            spriteBatch.Draw(sprite, new Vector2(0, ViewPortHeight-100), rect, col, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            //spriteBatch.Draw(sprite, rect, col);
        }
        
        void StartDraws()
        {
            Vector2 Centre = new Vector2(ViewPortWidth/2, ViewPortHeight/2);
            Vector2 TopCentre = new Vector2(ViewPortWidth / 2, 20);
            Vector2 BottomCentre = new Vector2(ViewPortWidth / 2, ViewPortHeight-20);
            Vector2 LeftCentre = new Vector2(20, ViewPortHeight / 2);
            Vector2 RightCentre = new Vector2(ViewPortWidth - 20, ViewPortHeight / 2);
            switch (currentState)
            {
                case GameState.Splash:
                    //DrawString("Splash Screen", LeftCentre, ScreenPositions.Left);
                    DrawObject(SplashScreen, Centre, 1.0f);
                    //DrawObject(Hole, Centre, 5.0f);
                    break;
                case GameState.MainMenu:
                    BG = Color.BlueViolet;
                    DrawString("Press Start to go to tank selection", Centre);
                    break;
                case GameState.SelectionMenu:
                    BG = Color.Gainsboro;
                    TankType[] tankList = { TankType.Medium, TankType.Heavy, TankType.Destroyer };
                    Centre.X = Centre.X - 150;
                    int count = 0;
                    foreach (TankType t in tankList)
                    {
                        DrawObject(Tanks[(int)t], Centre, 2.5f);
                        DrawObject(Button[count], new Vector2(Centre.X, Centre.Y + 150), 1.0f);

                        Centre.X = Centre.X + 150;
                        count++;
                    }
                    break;
                case GameState.TankConfirm:
                    DrawObject(Tanks[(int)SelectedTank], Centre, 2.5f);
                    DrawString(Player.name, new Vector2(Centre.X, Centre.Y - 150), 3.0f, Color.Black);
                    DrawObject(Button[0], new Vector2(Centre.X - 50, Centre.Y + 150), 1.0f);
                    DrawObject(Button[1], new Vector2(Centre.X + 50, Centre.Y + 150), 1.0f);
                    break;
                case GameState.InGame:
                    BG = Color.ForestGreen;
                    DrawBanner();
                    foreach (Damage d in Player.HoleList) d.Draw(spriteBatch);
                    foreach (Damage d in Computer.HoleList) d.Draw(spriteBatch);
                    Player.Draw(spriteBatch);
                    Computer.Draw(spriteBatch);
                    foreach (Shell s in Player.ShellShot) s.Draw(spriteBatch);
                    foreach (Shell s in Computer.ShellShot) s.Draw(spriteBatch);
                    
                    for (int i = 0; i < Player.ShellShot.Count; i++)
                        if (Player.ShellShot[i].OutOfRange())
                        {
                            Player.HoleList.Add(new Damage(Hole, Player.ShellShot[i].Position, Player.ShellShot[i].rotationAngle, Player.power));
                            Player.ShellShot.RemoveAt(i);
                            
                        }
                    for (int i = 0; i < Computer.ShellShot.Count; i++)
                        if (Computer.ShellShot[i].OutOfRange())
                        {
                            Computer.HoleList.Add(new Damage(Hole, Computer.ShellShot[i].Position, Computer.ShellShot[i].rotationAngle, Computer.power));
                            Computer.ShellShot.RemoveAt(i);

                        }
                //DrawObject(Tanks[(int)SelectedTank], Centre, 2.5f);
                    //spriteBatch.Draw(Button[count], new Vector2(Centre.X, Centre.Y + 150), null, Color.White, 0.0f, spritePos, 1.0f, SpriteEffects.None, 0.0f);
                    break;

            }
            
        }

        private void DrawBanner()
        {
            Texture2D bannerRectangle = new Texture2D(GraphicsDevice, 1, 1);
            bannerRectangle.SetData(new Color[] { Color.Gold });
            DrawObject(bannerRectangle, new Rectangle(0,ViewPortHeight-100,ViewPortWidth, 100), Color.Gold);

            float distancetotarget = Vector2.Distance(Computer.position, Player.position);

            float speedconverter = (float)Math.Round(Player.speed, 1) * 10f;
            DrawString(speedconverter.ToString() + " kph", new Vector2(0, ViewPortHeight - 20), ScreenPositions.Left, Color.Black);
            DrawString("x: " + Math.Round(Player.position.X).ToString()
                + "y: " + Math.Round(Player.position.Y).ToString(), new Vector2(150, ViewPortHeight - 20), ScreenPositions.Left, Color.Black);
            DrawString("HP: " + Player.hp.ToString(), new Vector2(350, ViewPortHeight - 20), ScreenPositions.Left, Color.Black);
            DrawString(Player.name, new Vector2(ViewPortWidth - 20, ViewPortHeight - 20), ScreenPositions.Right, Color.Black);
            DrawString(Computer.rotationDifferenceDeg.ToString(), new Vector2(500, ViewPortHeight - 20), ScreenPositions.Left, Color.Black);
            
                    }

        void ComputerInput(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.InGame:
                    Vector2 dir;
                    dir = Computer.position - Player.position;
                    dir.Normalize();
                    float angle;
                    angle = (float)Math.Atan2(-dir.X, dir.Y);
                    if (angle < MathHelper.ToRadians(20.0f)) { Computer.Left(); }
                    /*
                    if (Vector2.Distance(Player.position, Computer.position) > 120.0f) Computer.Forward();
                    if (Vector2.Distance(Player.position, Computer.position) < 120.0f)
                    {
                        if (Vector2.Distance(Player.position, Computer.position) < 65.0f)
                        {
                            Computer.Reverse();
                            Computer.Fire();
                        }
                        else
                        {
                            Computer.Right();
                        }
                    }*/
                    break;
            }
        }

        void UpdateInput(GameTime gameTime)
        {
            //handle inputs
            // Allows the game to exit

            var newState = GamePad.GetState(PlayerIndex.One);
            var keyState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch(currentState)
            {
                case GameState.Splash:
                    //allow skip with a or start || or space
#region gamepad
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.A)) ||
                        (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.Start)))
                    {
                        currentState = GameState.MainMenu;
                    }
#endregion
#region keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && !oldkeys.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.MainMenu;
                    }
#endregion
                    break;
                case GameState.MainMenu:
                    //a or start = select || or space
#region gamepad
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.A)) ||
                        (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.Start)))
                    {
                        currentState = GameState.SelectionMenu;
                    }
#endregion
#region keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && !oldkeys.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.SelectionMenu;
                    }
#endregion
                    break;
                case GameState.SelectionMenu:
                        //Select from a list
                        //a or start = select
                    //left or right = selection
#region gamepad
                    if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.A))
                    {
                        SelectedTank = TankType.Medium;
                        SetupSelectedTank(SelectedTank, Player, ScreenPositions.Left);
                        SetupSelectedTank(TankType.Heavy, Computer, ScreenPositions.Right);
                    }
                    if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.B))
                    {
                        SelectedTank = TankType.Heavy;
                        SetupSelectedTank(SelectedTank, Player, ScreenPositions.Left);
                        SetupSelectedTank(TankType.Heavy, Computer, ScreenPositions.Right);
                    }
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.Y))
                    {
                        SelectedTank = TankType.Destroyer;
                        SetupSelectedTank(SelectedTank, Player, ScreenPositions.Left);
                        SetupSelectedTank(TankType.Heavy, Computer, ScreenPositions.Right);
                    }
#endregion
#region keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.A) && !oldkeys.IsKeyDown(Keys.A))
                    {
                        SelectedTank = TankType.Medium;
                        SetupSelectedTank(SelectedTank, Player, ScreenPositions.Left);
                        SetupSelectedTank(TankType.Heavy, Computer, ScreenPositions.Right);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.B) && !oldkeys.IsKeyDown(Keys.B))
                    {
                        SelectedTank = TankType.Heavy;
                        SetupSelectedTank(SelectedTank, Player, ScreenPositions.Left);
                        SetupSelectedTank(TankType.Heavy, Computer, ScreenPositions.Right);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Y) && !oldkeys.IsKeyDown(Keys.Y))
                    {
                        SelectedTank = TankType.Destroyer;
                        SetupSelectedTank(SelectedTank, Player, ScreenPositions.Left);
                        SetupSelectedTank(TankType.Heavy, Computer, ScreenPositions.Right);
                    }
#endregion
                    break;
                case GameState.TankConfirm:
#region gamepad
                    if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.A))
                    {
                        ConfirmTank(gameTime);
                    }
                    if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.B))
                        currentState = GameState.SelectionMenu;
#endregion
#region keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.A) && !oldkeys.IsKeyDown(Keys.A))
                    {
                        ConfirmTank(gameTime);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.B) && !oldkeys.IsKeyDown(Keys.B))
                        currentState = GameState.SelectionMenu;
#endregion
                    break;
                case GameState.InGame:
                    //up down left right for movement
                    //a = fire
                    //b = change attack type
                    //start = pause
#region gamepad
                    if (newState.IsButtonDown(Buttons.RightThumbstickUp))
                        Player.Forward();
                    if (newState.IsButtonDown(Buttons.RightThumbstickDown))
                        Player.Reverse();

                    if (newState.IsButtonDown(Buttons.LeftThumbstickLeft))
                        Player.Left();
                    if (newState.IsButtonDown(Buttons.LeftThumbstickRight))
                        Player.Right();
                    if (newState.IsButtonDown(Buttons.A))
                    {
                        if (gameTime.TotalGameTime.TotalSeconds > (Player.ammo.fireTime + Player.ammo.fireDelay))
                        {
                            Player.Fire();
                            Player.ammo.fireTime = gameTime.TotalGameTime.TotalSeconds;
                        }
                    }
                    if (newState.IsButtonDown(Buttons.Y))
                    {
                        currentState = GameState.SelectionMenu;
                    }
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.Start))
                        currentState = GameState.Pause;
#endregion
#region keyboard
                    if (keyState.IsKeyDown(Keys.Up))
                        Player.Forward();
                    if (keyState.IsKeyDown(Keys.Down))
                        Player.Reverse();
                    if (keyState.IsKeyDown(Keys.Left))
                        Player.Left();
                    if (keyState.IsKeyDown(Keys.Right))
                        Player.Right();
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        if (gameTime.TotalGameTime.TotalSeconds > (Player.ammo.fireTime + Player.ammo.fireDelay))
                        {
                            Player.Fire();
                            Player.ammo.fireTime = gameTime.TotalGameTime.TotalSeconds;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.Y))
                    {
                        currentState = GameState.SelectionMenu;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && !oldkeys.IsKeyDown(Keys.Space))
                        currentState = GameState.Pause;
#endregion
#region keys/gamepad
                    if (!newState.IsButtonDown(Buttons.RightThumbstickDown) &&
                        !newState.IsButtonDown(Buttons.RightThumbstickUp) &&
                        !keyState.IsKeyDown(Keys.Up) && !oldkeys.IsKeyDown(Keys.Up) &&
                        !keyState.IsKeyDown(Keys.Down) && !oldkeys.IsKeyDown(Keys.Down))
                        Player.SlowDown(Player.GetEngineBraking());
#endregion
                    break;
                case GameState.Pause:
                    //start = unpause
#region gamepad
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.Start))
                    {
                        currentState = GameState.InGame;
                    }
#endregion
#region keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && !oldkeys.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.InGame;
                    }
#endregion
                    break;
                case GameState.GameOver:
                    //start or a returns to main menu
#region gamepad
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.A)) ||
                        (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && !oldstate.IsButtonDown(Buttons.Start)))
                    {
                        currentState = GameState.MainMenu;
                    }
#endregion
#region keyboard
#endregion
                    break;
            }
            oldstate = newState;
            oldkeys = keyState;
        }

        void ConfirmTank(GameTime gameTime)
        {
            Computer.tank = Tanks[(int)TankType.Heavy];
            Computer.ammoImage = Ammo[(int)Computer.ammo.ammo];
            Computer.rectangle = new Texture2D(GraphicsDevice, 1, 1);
            Computer.rectangle.SetData(new Color[] { Color.Gold });

            Player.tank = Tanks[(int)SelectedTank];
            Player.ammoImage = Ammo[(int)Player.ammo.ammo];
            Player.rectangle = new Texture2D(GraphicsDevice, 1, 1);
            Player.rectangle.SetData(new Color[] { Color.Gold });
            confirmed = true;

            Player.ammo.fireTime = gameTime.TotalGameTime.TotalSeconds;
            currentState = GameState.InGame;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ArialFont = Content.Load<SpriteFont>("Arial");

            LoadTextures();
            // TODO: use this.Content to load your game content here
        }

        void LoadTextures()
        {
            //This is tank exteriors, add ons are seperate textures
            Tanks[(int)TankType.Medium]     = Content.Load<Texture2D>("Medium Tank");
            Tanks[(int)TankType.Heavy]      = Content.Load<Texture2D>("Heavy Tank");
            Tanks[(int)TankType.Destroyer]  = Content.Load<Texture2D>("Tank Hunter");

            //A, B and Y colour coded like xbox
            Button[0] = Content.Load<Texture2D>("A");
            Button[1] = Content.Load<Texture2D>("B");
            Button[2] = Content.Load<Texture2D>("Y");

            //ammo shells
            /***********************
             * 3mm Regular rounds  *
             * 8mm Regular rounds  *
             * 12mm Regular rounds *
             ***********************/
            Ammo[(int)AmmoType.shell_3mm]   = Content.Load<Texture2D>("3mm");
            Ammo[(int)AmmoType.shell_8mm]   = Content.Load<Texture2D>("8mm");
            Ammo[(int)AmmoType.shell_12mm]  = Content.Load<Texture2D>("12mm");

            Hole = Content.Load<Texture2D>("hole");
            SplashScreen = Content.Load<Texture2D>("Splash");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            //ComputerInput(gameTime);
            UpdateInput(gameTime);
            
            if (currentState == GameState.InGame && confirmed == true)
            {
                Computer.Update(gameTime);
                if (Computer.OutOfRange(Player.position, Computer.position, Computer.ammo.ammoRange + 50.0f))
                {
                    Computer.Forward();
                }
                else
                {
                    Computer.SlowDown(Computer.GetEngineBraking());
                }

                if (!Computer.OutOfRange(Player.position, Computer.position, Computer.ammo.ammoRange))
                {
                    if (gameTime.TotalGameTime.TotalSeconds > (Computer.ammo.fireTime + Computer.ammo.fireDelay))
                    {
                        Computer.Fire();
                        Computer.ammo.fireTime = gameTime.TotalGameTime.TotalSeconds;
                    }
                }
                Computer.Track((int)Player.position.X, (int)Player.position.Y);

                Player.Update(gameTime);
                foreach (Shell s in Player.ShellShot)
                {
                    s.Update(gameTime);
                }
                foreach (Shell s in Computer.ShellShot)
                {
                    s.Update(gameTime);
                }    
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BG);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            StartDraws();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        TcpListener server;
        TcpClient c;
        bool serverRunning = false;
        void SetupServer()
        {
            server = new TcpListener(IPAddress.Any, 1234);
        }
        void StartServer()
        {
            server.Start();
            serverRunning = true;
            Thread t = new Thread(new ParameterizedThreadStart(ClientHandle));
            t.Start(serverRunning);
        }
        void StopServer()
        {
            server.Stop();
            serverRunning = false;
        }
        void ClientHandle(object sr)
        {
            while((bool)sr)
            {
                c = server.AcceptTcpClient();
                NetworkStream stream = c.GetStream();
                //send start signal to both.
                stream.WriteByte(0x01);
            }
        }

        public enum Handler : byte
        {
            Disconnect, Connected, TankType, 
            Forward, Backward, Left, Right, Fire
        }

        void ReadHandle(Handler a)
        {
            switch (a)
            {
                case Handler.Disconnect:
                    break;
                case Handler.Connected:
                    break;
                case Handler.TankType:
                    break;
                case Handler.Forward:
                    break;
                case Handler.Backward:
                    break;
                case Handler.Left:
                    break;
                case Handler.Right:
                    break;
                case Handler.Fire:
                    break;
                
            }
        }

    }
}
