// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace NanoGames.Application
{
    /// <summary>
    /// Contains the persistent game settings.
    /// </summary>
    internal sealed class Settings
    {
        private static readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NanoGames", "Settings.json");

        private bool _enableSaving = false;

        private bool _showFps = false;
        private bool _isFullscreen = false;
        private string _playerName = "ANON";
        private string _lastServer = string.Empty;

        private Settings()
        {
        }

        /// <summary>
        /// Gets the current settings.
        /// </summary>
        public static Settings Instance { get; } = Load();

        /// <summary>
        /// Gets or sets a value indicating whether to show the FPS.
        /// </summary>
        public bool ShowFps
        {
            get
            {
                return _showFps;
            }

            set
            {
                _showFps = value;
                Save();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window should be fullscreen.
        /// </summary>
        public bool IsFullscreen
        {
            get
            {
                return _isFullscreen;
            }

            set
            {
                _isFullscreen = value;
                Save();
            }
        }

        /// <summary>
        /// Gets or sets the player name.
        /// </summary>
        public string PlayerName
        {
            get
            {
                return _playerName;
            }

            set
            {
                _playerName = value;
                Save();
            }
        }

        /// <summary>
        /// Gets or sets the last server.
        /// </summary>
        public string LastServer
        {
            get
            {
                return _lastServer;
            }

            set
            {
                _lastServer = value;
                Save();
            }
        }

        private static Settings Load()
        {
            Settings settings = null;
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var settingsString = File.ReadAllText(_settingsPath, Encoding.UTF8);
                    settings = JsonConvert.DeserializeObject<Settings>(settingsString);
                }
            }
            catch
            {
                settings = new Settings();
            }

            if (settings == null)
            {
                settings = new Settings();
            }

            settings._enableSaving = true;

            return settings;
        }

        private void Save()
        {
            if (_enableSaving)
            {
                var settingsString = JsonConvert.SerializeObject(this, Formatting.Indented);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));
                    File.WriteAllText(_settingsPath, settingsString, Encoding.UTF8);
                }
                catch
                {
                }
            }
        }
    }
}
