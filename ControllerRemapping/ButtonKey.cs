using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace ControllerRemapping
{
    internal enum ButtonOrKey
    {
        Button,
        Key,
    }

    internal class ButtonKey
    {
        public Keys AccessKey { get; set; }
        public Buttons AccessButton { get; set; }

        public ButtonOrKey ButtonOrKey { get; set; }

        public ButtonKey(Keys key)
        {
            AccessKey = key;
            ButtonOrKey = ButtonOrKey.Key;
        }

        public ButtonKey(Buttons button)
        {
            AccessButton = button;
            ButtonOrKey = ButtonOrKey.Button;
        }

        public override string ToString()
        {
            string temp = ButtonOrKey.ToString() + " ";
            if (ButtonOrKey == ButtonOrKey.Button)
            {
                temp += AccessButton.ToString();
            }
            else if (ButtonOrKey == ButtonOrKey.Key)
            {
                temp += AccessKey.ToString();
            }
            return temp;
        }
    }
}