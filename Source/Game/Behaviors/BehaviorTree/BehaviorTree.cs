using Force.DeepCloner;
using Moonborn.Game.Behaviors;
using Moonborne.Game.Assets;
using Moonborne.Game.Behavior;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Behaviors
{
    public enum BehaviorStatus
    {
        Success,
        Failure,
        Running
    }

    /// <summary>
    /// Base behavior tree node structure
    /// </summary>
    public abstract class BehaviorTreeNode : Node
    {
        public abstract string Name { get; }

        /// <summary>
        /// Ticks the behavior tree and executes behavior
        /// </summary>
        /// <returns></returns>
        public abstract BehaviorStatus Tick();

        /// <summary>
        /// When behaviors trees executed again they may reset
        /// </summary>
        public virtual void Reset() { }
    }

    /// <summary>
    /// The behavior tree component that can be attached to game objects
    /// </summary>
    public class BehaviorTree : GameBehavior
    {
        internal override string Name => "Behavior Tree";

        public BehaviorTreeNode Root;
        
        /// <summary>
        /// Reference to the asset that defines this behavior tree
        /// </summary>
        public BehaviorTreeAsset Asset { get; set; }

        /// <summary>
        /// Tick the behavior tree
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);
            Tick();
        }

        /// <summary>
        /// Tick the node
        /// </summary>
        /// <returns></returns>
        public BehaviorStatus Tick()
        {
            return Root?.Tick() ?? BehaviorStatus.Failure;
        }
        
        /// <summary>
        /// Initialize the behavior tree with a clone on give
        /// </summary>
        public void Initialize()
        {
            if (Asset == null)
            {
                Root = null;
                return;
            }

            Root = Asset.Tree.Root.DeepClone();
        }

        /// <summary>
        /// Initialize the tree
        /// </summary>
        public override void OnBuild()
        {
            Asset = AssetManager.GetAsset<BehaviorTreeAsset>(Asset.Name);
            Initialize();
        }
    }

    public class BehaviorTreeAsset : Asset
    {
        public BehaviorTree Tree;

        public BehaviorTreeAsset(string name, string folder) : base(name, folder)
        {
            Tree = new BehaviorTree();
            Tree.Root = null; 
        }

        /// <summary>
        /// Open the behavior tree in the editor
        /// </summary>
        public override void OnDoubleClick()
        {
            NodeEditor.Open(this);
        }
    }
}
