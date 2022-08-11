using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

//using System.Windows.Forms;

namespace ControllerRemapping
{
    internal static class Input
    {
        private static KeyboardState myCurrentKeyState;

        private static KeyboardState myPreviousKeyState;

        private static GamePadState myCurrentGamePadState;
        private static GamePadState myPreviousGamePadState;

        private static MouseState myCurrentMouseState;
        private static MouseState myPreviousMouseState;

        private static Vector2 myDirectional;
        public static Vector2 AccessDirectional { get => myDirectional; }

        public static Vector2 AccessCurrentNormalizedDirectional { private set; get; }
        public static Vector2 AccessPreviousNormalizedDirectional { private set; get; }

        public static Vector2 mousePosWithOffset { private set; get; }

        public static int clampedScrollWheelValue { private set; get; }

        public static int scrollWheelValueChange { private set; get; }

        private static int myMinScrollWheel;
        private static int myMaxScrollWheel;

        private static int myCurrentscrollWheelValue;
        private static int myPreviousscrollWheelValue;

        private static List<ButtonKey> remappedButtons = new List<ButtonKey>();

        private static List<ButtonKey> newMapsForButtons = new List<ButtonKey>();

        //private static Camera myCamera;
        public static Vector2 myWorldMousePos { private set; get; }

        //public static void setCameraStuff(Camera camera)
        //{
        //    myMaxScrollWheel = (int)Math.Round((camera.AccessMaxZoom - 1) / 0.001f);
        //    myMinScrollWheel = (int)Math.Round((camera.AccessMinZoom - 1) / 0.001f);
        //    myCamera = camera;
        //}

        /// <summary>
        /// Uppdaterar input grejerna.
        /// </summary>
        public static KeyboardState GetState(GameWindow Window, RenderTarget2D renderTarget)
        {
            myPreviousKeyState = myCurrentKeyState;
            myCurrentKeyState = Keyboard.GetState();

            myPreviousGamePadState = myCurrentGamePadState;
            myCurrentGamePadState = GamePad.GetState(PlayerIndex.One);

            myPreviousMouseState = myCurrentMouseState;
            myCurrentMouseState = Mouse.GetState();
            //Vector2 input = new Vector2();
            myDirectional = new Vector2();
            myDirectional += new Vector2(0, GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed ? 1 : 0);
            myDirectional += new Vector2(0, GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed ? -1 : 0);
            myDirectional += new Vector2(GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed ? -1 : 0, 0);
            myDirectional += new Vector2(GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed ? 1 : 0, 0);

            myDirectional += new Vector2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X, -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);

            myDirectional += new Vector2(0, GetButton(Keys.Down) || GetButton(Keys.S) ? 1 : 0);
            myDirectional += new Vector2(0, GetButton(Keys.Up) || GetButton(Keys.W) ? -1 : 0);
            myDirectional += new Vector2(GetButton(Keys.Left) || GetButton(Keys.A) ? -1 : 0, 0);
            myDirectional += new Vector2(GetButton(Keys.Right) || GetButton(Keys.D) ? 1 : 0, 0);
            myDirectional = new Vector2(Math.Clamp(AccessDirectional.X, -1, 1), Math.Clamp(AccessDirectional.Y, -1, 1));

            AccessPreviousNormalizedDirectional = AccessCurrentNormalizedDirectional;
            AccessCurrentNormalizedDirectional = AdvancedMath.ClampMagnitude(AccessDirectional, 1);
            mousePosWithOffset = MousePos(Window, renderTarget);

            myPreviousscrollWheelValue = myCurrentscrollWheelValue;
            myCurrentscrollWheelValue = Mouse.GetState().ScrollWheelValue;

            clampedScrollWheelValue += myCurrentscrollWheelValue - myPreviousscrollWheelValue;

            clampedScrollWheelValue = MathHelper.Clamp(clampedScrollWheelValue, myMinScrollWheel, myMaxScrollWheel);

            scrollWheelValueChange = myCurrentscrollWheelValue - myPreviousscrollWheelValue;

            myMousePos = new Vector2(myCurrentMouseState.Position.X, myCurrentMouseState.Position.Y);

