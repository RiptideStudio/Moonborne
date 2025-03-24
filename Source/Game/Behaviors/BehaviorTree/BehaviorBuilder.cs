using Moonborn.Game.Behaviors;
using Moonborne.Game.Behaviors;
using Moonborne.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Behaviors
{
    /// <summary>
    /// Keep track of all of the behavior nodes we've made
    /// </summary>
    public class BehaviorNodeLibrary
    {
        public static Dictionary<string, Type> Nodes = new Dictionary<string, Type>();
        public static void Initialize()
        {
            // Find all behavior tree nodes and create a dictionary of them
            Nodes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(BehaviorTreeNode))
                               && !type.IsAbstract)
                .ToDictionary(type => type.Name, type => type);
        }
    }
}
