using Moonborne.Game.Assets;
using Moonborne.Game.Behavior;
using System;
using System.Collections.Generic;
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

    public abstract class BehaviorTreeNode
    {
        public int Id;
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
        public List<BehaviorTreeNode> Children = new();
        private int currentIndex = 0;

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
        public List<BehaviorTreeNode> Children = new();
        private int currentIndex = 0;

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
        public Func<BehaviorStatus> Action;

        public override BehaviorStatus Tick()
        {
            return Action?.Invoke() ?? BehaviorStatus.Failure;
        }
    }
}