            string temp = myCurrentGamePadState.ToString();
            temp = myCurrentKeyState.ToString();
            for (int i = 0; i < 10; i++)
            {
                GamePadCapabilities gamePadCapabilities = GamePad.GetCapabilities(i);
                temp = gamePadCapabilities.DisplayName;
            }

            //myWorldMousePos = myCamera.ScreenToWorldSpace(MousePos());
            return myCurrentKeyState;
        }

        public static ButtonKey GetCurrentButtonKey()
        {
            for (int i = 0; i < 31; i++)
            {
                Buttons temp = (Buttons)MathF.Pow(2, i);
                if (temp == Buttons.RightStick)
                {
                }
                if (myPreviousGamePadState.IsButtonDown((Buttons)MathF.Pow(2, i)) && !myCurrentGamePadState.IsButtonDown((Buttons)MathF.Pow(2, i)))
                {
                    return new ButtonKey(temp);
                }
            }
            if (myPreviousKeyState.GetPressedKeyCount() == 1 && myCurrentKeyState.GetPressedKeyCount() == 0)
            {
                Keys[] temp = myPreviousKeyState.GetPressedKeys();
                return new ButtonKey(temp[0]);
            }
            return null;
        }

        public static ButtonKey[] GetCurrentButtonKeys()
        {
            List<ButtonKey> buttonKeys = new List<ButtonKey>();
            for (int i = 0; i < 31; i++)
            {
                if (myCurrentGamePadState.IsButtonDown((Buttons)MathF.Pow(2, i)))
                {
                    Buttons temp = (Buttons)MathF.Pow(2, i);
                    buttonKeys.Add(new ButtonKey(temp));
                    //Buttons.LeftThumbstickRight
                }
            }
            if (myCurrentKeyState.GetPressedKeyCount() > 0)
            {
                Keys[] temp = myPreviousKeyState.GetPressedKeys();
                for (int i = 0; i < temp.Length; i++)
                {
                    buttonKeys.Add(new ButtonKey(temp[i]));
                }
            }
            if (buttonKeys.Count > 0)
            {
                return buttonKeys.ToArray();
            }
            return null;
        }

        public static ButtonKey[] GetCurrentButtonKeysWithRedirect()
        {
            ButtonKey[] buttonKeys = GetCurrentButtonKeys();
            if (buttonKeys != null)
            {
                for (int i = 0; i < buttonKeys.Length; i++)
                {
                    buttonKeys[i] = GetRemap(buttonKeys[i]);
                }
                return buttonKeys;
            }
            return null;
        }

