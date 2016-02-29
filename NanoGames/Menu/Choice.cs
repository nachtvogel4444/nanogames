// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Menu
{
    /// <summary>
    /// A choice in a <see cref="ChoiceMenuItem{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the choice value.</typeparam>
    internal sealed class Choice<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Choice{T}"/> class.
        /// </summary>
        /// <param name="value">The choice value.</param>
        /// <param name="text">The choice text.</param>
        public Choice(T value, string text)
        {
            Value = value;
            Text = text;
        }

        /// <summary>
        /// Gets or sets the choice value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the choice text.
        /// </summary>
        public string Text { get; set; }
    }
}
