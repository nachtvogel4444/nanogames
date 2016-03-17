// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Application.Ui;
using NanoGames.Engine;
using System;

namespace NanoGames.Application
{
    /// <summary>
    /// The settings view.
    /// </summary>
    internal sealed class SettingsView : IView
    {
        private readonly Action _goBack;
        private readonly Menu _menu;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        /// <param name="goBack">The action to invoke when leaving the settings menu.</param>
        public SettingsView(Action goBack)
        {
            _goBack = goBack;
            _menu = new Menu("SETTINGS")
            {
                OnBack = _goBack,
                SelectedIndex = 1,
                Items =
                {
                    new TextMenuItem("NAME")
                    {
                        Text = Settings.Instance.PlayerName,
                        MaxLength = Settings.MaxPlayerNameLength,
                        OnChange = value => Settings.Instance.PlayerName = value,
                    },
                    new ChoiceMenuItem<bool>("FULLSCREEN")
                    {
                        Choices =
                        {
                            new Choice<bool>(false, "NO"),
                            new Choice<bool>(true, DebugMode.IsEnabled ? "DEBUG" : "YES"),
                        },
                        SelectedValue = Settings.Instance.IsFullscreen,
                        OnSelect = v =>
                        {
                            Window.Current.IsFullscreen = v;
                            Settings.Instance.IsFullscreen = v;
                        },
                    },
                    new ChoiceMenuItem<bool>("VSYNC")
                    {
                        Choices =
                        {
                            new Choice<bool>(false, "NO"),
                            new Choice<bool>(true, "YES"),
                        },
                        SelectedValue = Settings.Instance.IsVSynced,
                        OnSelect = v =>
                        {
                            Window.Current.IsVSynced = v;
                            Settings.Instance.IsVSynced = v;
                        },
                    },
                    new ChoiceMenuItem<bool>("SHOW FPS")
                    {
                        Choices =
                        {
                            new Choice<bool>(false, "NO"),
                            new Choice<bool>(true, "YES"),
                        },
                        SelectedValue = Settings.Instance.ShowFps,
                        OnSelect = value => Settings.Instance.ShowFps = value,
                    },
                    new CommandMenuItem("BACK", goBack),
                },
            };
        }

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            _menu.Update(terminal);
        }
    }
}