        private static ButtonKey GetRemap(ButtonKey buttonKey)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == buttonKey.ButtonOrKey && o.AccessButton == buttonKey.AccessButton && o.AccessKey == buttonKey.AccessKey);
            if (index >= 0)
            {
                return newMapsForButtons[index];
            }
            else
            {
                return buttonKey;
            }
        }

        public static void AddRemap(ButtonKey buttonToRemap, ButtonKey buttonToForwardTo)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == buttonToRemap.ButtonOrKey && o.AccessButton == buttonToRemap.AccessButton && o.AccessKey == buttonToRemap.AccessKey);
            if (index >= 0)
            {
                remappedButtons.RemoveAt(index);
                newMapsForButtons.RemoveAt(index);
            }
            remappedButtons.Add(buttonToRemap);
            newMapsForButtons.Add(buttonToForwardTo);
        }

        /// <summary>
        /// Kollar om knappen är nedtryckt
        /// </summary>
        public static bool GetButton(Keys key)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == ButtonOrKey.Key && o.AccessKey == key);
            if (index >= 0)
            {
                if (newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button)
                {
                    return ActualGetButton(newMapsForButtons[index].AccessButton);
                }
                else
                {
                    return ActualGetButton(newMapsForButtons[index].AccessKey);
                }
                //return ActualGetButton(newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button ? newMapsForButtons[index].AccessButton : newMapsForButtons[index].AccessKey);
            }
            else
            {
                return ActualGetButton(key);
            }
        }

        private static bool ActualGetButton(Keys key)
        {
            return myCurrentKeyState.IsKeyDown(key);
        }

        /// <summary>
        /// Kollar om knappen trycktes ner denna framen.
        /// </summary>
        public static bool GetButtonDown(Keys key)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == ButtonOrKey.Key && o.AccessKey == key);
            if (index >= 0)
            {
                if (newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button)
                {
                    return ActualGetButtonDown(newMapsForButtons[index].AccessButton);
                }
                else
                {
                    return ActualGetButtonDown(newMapsForButtons[index].AccessKey);
                }
                //return ActualGetButton(newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button ? newMapsForButtons[index].AccessButton : newMapsForButtons[index].AccessKey);
            }
            else
            {
                return ActualGetButtonDown(key);
            }
        }

        private static bool ActualGetButtonDown(Keys key)
        {
            return myCurrentKeyState.IsKeyDown(key) && !myPreviousKeyState.IsKeyDown(key);
        }

        /// <summary>
        /// Kollar om knappen lyftes upp denna framen.
        /// </summary>
        public static bool GetButtonUp(Keys key)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == ButtonOrKey.Key && o.AccessKey == key);
            if (index >= 0)
            {
                if (newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button)
                {
                    return ActualGetButtonUp(newMapsForButtons[index].AccessButton);
                }
                else
                {
                    return ActualGetButtonUp(newMapsForButtons[index].AccessKey);
                }
                //return ActualGetButton(newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button ? newMapsForButtons[index].AccessButton : newMapsForButtons[index].AccessKey);
            }
            else
            {
                return ActualGetButtonUp(key);
            }
        }

        private static bool ActualGetButtonUp(Keys key)
        {
            return !myCurrentKeyState.IsKeyDown(key) && myPreviousKeyState.IsKeyDown(key);
        }

        /// <summary>
        /// Kollar om knappen är nedtryckt.
        /// </summary>
        public static bool GetButton(Buttons key)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == ButtonOrKey.Button && o.AccessButton == key);
            if (index >= 0)
            {
                if (newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button)
                {
                    return ActualGetButton(newMapsForButtons[index].AccessButton);
                }
                else
                {
                    return ActualGetButton(newMapsForButtons[index].AccessKey);
                }
                //return ActualGetButton(newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button ? newMapsForButtons[index].AccessButton : newMapsForButtons[index].AccessKey);
            }
            else
            {
                return ActualGetButton(key);
            }
        }

        private static bool ActualGetButton(Buttons key)
        {
            return myCurrentGamePadState.IsButtonDown(key);
        }

        /// <summary>
        /// Kollar om knappen trycktes ner denna framen.
        /// </summary>
        public static bool GetButtonDown(Buttons key)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == ButtonOrKey.Button && o.AccessButton == key);
            if (index >= 0)
            {
                if (newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button)
                {
                    return ActualGetButtonDown(newMapsForButtons[index].AccessButton);
                }
                else
                {
                    return ActualGetButtonDown(newMapsForButtons[index].AccessKey);
                }
                //return ActualGetButton(newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button ? newMapsForButtons[index].AccessButton : newMapsForButtons[index].AccessKey);
            }
            else
            {
                return ActualGetButtonDown(key);
            }
        }

        private static bool ActualGetButtonDown(Buttons key)
        {
            return myCurrentGamePadState.IsButtonDown(key) && !myPreviousGamePadState.IsButtonDown(key);
        }

        /// <summary>
        /// Kollar om knappen lyftes denna framen.
        /// </summary>
        public static bool GetButtonUp(Buttons key)
        {
            int index = remappedButtons.FindIndex(o => o.ButtonOrKey == ButtonOrKey.Button && o.AccessButton == key);
            if (index >= 0)
            {
                if (newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button)
                {
                    return ActualGetButtonUp(newMapsForButtons[index].AccessButton);
                }
                else
                {
                    return ActualGetButtonUp(newMapsForButtons[index].AccessKey);
                }
                //return ActualGetButton(newMapsForButtons[index].ButtonOrKey == ButtonOrKey.Button ? newMapsForButtons[index].AccessButton : newMapsForButtons[index].AccessKey);
            }
            else
            {
                return ActualGetButtonUp(key);
            }
        }

        private static bool ActualGetButtonUp(Buttons key)
        {
            return !myCurrentGamePadState.IsButtonDown(key) && myPreviousGamePadState.IsButtonDown(key);
        }

        /// <summary>
        /// Kollar om knappen är nedtryckt.
        /// </summary>
        public static bool GetMouseButton(int key)
        {
            if (key == 0)
            {
                return myCurrentMouseState.LeftButton == ButtonState.Pressed;
            }
            if (key == 1)
            {
                return myCurrentMouseState.RightButton == ButtonState.Pressed;
            }
            if (key == 2)
            {
                return myCurrentMouseState.MiddleButton == ButtonState.Pressed;
            }
            if (key == 3)
            {
                return myCurrentMouseState.XButton1 == ButtonState.Pressed;
            }
            if (key == 4)
            {
                return myCurrentMouseState.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        /// <summary>
        /// Kollar om knappen trycks ner denna framen men inte förra.
        /// </summary>
        public static bool GetMouseButtonDown(int key)
        {
            if (key == 0)
            {
                return myCurrentMouseState.LeftButton == ButtonState.Pressed && myPreviousMouseState.LeftButton == ButtonState.Released;
            }
            if (key == 1)
            {
                return myCurrentMouseState.RightButton == ButtonState.Pressed && myPreviousMouseState.RightButton == ButtonState.Released;
            }
            if (key == 2)
            {
                return myCurrentMouseState.MiddleButton == ButtonState.Pressed && myPreviousMouseState.MiddleButton == ButtonState.Released;
            }
            if (key == 3)
            {
                return myCurrentMouseState.XButton1 == ButtonState.Pressed && myPreviousMouseState.XButton1 == ButtonState.Released;
            }
            if (key == 4)
            {
                return myCurrentMouseState.XButton2 == ButtonState.Pressed && myPreviousMouseState.XButton2 == ButtonState.Released;
            }
            return false;
        }

        /// <summary>
        /// Kollar om knappen lyftes upp denna framen.
        /// </summary>
        public static bool GetMouseButtonUp(int key)
        {
            if (key == 0)
            {
                return myCurrentMouseState.LeftButton == ButtonState.Released && myPreviousMouseState.LeftButton == ButtonState.Pressed;
            }
            if (key == 1)
            {
                return myCurrentMouseState.RightButton == ButtonState.Released && myPreviousMouseState.RightButton == ButtonState.Pressed;
            }
            if (key == 2)
            {
                return myCurrentMouseState.MiddleButton == ButtonState.Released && myPreviousMouseState.MiddleButton == ButtonState.Pressed;
            }
            if (key == 3)
            {
                return myCurrentMouseState.XButton1 == ButtonState.Released && myPreviousMouseState.XButton1 == ButtonState.Pressed;
            }
            if (key == 4)
            {
                return myCurrentMouseState.XButton2 == ButtonState.Released && myPreviousMouseState.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        /// <summary>
        /// Returnerar muspositionen.
        /// </summary>
        public static Vector2 MousePos()
        {
            //return new Vector2(myCurrentMouseState.Position.X, myCurrentMouseState.Position.Y);
            return myMousePos;
        }

        public static Vector2 myMousePos { private set; get; }

        /// <summary>
        /// Returnerar muspositionen med en offset beroende rendertargeten.
        /// </summary>
        public static Vector2 MousePos(GameWindow Window, RenderTarget2D renderTarget)
        {
            Vector2 offset = new Vector2((float)renderTarget.Width / (float)Window.ClientBounds.Width, (float)renderTarget.Height / (float)Window.ClientBounds.Height);
            return new Vector2(myCurrentMouseState.Position.X * offset.X, myCurrentMouseState.Position.Y * offset.Y);
        }

        /// <summary>
        /// Returnerar om musen är innanför fönstret.
        /// </summary>
        public static bool MouseWithinWindow(GameWindow window)
        {
            Rectangle screen = new Rectangle(0, 0, window.ClientBounds.Width, window.ClientBounds.Height);
            return screen.Contains(MousePos());
        }
    }
}