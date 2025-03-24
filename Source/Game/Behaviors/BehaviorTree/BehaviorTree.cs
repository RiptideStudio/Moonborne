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

    public abstract class BehaviorTreeNode : Node
    {
        public abstract string Name { get; }
        public abstract BehaviorStatus Tick();
    }

    /// <summary>
    /// The component
    /// </summary>
    public class BehaviorTree : GameBehavior
    {
        internal override string Name => "Behavior Tree";

        public BehaviorTreeNode Root;

        public BehaviorStatus Tick()
        {
            return Root?.Tick() ?? BehaviorStatus.Failure;
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
    }

    public class SequenceNode : BehaviorTreeNode
    {
        public override string Name => "Sequence Node";

        public List<BehaviorTreeNode> Children = new();
        private int currentIndex = 0;

        public SequenceNode() : base()
        {
            BoxColor = Color.DarkSlateGray;
            OutlineColor = Color.SlateGray;
        }

        public override BehaviorStatus Tick()
        {
            while (currentIndex < Children.Count)
            {
                var status = Children[currentIndex].Tick();
                if (status == BehaviorStatus.Running) return BehaviorStatus.Running;
                if (status == BehaviorStatus.Failure)
                {
                    currentIndex = 0;
                    return BehaviorStatus.Failure;
                }
                currentIndex++;
            }

            currentIndex = 0;
            return BehaviorStatus.Success;
        }
    }

    public class SelectorNode : BehaviorTreeNode
    {
        public override string Name => "Selector Node";

        public List<BehaviorTreeNode> Children = new();
        private int currentIndex = 0;

        public SelectorNode() : base()
        {
            BoxColor = Color.DarkSlateGray;
            OutlineColor = Color.SlateGray;
        }

        public override BehaviorStatus Tick()
        {
            while (currentIndex < Children.Count)
            {
                var status = Children[currentIndex].Tick();
                if (status == BehaviorStatus.Running) return BehaviorStatus.Running;
                if (status == BehaviorStatus.Success)
                {
                    currentIndex = 0;
                    return BehaviorStatus.Success;
                }
                currentIndex++;
            }

            currentIndex = 0;
            return BehaviorStatus.Failure;
        }
    }

    public class ActionNode : BehaviorTreeNode
    {
        public override string Name => "Action Node";
        public Func<BehaviorStatus> Action;

        public ActionNode() : base()
        {
            BoxColor = Color.DarkRed;
            OutlineColor = Color.IndianRed;
        }

        public override BehaviorStatus Tick()
        {
            return Action?.Invoke() ?? BehaviorStatus.Failure;
        }
    }
}
