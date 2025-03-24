using Moonborne.Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Behaviors
{
    /// <summary>
    /// Calls each behavior in order, not exiting when one fails
    /// </summary>
    public class SequenceNode : BehaviorTreeNode
    {
        public override string Name => "Sequencer";

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

                if (status == BehaviorStatus.Running)
                {
                    return BehaviorStatus.Running;
                }

                if (status == BehaviorStatus.Failure)
                {
                    currentIndex = 0; // reset for next evaluation
                    return BehaviorStatus.Failure;
                }

                currentIndex++; // success, move to next child
            }

            currentIndex = 0; // all succeeded
            return BehaviorStatus.Success;
        }

        public override void Reset()
        {
            currentIndex = 0;
            foreach (var child in Children)
                child.Reset();
        }
    }

    /// <summary>
    /// Calls each behavior in order 
    /// </summary>
    public class SelectorNode : BehaviorTreeNode
    {
        public override string Name => "Selector";

        public List<BehaviorTreeNode> Children = new();
        private int currentIndex = 0;

        public SelectorNode() : base()
        {
            BoxColor = Color.DarkOliveGreen;
            OutlineColor = Color.GreenYellow;
        }

        public override void Reset()
        {
            currentIndex = 0;
            foreach (var child in Children)
                child.Reset();
        }

        public override BehaviorStatus Tick()
        {
            while (currentIndex < Children.Count)
            {
                var status = Children[currentIndex].Tick();

                if (status == BehaviorStatus.Running)
                {
                    return BehaviorStatus.Running;
                }

                if (status == BehaviorStatus.Success)
                {
                    currentIndex = 0; // reset for next full evaluation
                    return BehaviorStatus.Success;
                }

                currentIndex++; // try next
            }

            currentIndex = 0; // all failed
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
