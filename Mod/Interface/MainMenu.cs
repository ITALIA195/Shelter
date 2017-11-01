﻿using System.Globalization;
using UnityEngine;

namespace Mod.Interface
{
    public class MainMenu : Gui
    {
        private Texture2D btnNormal;
        private Texture2D btnHover;
        private Texture2D btnActive;
        private GUIStyle serverSelect;
        private GUIStyle text;
        private GUIStyle selected;

        public MainMenu()
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectToMaster(PlayerPrefs.GetString("ShelterServer", "app-eu.exitgamescloud.com"), 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
//            PhotonNetwork.ConnectToMaster("localhost", 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
            Enable();
        }

        public override void OnShow()
        {
            btnNormal = Shelter.CreateTexture(169, 169, 169, 100);
            btnHover = Shelter.CreateTexture(169, 169, 169, 255);
            btnActive = Shelter.CreateTexture(134, 134, 134, 255);
            serverSelect = new GUIStyle(GUIStyle.none)
            {
                normal = {background = btnNormal},
                hover = {background = btnHover},
                active = {background = btnActive},
                alignment = TextAnchor.MiddleCenter
            };
            text = new GUIStyle
            {
                fontSize = 20,
                normal = { textColor = Color(178, 102, 106) },
                alignment = TextAnchor.MiddleCenter,
            };
            selected = new GUIStyle(text)
            {
                fontStyle = FontStyle.Bold
            };
        }

        public override void Render()
        {
            Rect rect;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1)); // Scale the gui for any resoultion
            GUI.Label(rect = new Rect(145, 347, 166, 40), "Create", IsVisible("CreateRoom") ? selected : text);
            if (GUI.Button(new Rect(rect.x, rect.y - 10f, rect.width, rect.height), string.Empty, GUIStyle.none))
            {
                Disable("ServerList");
                Enable("CreateRoom");
            }
            GUI.Label(rect = new Rect(143, 370, 167, 40), "Server list", IsVisible("ServerList") ? selected : text);
            if (GUI.Button(new Rect(rect.x, rect.y - 10f, rect.width, rect.height), string.Empty, GUIStyle.none))
            {
                Disable("CreateRoom");
                Enable("ServerList");
            }

            GUI.matrix = Matrix4x4.identity;
            GUILayout.BeginArea(new Rect(Screen.width - 250f, 0, 150f, 40f));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("EU", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
                PlayerPrefs.SetString("ShelterServer", "app-eu.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            if (GUILayout.Button("US", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-us.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
                PlayerPrefs.SetString("ShelterServer", "app-us.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            if (GUILayout.Button("JPN", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-jp.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
                PlayerPrefs.SetString("ShelterServer", "app-jp.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            if (GUILayout.Button("ASIA", serverSelect))
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectToMaster("app-asia.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
                PlayerPrefs.SetString("ShelterServer", "app-asia.exitgamescloud.com");
                Loading.Start("ConnectingToLobby");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }

        public override void OnHide()
        {
            Destroy(btnActive);
            Destroy(btnHover);
            Destroy(btnNormal);
        }
    }
}