using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ControllerRemapping
{
    internal class Sprite
    {
        protected Vector2 myOrigin;
        public Vector2 AccessOrigin { get => myOrigin; set => myOrigin = value; }

        protected Texture2D myTexture;
        public Texture2D AccessTex { get => myTexture; set => myTexture = value; }

        protected Vector2 myPosition;
        public Vector2 AccessPosition { get => myPosition; set => myPosition = value; }

        protected float myActualScale = 1;

        public float AccessScale
        {
            get
            {
                return myActualScale;
            }
            set
            {
                myActualScale = value;
                Initialize();
            }
        }

        protected float myRotation = 0;
        public float AccessRotation { get => myRotation; set => myRotation = value; }

        protected Rectangle myRectangle;
        public Rectangle AccessRectangle { get => myRectangle; set => myRectangle = value; }

        public Color[] myTextureData;
        public Color[] AccessTextureData { get => myTextureData; set => myTextureData = value; }

        protected SpriteEffects mySpriteEffects = SpriteEffects.None;

        public SpriteEffects AccessSpriteEffects { get => mySpriteEffects; set => mySpriteEffects = value; }

        public Matrix AccessTransform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-new Vector2(myOrigin.X * myTexture.Width, myOrigin.Y * myTexture.Height), 0)) *
                  Matrix.CreateRotationZ(myRotation) *
                  Matrix.CreateTranslation(new Vector3(myPosition, 0));
            }
        }

        public Sprite(Texture2D _tex, Vector2 _position)
        {
            myTexture = _tex;
            myPosition = _position;
            myOrigin = new Vector2(0.5f, 0.5f);
            Initialize();
        }

        public Sprite(Texture2D _tex, Vector2 _position, Vector2 _origin)
        {
            myTexture = _tex;
            myPosition = _position;
            myOrigin = _origin;
            Initialize();
        }

        public Sprite(Texture2D _tex, RenderTarget2D renderTarget)
        {
            myTexture = _tex;
            myPosition = new Vector2(renderTarget.Width / 2, renderTarget.Height / 2);
            myOrigin = new Vector2(0.5f, 0.5f);
            Initialize();
            //origin = new Vector2();
        }

        public virtual void Initialize() // Sätter upp kollisions skiten
        {
            myTextureData = new Color[(myTexture.Width) * (myTexture.Height)];
            myTexture.GetData(myTextureData);
            myRectangle = new Rectangle((int)(myPosition.X - (myTexture.Width * myOrigin.X)), (int)(myPosition.Y - (myTexture.Height * myOrigin.Y)), (int)(myTexture.Width * AccessScale), (int)(myTexture.Height * AccessScale));
        }

        public virtual void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(myTexture, myPosition, null, Color.White, myRotation, new Vector2(myOrigin.X * myTexture.Width, myOrigin.Y * myTexture.Height), AccessScale, mySpriteEffects, 1);
        }

        public virtual void Draw(SpriteBatch _spriteBatch, Color color)
        {
            _spriteBatch.Draw(myTexture, myPosition, null, color, myRotation, new Vector2(myOrigin.X * myTexture.Width, myOrigin.Y * myTexture.Height), AccessScale, mySpriteEffects, 1);
        }

        public virtual void DrawBox(SpriteBatch _spriteBatch, Color color, Texture2D debugTex)
        {
            //_spriteBatch.Draw(debugTex, position, null, color, rotation, new Vector2(origin.X * tex.Width, origin.Y * tex.Height), playerScale, SpriteEffects.None, 2);
            _spriteBatch.Draw(debugTex, myRectangle, color);
        }

        public virtual void UpdatePos(float deltaTime)
        {
            myRectangle.X = (int)(myPosition.X - (myTexture.Width * AccessScale * myOrigin.X));
            myRectangle.Y = (int)(myPosition.Y - (myTexture.Height * AccessScale * myOrigin.Y));

            //myRectangle.X = (int)(myPosition.X - (myTexture.Width * AccessScale) / 2);
            //myRectangle.Y = (int)(myPosition.Y - (myTexture.Height * AccessScale) / 2);
        }

        public virtual void UpdatePos(float deltaTime, RenderTarget2D renderTarget)
        {
            myRectangle.X = (int)(myPosition.X - (myTexture.Width * myOrigin.X));
            myRectangle.Y = (int)(myPosition.Y - (myTexture.Height * myOrigin.Y));

            //myRectangle.X = (int)(myPosition.X - (myTexture.Width * AccessScale) / 2);
            //myRectangle.Y = (int)(myPosition.Y - (myTexture.Height * AccessScale) / 2);

            //myRectangle = new Rectangle((int)(myPosition.X - (myTexture.Width * myOrigin.X)), (int)(myPosition.Y - (myTexture.Height * myOrigin.Y)), (int)(myTexture.Width * AccessScale), (int)(myTexture.Height * AccessScale));
        }

        /// <summary>
        /// Kollar pixel perfect om den kolliderar. Inte min kod.
        /// </summary>
        public virtual bool Intersects(Sprite sprite)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            var transformAToB = this.AccessTransform * Matrix.Invert(sprite.AccessTransform);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            var yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < this.myRectangle.Height; yA++)
            {
                // Start at the beginning of the row
                var posInB = yPosInB;

                for (int xA = 0; xA < this.myRectangle.Width; xA++)
                {
                    // Round to the nearest pixel
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < sprite.myRectangle.Width &&
                        0 <= yB && yB < sprite.myRectangle.Height)
                    {
                        // Get the colors of the overlapping pixels
                        try
                        {
                            var colourA = this.myTextureData[xA + yA * this.myRectangle.Width];
                            var colourB = sprite.myTextureData[xB + yB * sprite.myRectangle.Width];

                            // If both pixel are not completely transparent
                            if (colourA.A != 0 && colourB.A != 0)
                            {
                                return true;
                            }
                        }
                        catch (Exception)
                        {
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Kollar om två rektanglar rör varandra
        /// </summary>
        public virtual bool BasicIntersects(Sprite sprite)
        {
            return sprite.myRectangle.Intersects(myRectangle);
        }

        /// <summary>
        /// Kollar om två rektanglar rör varandra
        /// </summary>
        public virtual bool BasicIntersects(Rectangle aRectangle)
        {
            return aRectangle.Intersects(myRectangle);
        }

        public virtual int PartOutsideScreen(RenderTarget2D renderTarget)
        {
            if (myRectangle.Top < 0)
            {
                // Top outside screen
                return 0;
            }
            if (myRectangle.Bottom > renderTarget.Height)
            {
                // Bottom outside screen
                return 1;
            }
            if (myRectangle.Left < 0)
            {
                // Left outside screen
                return 2;
            }
            if (myRectangle.Right > renderTarget.Width)
            {
                // Right outside screen
                return 3;
            }
            return -1;
        }

        public virtual int PartOutsideScreen(GameWindow window)
        {
            if (myRectangle.Top < 0)
            {
                // Top outside screen
                return 0;
            }
            if (myRectangle.Bottom > window.ClientBounds.Height)
            {
                // Bottom outside screen
                return 1;
            }
            if (myRectangle.Left < 0)
            {
                // Left outside screen
                return 2;
            }
            if (myRectangle.Right > window.ClientBounds.Width)
            {
                // Right outside screen
                return 3;
            }
            return -1;
        }

        /// <summary>
        /// Räknar ut vilken riktning som är upp för spriten. Inte min kod.
        /// </summary>
        public Vector2 Up()
        {
            Vector2 direction = new Vector2((float)Math.Cos(myRotation), (float)Math.Sin(myRotation));
            direction.Normalize();
            return AdvancedMath.Rotate(new Vector2(1, 1) * direction, 90);
        }

        /// <summary>
        /// Räknar ut vilken riktning som är åt höger för spriten. Inte min kod.
        /// </summary>
        public Vector2 Right()
        {
            Vector2 direction = new Vector2((float)Math.Cos(myRotation), (float)Math.Sin(myRotation));
            direction.Normalize();
            return AdvancedMath.Rotate(new Vector2(1, 1) * direction, 180);
        }

        /// <summary>
        /// Skapar en rektangel som täcker hela spriten oavsett rotation eller skala. Inte min kod.
        /// </summary>
        public Rectangle BoundingBox()
        {
            int x, y, w, h;
            if (myRotation != 0)
            {
                var cos = Math.Abs(Math.Cos(myRotation));
                var sin = Math.Abs(Math.Sin(myRotation));
                var t1_opp = myTexture.Width * AccessScale * cos;
                var t1_adj = Math.Sqrt(Math.Pow(myTexture.Width * AccessScale, 2) - Math.Pow(t1_opp, 2));
                var t2_opp = myTexture.Height * AccessScale * sin;
                var t2_adj = Math.Sqrt(Math.Pow(myTexture.Height * AccessScale, 2) - Math.Pow(t2_opp, 2));

                w = (int)(t1_opp + t2_opp);
                h = (int)(t1_adj + t2_adj);
                x = (int)(myPosition.X - (w / 2));
                y = (int)(myPosition.Y - (h / 2));
            }
            else
            {
                x = (int)myPosition.X;
                y = (int)myPosition.Y;
                w = (int)(myTexture.Width * AccessScale);
                h = (int)(myTexture.Height * AccessScale);
            }
            return new Rectangle(x, y, w, h);
        }
    }
}