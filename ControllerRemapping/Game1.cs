using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ControllerRemapping
{
    internal enum RemappingState
    {
        notRemapping,
        waitingForButtonToRemap,
        waitingForButtonToForwardTo,
    };

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Sprite mySprite;

        private RenderTarget2D myRenderTarget;

        private Button myButton;

        private Texture2D mySquare;
        private Texture2D myCircle;

        private RemappingState myState = RemappingState.notRemapping;

        private SpriteFont myFont;

        private ButtonKey myWaitingForButtonToRemap;
        private ButtonKey myWaitingForButtonToForwardTo;

        private Sprite arrow;

        private bool showController = false;

        private Texture2D[] knapparna = new Texture2D[24];

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            myRenderTarget = new RenderTarget2D(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            mySquare = new Texture2D(GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.White;
            mySquare.SetData(colorData);

            myCircle = CreateCircleTex(100);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mySprite = new Sprite(Content.Load<Texture2D>("XboxOne_Diagram"), new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2));
            mySprite.AccessScale = ((float)_graphics.PreferredBackBufferHeight / mySprite.AccessTex.Height) * 0.7f;
            myFont = Content.Load<SpriteFont>("font");
            arrow = new Sprite(Content.Load<Texture2D>("Directional_Arrow_Straight"), new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2));
            Vector2 size = new Vector2(130, 50);
            myButton = new Button(new Rectangle(_graphics.PreferredBackBufferWidth - (int)size.X - 10, 10, (int)size.X, (int)size.Y), mySquare, "New remap");
            // TODO: use this.Content to load your game content here

            for (int i = 0; i < knapparna.Length; i++)
            {
                string temp = "XboxOneButtons/XboxOne_" + ((Buttons)MathF.Pow(2, i)).ToString();
                try
                {
                    knapparna[i] = Content.Load<Texture2D>("XboxOneButtons/XboxOne_" + ((Buttons)MathF.Pow(2, i)).ToString());
                }
                catch (Exception)
                {
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Input.GetState(Window, myRenderTarget);
            // TODO: Add your update logic here
            if (myButton.Clicked())
            {
                myState = RemappingState.waitingForButtonToRemap;
                myWaitingForButtonToRemap = null;
                myWaitingForButtonToForwardTo = null;
            }
            if (myState == RemappingState.waitingForButtonToRemap || myState == RemappingState.waitingForButtonToForwardTo)
            {
                ButtonKey buttonKey = Input.GetCurrentButtonKey();
                if (buttonKey != null)
                {
                    if (myState == RemappingState.waitingForButtonToRemap)
                    {
                        myWaitingForButtonToRemap = buttonKey;
                        myState = RemappingState.waitingForButtonToForwardTo;
                    }
                    else if (myState == RemappingState.waitingForButtonToForwardTo)
                    {
                        myWaitingForButtonToForwardTo = buttonKey;
                        Input.AddRemap(myWaitingForButtonToRemap, myWaitingForButtonToForwardTo);
                        myState = RemappingState.notRemapping;
                    }
                }
            }
            if (Input.GetButtonDown(Keys.PrintScreen))
            {
                showController = !showController;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            //mySprite.Draw(_spriteBatch);
            myButton.Draw(_spriteBatch, myFont, Color.Green);
            ButtonKey[] buttonKeys = Input.GetCurrentButtonKeysWithRedirect();
            if (buttonKeys != null && buttonKeys.Length > 0)
            {
                string temp = string.Empty;
                for (int i = 0; i < buttonKeys.Length; i++)
                {
                    temp += buttonKeys[i].ToString();
                    temp += "\n";
                }
                _spriteBatch.DrawString(myFont, temp, new Vector2(), Color.White);
            }

            if (myWaitingForButtonToForwardTo != null)
            {
                string temp = "myWaitingForButtonToForwardTo: " + myWaitingForButtonToForwardTo.ToString();
                int index = myWaitingForButtonToForwardTo.ButtonOrKey == ButtonOrKey.Button ? GetButtonIndex((int)myWaitingForButtonToForwardTo.AccessButton) : 10;
                Vector2 texPos = new Vector2();
                if (myWaitingForButtonToForwardTo.ButtonOrKey == ButtonOrKey.Button && knapparna[index] != null)
                {
                    temp = "myWaitingForButtonToForwardTo: ";
                    Vector2 texOffset = new Vector2(knapparna[index].Width, knapparna[index].Height);
                    texPos = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2) - new Vector2(0, texOffset.Y) / 2 + new Vector2(280, 0);
                    _spriteBatch.Draw(knapparna[index], texPos, Color.White);
                }

                Vector2 offset = myFont.MeasureString(temp);
                Vector2 pos = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2) - new Vector2(0, offset.Y) / 2 + new Vector2(50, 0);
                _spriteBatch.DrawString(myFont, temp, pos, Color.White);
            }
            if (myWaitingForButtonToRemap != null)
            {
                string temp = "myWaitingForButtonToRemap: " + myWaitingForButtonToRemap.ToString();
                int index = myWaitingForButtonToRemap.ButtonOrKey == ButtonOrKey.Button ? GetButtonIndex((int)myWaitingForButtonToRemap.AccessButton) : 10;
                Vector2 texPos = new Vector2();
                if (myWaitingForButtonToRemap.ButtonOrKey == ButtonOrKey.Button && knapparna[index] != null)
                {
                    temp = "myWaitingForButtonToRemap: ";
                    Vector2 texOffset = new Vector2(knapparna[index].Width, knapparna[index].Height);
                    texPos = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2) - new Vector2(texOffset.X * 2, texOffset.Y) / 2 - new Vector2(50, 0);
                    _spriteBatch.Draw(knapparna[index], texPos, Color.White);
                }
                Vector2 offset = myFont.MeasureString(temp);
                Vector2 pos = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2) - new Vector2(offset.X * 2, offset.Y) / 2 - new Vector2(40, 0);

                _spriteBatch.DrawString(myFont, temp, pos - new Vector2(texPos.X - pos.X, 0), Color.White);
            }
            if (myState == RemappingState.waitingForButtonToRemap)
            {
                string temp = "?";
                Vector2 offset = myFont.MeasureString(temp);
                Vector2 pos = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2) - offset / 2 - new Vector2(100, 0);
                _spriteBatch.DrawString(myFont, temp, pos, Color.White);
            }
            if (myWaitingForButtonToRemap != null)
            {
                arrow.Draw(_spriteBatch);
            }

            if (showController)
            {
                mySprite.Draw(_spriteBatch);
            }
            // TODO: Add your drawing code here
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private int GetButtonIndex(int input)
        {
            float temp = (MathF.Log10(2f) / MathF.Log10(input));
            temp = 1f / temp;
            return (int)MathF.Round(1f / (MathF.Log10(2f) / MathF.Log10(input)));
        }

        /// <summary>
        /// Skapar en cirkel textur.
        /// </summary>
        private Texture2D CreateCircleTex(int aRadius)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, aRadius, aRadius);
            Color[] colorData = new Color[aRadius * aRadius];

            float diam = aRadius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < aRadius; x++)
            {
                for (int y = 0; y < aRadius; y++)
                {
                    int index = x * aRadius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}