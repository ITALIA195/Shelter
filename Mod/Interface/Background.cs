﻿using UnityEngine;

namespace Mod.Interface
{
    public class Background : Gui
    {
        private Texture2D background;

        public Background()
        {
            Enable();
        }

        public override void OnShow()
        {
            background = GetImage("Background");
        }

        public override void Render()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);
        }

        public override void OnHide()
        {
            Destroy(background);
        }
    }
}