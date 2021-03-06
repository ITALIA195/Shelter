﻿using System.Text;
using Photon;
using UnityEngine;

namespace Mod.Interface
{
    public class ServerList : Gui
    {
        private string _filter = string.Empty;
        
        private GUIStyle filterStyle;
        private GUIStyle searchStyle;
        private GUIStyle buttonStyle;
        private Texture2D buttonHover;
        private Texture2D buttonActive;
        private Texture2D background;
        private Vector2 _scrollPosition;
        
        public static float Alpha;

        private const float Width = 1280;
        private const float Height = 720;
        
        private float _width;
        private float _height;

        protected override void OnShow()
        {
            if (PhotonNetwork.connectionStatesDetailed != ClientState.JoinedLobby)
                Loading.Show();
            buttonHover = Texture(255, 255, 255, 70);
            buttonActive = Texture(255, 255, 255, 140);
            filterStyle = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 22,
                normal = { textColor = Color(255, 255, 255), background = null },
                active = { background = buttonActive },
                hover = { background = buttonHover, textColor = Color(250, 250, 250) },
                border = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.UpperCenter
            };
            searchStyle = new GUIStyle
            {
                fontSize = 22,
                normal = { textColor = Color(110, 110, 110) },
                alignment = TextAnchor.MiddleCenter
            };
            buttonStyle = new GUIStyle(GUIStyle.none)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                active = {background = buttonActive},
                hover = {background = buttonHover}
            };
            background = Texture(255, 255, 255, 63);
        }

        private void Animation()
        {
            if (_width < Width || _height < Height)
            {
                const float changeInValue = 1.5f;
                const float slowdown = 0.5f;
                
                _width += (Width * changeInValue - _width * slowdown) * Time.deltaTime;
                _height += (Height * changeInValue - _height * slowdown) * Time.deltaTime;
                
                if (_width < Width || _height < Height)
                    return;
                
                _width = Width;
                _height = Height;
            }
            else if (PhotonNetwork.connectionStatesDetailed == ClientState.JoinedLobby && PhotonNetwork.countOfRooms > 0)
            {
                Alpha += Time.deltaTime * 100;
                if (Alpha < 255) return;
                Alpha = Mathf.Clamp(Alpha, 0, 255);
            }
        }

        protected override void Render()
        {
            if (Event.current.type == EventType.KeyDown)
                GUI.FocusControl("Search");

            Rect windowRect = new Rect(Screen.width / 2f - _width / 2f, Screen.height / 2f - _height / 2f, _width, _height);
            GUI.DrawTexture(windowRect, background);
            if (Alpha < 255f)
                Animation();

            if (GUI.GetNameOfFocusedControl() != "Search" && string.IsNullOrEmpty(_filter))
                GUI.Label(new Rect(windowRect.x, windowRect.y, windowRect.width, 33), "Search...", searchStyle);
            GUILayout.BeginArea(windowRect);
            GUI.SetNextControlName("Search");
            _filter = GUILayout.TextField(_filter.Replace("\n", ""), filterStyle);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false, GUIStyle.none, GUIStyle.none);
            foreach (Room room in Room.List)
            {
                if (_filter != string.Empty && !room.Name.RemoveColors().ContainsIgnoreCase(_filter) && !room.Map.Name.ContainsIgnoreCase(_filter))
                    continue;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(GetRoomName(room), buttonStyle))
                    Connecting.ConnectTo(room);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static string GetRoomName(Room room)
        {
            var alpha = ((byte) Alpha).ToString("X2");
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<color=#5D334B{0}>", alpha);
            {
                if (room.IsProtected)
                    builder.AppendFormat("<color=#034C94{0}>[</color>" +
                                         "<color=#1191D1{0}>PW</color>" +
                                         "<color=#034C94{0}>]</color> ", alpha);
                if (!room.IsOpen)
                    builder.AppendFormat("<color=#034C94{0}>[</color>" +
                                         "<color=#FF0000{0}>CLOSED</color>" +
                                         "<color=#034C94{0}>]</color> ", alpha);
                builder.Append(room.Name.RemoveColors());
                builder.Append(" || ");
                builder.Append(room.Map.Name);
                builder.Append(" || ");

                var color = room.IsJoinable ? "00FF00" : "FF0000";
                builder.AppendFormat("<color=#{1}{0}>{2}/{3}</color>", alpha, color, room.Players, room.MaxPlayers);
            }
            builder.Append("</color>");
            return builder.ToString();
        }
        
        protected override void OnHide()
        {
            Alpha = 0f;
            _width = 0f;
            _height = 0f;
            Destroy(background);
            Destroy(buttonActive);
            Destroy(buttonHover);
        }
    }
}
