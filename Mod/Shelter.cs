﻿using Mod.Managers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Mod
{
    public class Shelter : MonoBehaviour
    {
        public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        public static Stopwatch Stopwatch = Stopwatch.StartNew();
        private static InterfaceManager _interfaceManager;
        private static CommandManager _commandManager;

        public void InitComponents()
        {
            _interfaceManager = new InterfaceManager();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) && Input.GetKey(KeyCode.LeftControl))
                File.WriteAllLines($"GameObjects{UnityEngine.Random.Range(0, 255)}.txt", FindObjectsOfType(typeof(GameObject)).OrderBy(x => x.GetInstanceID()).Select(x => $"{x.GetInstanceID()} - {x.name}").ToArray());
        }

        public static void OnMainMenu()
        {
            _commandManager?.Dispose();
            _commandManager = null;
            _interfaceManager.DisableAll();
            _interfaceManager.Enable("Background");
            _interfaceManager.Enable("Loading");
            _interfaceManager.Enable("MainMenu");
        }

        public static void OnJoinedGame()
        {
            _commandManager = new CommandManager();
            _interfaceManager.DisableAll();
            _interfaceManager.Enable("Chat");
            _interfaceManager.Enable("Scoreboard");
        }

        #region Static methods

        public static void Log(params object[] messages)
        {
            foreach (var obj in messages)
                Log(obj);
        }

        public static void Log(object msg)
        {

        }

        public static Texture2D GetImage(string image)
        {
            if (Assembly.GetManifestResourceInfo($@"Mod.Resources.{image}.png") == null) return null;
            using (var stream = Assembly.GetManifestResourceStream($@"Mod.Resources.{image}.png"))
            {
                var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                texture.LoadImage(stream.ToBytes());
                texture.Apply();
                return texture;
            }
        }

        #endregion

        public static InterfaceManager InterfaceManager => _interfaceManager;
        public static CommandManager CommandManager => _commandManager;
    }
}
