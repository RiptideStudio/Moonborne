using System;
using System.IO;

namespace Moonborne.Game.Assets
{
    /// <summary>
    /// Base class from which all asset types are derived (prefabs, textures, etc.)
    /// </summary>
    public abstract class Asset
    {
        public string Name;
        public string Path;
        public string Folder;
        public Type AssetType;

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
            Path = "Assets/"+folder +"/"+ name;
        }

        public Asset()
        {
            AssetType = GetType();
        }
    }
}
