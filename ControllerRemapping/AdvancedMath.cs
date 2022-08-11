using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace ControllerRemapping
{
    internal static class AdvancedMath
    {
        public const float Deg2Rad = ((float)Math.PI * 2) / 360; //unity
        public const float Rad2Deg = 360 / ((float)Math.PI * 2);//unity

        /// <summary>
        /// Dampar lerp mellan två floats. Stulen från unity.
        /// </summary>
        public static float SmoothDamp(float aCurrent, float aTarget, ref float aCurrentVelocity, float aSmoothTime, float aMaxSpeed, float aDeltaTime) // Obs Denna funktionen är tagen direkt från Unity där den är inbyggd. Jag skäms men jag behöver den för att göra någon dampening
        {
            aSmoothTime = Math.Max(0.0001f, aSmoothTime);
            float num = 2f / aSmoothTime;
            float num2 = num * aDeltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = aCurrent - aTarget;
            float num5 = aTarget;
            float num6 = aMaxSpeed * aSmoothTime;
            num4 = Math.Clamp(num4, -num6, num6);
            aTarget = aCurrent - num4;
            float num7 = (aCurrentVelocity + num * num4) * aDeltaTime;
            aCurrentVelocity = (aCurrentVelocity - num * num7) * num3;
            float num8 = aTarget + (num4 + num7) * num3;
            if (num5 - aCurrent > 0f == num8 > num5)
            {
                num8 = num5;
                aCurrentVelocity = (num8 - num5) / aDeltaTime;
            }
            return num8;
        }

        /// <summary>
        /// Räknar ut skillnaden i grader mellan två punkter. Inte min kod.
        /// </summary>
        public static float AngleBetween(Vector2 aVectOne, Vector2 aVectTwo)
        {
            float dotProd;
            float Ratio;
            dotProd = Vector2.Dot(aVectOne, aVectTwo);
            Ratio = dotProd / aVectOne.Length();
            return (float)(Math.Acos(Ratio)) * Rad2Deg;
        }

        /// <summary>
        /// Räknar ut avståndet mellan två punkter.
        /// </summary>
        public static float Vector2Distance(Vector2 aVectOne, Vector2 aVectTwo)
        {
            return Magnitude(aVectOne - aVectTwo);
        }

        /// <summary>
        /// Kollar om magnituden av en vektor är mindre än ett visst avstånd.
        /// </summary>
        public static bool Vector2WithinDistance(Vector2 aVectOne, float aDistance)
        {
            return SqrMagnitude(aVectOne) < aDistance * aDistance;
        }

        /// <summary>
        /// Gör om en grad till en vektor. Inte min kod
        /// </summary>
        public static Vector2 AngleToVector(float aAngle)
        {
            return new Vector2((float)Math.Cos(aAngle), (float)Math.Sin(aAngle));
        }

        /// <summary>
        /// Gör om en vector till en grad. Inte min kod
        /// </summary>
        public static float VectorToAngle(Vector2 aVector)
        {
            return (float)Math.Atan2(aVector.Y, aVector.X);
        }

        /// <summary>
        /// Roterar en vektor baserat på grader i deg. Stulen från unity.
        /// </summary>
        public static Vector2 Rotate(Vector2 aVect, float aDegrees)
        {
            float sin = (float)Math.Sin(aDegrees * Deg2Rad);
            float cos = (float)Math.Cos(aDegrees * Deg2Rad);

            float tx = aVect.X;
            float ty = aVect.Y;
            aVect.X = (cos * tx) - (sin * ty);
            aVect.Y = (sin * tx) + (cos * ty);
            return aVect;
        }

        /// <summary>
        /// Clampar magnituden av en vektor till en viss max längd. Inte min kod.
        /// </summary>
        public static Vector2 ClampMagnitude(Vector2 aVector, float aMaxLength)
        {
            float sqrMagnitude = AdvancedMath.SqrMagnitude(aVector);
            if (sqrMagnitude > aMaxLength * aMaxLength)
            {
                float mag = (float)Math.Sqrt(sqrMagnitude);
                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = aVector.X / mag;
                float normalized_y = aVector.Y / mag;
                return new Vector2(normalized_x * aMaxLength, normalized_y * aMaxLength);
            }
            return aVector;
        }

        /// <summary>
        /// Sätter magnituden av en vektor. Inte min kod.
        /// </summary>
        public static Vector2 SetMagnitude(Vector2 aVector, float aLength)
        {
            float sqrMagnitude = SqrMagnitude(aVector);
            float mag = (float)Math.Sqrt(sqrMagnitude);
            float normalized_x = aVector.X / mag;
            float normalized_y = aVector.Y / mag;
            return new Vector2(normalized_x * aLength, normalized_y * aLength);
        }

        /// <summary>
        /// Normaliserar en vektor. Min men inspirerad av clampMagnitude.
        /// </summary>
        public static Vector2 Normalize(Vector2 aVector2)
        {
            float mag = Magnitude(aVector2);
            float normalized_x = aVector2.X / mag;
            float normalized_y = aVector2.Y / mag;
            return new Vector2(normalized_x, normalized_y);
        }

        /// <summary>
        /// Räknar ut den kvadrerade magnituden(längden) av en vektor. Stulen från unity.
        /// </summary>
        public static float SqrMagnitude(Vector2 aVect)
        {
            return aVect.X * aVect.X + aVect.Y * aVect.Y;
        }

        /// <summary>
        /// Räknar ut magnituden(längden) av en vektor. Stulen från unity.
        /// </summary>
        public static float Magnitude(Vector2 aVect)
        {
            return (float)Math.Sqrt(SqrMagnitude(aVect));
        }

        /// <summary>
        /// Delar upp en texture som spritesheet. Inte min kod.
        /// </summary>
        public static Texture2D[] Split(Texture2D aOriginal, int aPartWidth, int aPartHeight, out int aXCount, out int aYCount)
        {
            aYCount = aOriginal.Height / aPartHeight /*+ (partHeight % original.Height == 0 ? 0 : 1)*/;//The number of textures in each horizontal row
            aXCount = aOriginal.Width / aPartWidth /*+ (partWidth % original.Width == 0 ? 0 : 1)*/;//The number of textures in each vertical column
            Texture2D[] r = new Texture2D[aXCount * aYCount];//Number of parts = (area of original) / (area of each part).
            int dataPerPart = aPartWidth * aPartHeight;//Number of pixels in each of the split parts

            //Get the pixel data from the original texture:
            Color[] originalData = new Color[aOriginal.Width * aOriginal.Height];
            aOriginal.GetData<Color>(originalData);

            int index = 0;
            for (int y = 0; y < aYCount * aPartHeight; y += aPartHeight)
            {
                for (int x = 0; x < aXCount * aPartWidth; x += aPartWidth)
                {
                    //The texture at coordinate {x, y} from the top-left of the original texture
                    Texture2D part = new Texture2D(aOriginal.GraphicsDevice, aPartWidth, aPartHeight);
                    //The data for part
                    Color[] partData = new Color[dataPerPart];

                    //Fill the part data with colors from the original texture
                    for (int py = 0; py < aPartHeight; py++)
                    {
                        for (int px = 0; px < aPartWidth; px++)
                        {
                            int partIndex = px + py * aPartWidth;
                            //If a part goes outside of the source texture, then fill the overlapping part with Color.Transparent
                            if (y + py >= aOriginal.Height || x + px >= aOriginal.Width)
                                partData[partIndex] = Color.Transparent;
                            else
                                partData[partIndex] = originalData[(x + px) + (y + py) * aOriginal.Width];
                        }
                    }
                    //Fill the part with the extracted data
                    part.SetData<Color>(partData);
                    //Stick the part in the return array:
                    r[index++] = part;
                }
            }
            //Return the array of parts.
            return r;
        }

        /// <summary>
        /// inte min kod. Flippar textur.
        /// </summary>
        public static Texture2D SaveAsFlippedTexture2D(Texture2D aInput, bool aVertical, bool aHorizontal)
        {
            Texture2D flipped = new Texture2D(aInput.GraphicsDevice, aInput.Width, aInput.Height);
            Color[] data = new Color[aInput.Width * aInput.Height];
            Color[] flipped_data = new Color[data.Length];

            aInput.GetData<Color>(data);

            for (int x = 0; x < aInput.Width; x++)
            {
                for (int y = 0; y < aInput.Height; y++)
                {
                    int index = 0;
                    if (aHorizontal && aVertical)
                        index = aInput.Width - 1 - x + (aInput.Height - 1 - y) * aInput.Width;
                    else if (aHorizontal && !aVertical)
                        index = aInput.Width - 1 - x + y * aInput.Width;
                    else if (!aHorizontal && aVertical)
                        index = x + (aInput.Height - 1 - y) * aInput.Width;
                    else if (!aHorizontal && !aVertical)
                        index = x + y * aInput.Width;

                    flipped_data[x + y * aInput.Width] = data[index];
                }
            }

            flipped.SetData<Color>(flipped_data);

            return flipped;
        }

        /// <summary>
        /// Avrundar till närmasta bråket. exempel: fraction är 5. avrundar till närmsta femtedelen.
        /// </summary>
        public static float RoundToNearest(float aInput, float aFraction)
        {
            return (float)(Math.Round((double)aInput * (double)aFraction, MidpointRounding.ToEven) / aFraction);
        }

        /// <summary>
        /// Lerpar mellan två värden.
        /// </summary>
        public static float Lerp(float aMin, float aMax, float aValue) // stulen från Unity
        {
            return aMin + (aMax - aMin) * Math.Clamp(aValue, 0, 1);
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) // stulen från Unity
        {
            return new Vector2(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        /// <summary>
        /// Ändrar två normaliserade radianer så det är så lite skillnad som möjligt mellan dem men de fortfarande har samma värde så lerp funkar.
        /// </summary>
        public static void AngleDifference(ref float angle1, ref float angle2)
        {
            if (Math.Abs(angle1 - angle2) < Math.PI)
            {
                return;
            }
            else if (Math.Abs(angle1 + 2 * (float)Math.PI - angle2) < Math.PI)
            {
                angle1 += 2 * (float)Math.PI;
            }
            else if (Math.Abs(angle2 + 2 * (float)Math.PI - angle1) < Math.PI)
            {
                angle2 += 2 * (float)Math.PI;
            }
        }

        /// <summary>
        /// Ändrar en normaliserad radian så att den går mellan minus pi och positiv pi.
        /// </summary>
        public static float AngleDifference(float angle1)
        {
            if (Math.Abs(angle1) < Math.PI)
            {
                return angle1;
            }
            else if (Math.Abs(angle1 + 2 * (float)Math.PI) < Math.PI)
            {
                angle1 += 2 * (float)Math.PI;
            }
            else if (Math.Abs(2 * (float)Math.PI - angle1) < Math.PI)
            {
                angle1 -= 2 * (float)Math.PI;
            }
            return angle1;
        }

        /// <summary>
        /// Normaliserar radianer mellan 0 och 2 pi.
        /// </summary>
        public static float NormalizeRadians(float rad)
        {
            while (rad < 0)
            {
                rad += 2 * (float)Math.PI;
            }
            while (rad > 2 * Math.PI)
            {
                rad -= 2 * (float)Math.PI;
            }
            return rad;
        }

        /// <summary>
        /// Ger tillbaka random float mellan värden
        /// </summary>
        public static float Random(Random rng, float min, float max)
        {
            return (float)rng.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Kollar om regioner av textur är genomskinlig. Inte min kod
        /// </summary>
        public static bool IsRegionTransparent(Texture2D texture, Rectangle r)
        {
            int size = r.Width * r.Height;
            Color[] buffer = new Color[size];
            texture.GetData(0, r, buffer, 0, size);
            return buffer.All(c => c == Color.Transparent);
        }

        /// <summary>
        /// Kollar om hela texturen är genomskinlig.
        /// </summary>
        public static bool IsRegionTransparent(Texture2D texture)
        {
            Rectangle r = new Rectangle(0, 0, texture.Width, texture.Height);
            int size = r.Width * r.Height;
            Color[] buffer = new Color[size];
            texture.GetData(0, r, buffer, 0, size);
            return buffer.All(c => c == Color.Transparent);
        }

        /// <summary>
        /// Returnerar närmsta heltalsmultiplikationen. Inte min kod.
        /// </summary>
        public static int GetNearestMultiple(int value, int factor)
        {
            return (int)Math.Round((value / (double)factor), MidpointRounding.AwayFromZero) * factor;
        }

        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        }
    }
}