using Microsoft.Xna;
using Moonborne.Game.Objects;
using System;

namespace Moonborne.Game.Components
{
    /// <summary>
    /// Enum to define component types
    /// </summary>
    public enum ComponentType
    {
        Transform,
        None
    }

    /// <summary>
    /// Base component class
    /// </summary>
    public abstract class Component
    {
        public GameObject Parent { get; private set; }
        public abstract string Name { get; }
        public abstract ComponentType Type { get; }

    }
}