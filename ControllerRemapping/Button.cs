using ControllerRemapping;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ControllerRemapping
{
    internal class Button
    {
        private Rectangle myRectangle;
        public Rectangle AccessRectangle { private set => myRectangle = value; get => myRectangle; }

        private Texture2D myTexture;
        public Texture2D AccessTexture { set => myTexture = value; get => myTexture; }

        private string myText;
        public string AccessText { private set => myText = value; get => myText; }

        private bool myPressed;
        public bool AccessPressed { private set => myPressed = value; get => myPressed; }

        private Color myRectangleColor;
        public Color AccessRectangleColor { private set => myRectangleColor = value; get => myRectangleColor; }

        private Color myTextColor;
        public Color AccessTextColor { private set => myTextColor = value; get => myTextColor; }

        public Button(Rectangle aRectangle, Texture2D aTexture, string aText)
        {
            myRectangle = aRectangle;
            myTexture = aTexture;
            myText = aText;
            myPressed = false;
            myRectangleColor = Color.White;
            myTextColor = Color.White;
        }

        public Button(Rectangle aRectangle, Texture2D aTexture, string aText, Color aRectangleColor)
        {
            myRectangle = aRectangle;
            myTexture = aTexture;
            myText = aText;
            myPressed = false;
            myRectangleColor = aRectangleColor;
            myTextColor = Color.White;
        }

        public bool Clicked() // Kollar om knappen har blivit tryckt
        {
            if (myPressed)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (myRectangle.Contains(Input.MousePos().X, Input.MousePos().Y))
                    {
                        myPressed = false;
                        return true;
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (myRectangle.Contains(Input.MousePos().X, Input.MousePos().Y))
                {
                    myPressed = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                myPressed = false;
            }
            return false;
        }

        public void SetPos(int aX, int aY)
        {
            myRectangle = new Rectangle(aX, aY, myRectangle.Width, myRectangle.Height);
            //rectangle.Y = y;
        }

        public void SetSize(int aWidth, int aHeight)
        {
            myRectangle = new Rectangle(myRectangle.X, myRectangle.Y, aWidth, aHeight);
            //rectangle.Y = y;
        }

        public void SetSize(Vector2 aSize)
        {
            myRectangle = new Rectangle(myRectangle.X, myRectangle.Y, (int)aSize.X, (int)aSize.Y);
            //rectangle.Y = y;
        }

        public void Draw(SpriteBatch aSpriteBatch, SpriteFont aFont/*, Vector3 offset*/)
        {
            //setPos(rectangle.X - (int)offset.X, rectangle.Y - (int)offset.Y);
            //_spriteBatch.Draw(texture, new Rectangle(rectangle.X - (int)offset.X, rectangle.Y - (int)offset.Y, rectangle.Width, rectangle.Height), Color.White);
            aSpriteBatch.Draw(myTexture, myRectangle, myRectangleColor);
            Vector2 size = aFont.MeasureString(myText);

            aSpriteBatch.DrawString(aFont, myText, new Vector2(myRectangle.X + myRectangle.Width / 2 /*- offset.X*/, myRectangle.Y + myRectangle.Height / 2 /*- offset.Y*/), myTextColor, 0, new Vector2(size.X / 2, size.Y / 2), 1.2f, SpriteEffects.None, 1);
        }

        public void Draw(SpriteBatch aSpriteBatch, SpriteFont aFont, Color aColor/*, Vector3 offset*/)
        {
            //setPos(rectangle.X - (int)offset.X, rectangle.Y - (int)offset.Y);
            //_spriteBatch.Draw(texture, new Rectangle(rectangle.X - (int)offset.X, rectangle.Y - (int)offset.Y, rectangle.Width, rectangle.Height), Color.White);
            aSpriteBatch.Draw(myTexture, myRectangle, aColor);
            Vector2 size = aFont.MeasureString(myText);

            aSpriteBatch.DrawString(aFont, myText, new Vector2(myRectangle.X + myRectangle.Width / 2 /*- offset.X*/, myRectangle.Y + myRectangle.Height / 2 /*- offset.Y*/), myTextColor, 0, new Vector2(size.X / 2, size.Y / 2), 1.2f, SpriteEffects.None, 1);
        }
    }
}