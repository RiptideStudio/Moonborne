﻿using System;
using System.IO;

namespace Moonborne.Game.Assets
{
    /// <summary>
    /// Base class from which all asset types are derived (prefabs, textures, etc.)
    /// </summary>
    public abstract class Asset
    {
        public string Name;
        public string Folder;
        internal bool IsDraggable = false;
        internal Type AssetType;

        /// <summary>
        /// Construct a new asset given a filename and path
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="folder"></param>
        public Asset(string name, string folder)
        {
            Name = name;
            Folder = folder;
            AssetType = typeof(Asset);
        }

        /// <summary>
        /// For editable lists
        /// </summary>
        public Asset()
        {

        }

        /// <summary>
        /// Called after an asset is created
        /// </summary>
        public virtual void PostLoad()
        {

        }
    }
}
