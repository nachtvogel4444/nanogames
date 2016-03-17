// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Indicates that a class should not be cloned.
    /// Instances of the class are replaced by null during cloning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class NonClonableAttribute : Attribute
    {
    }
}
