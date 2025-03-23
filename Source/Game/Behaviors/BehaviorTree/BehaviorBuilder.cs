using Moonborn.Game.Behaviors;
using Moonborne.Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Behaviors
{
    public class BehaviorTreeBuilder
    {
        private Dictionary<int, BehaviorTreeNode> builtNodes = new();

        public BehaviorTree BuildTree(Node rootNode, List<Node> allNodes, List<Link> links)
        {
            BehaviorTree tree = new();
            tree.Root = BuildNode(rootNode, allNodes, links);
            return tree;
        }

        private BehaviorTreeNode BuildNode(Node node, List<Node> allNodes, List<Link> links)
        {
            if (builtNodes.ContainsKey(node.Id))
                return builtNodes[node.Id];

            BehaviorTreeNode treeNode;

            switch (node.Title)
            {
                case "Action":
                    treeNode = new ActionNode
                    {
                        Id = node.Id,
                        Action = () => {
                            Console.WriteLine($"Action: {node.Id}");
                            return BehaviorStatus.Success; // Or custom logic
                        }
                    };
                    break;

                case "Sequence":
                    var seq = new SequenceNode { Id = node.Id };
                    seq.Children = GetChildren(node, allNodes, links).Select(n => BuildNode(n, allNodes, links)).ToList();
                    treeNode = seq;
                    break;

                case "Selector":
                    var sel = new SelectorNode { Id = node.Id };
                    sel.Children = GetChildren(node, allNodes, links).Select(n => BuildNode(n, allNodes, links)).ToList();
                    treeNode = sel;
                    break;

                default:
                    throw new Exception($"Unknown node type: {node.Title}");
            }

            builtNodes[node.Id] = treeNode;
            return treeNode;
        }

        private List<Node> GetChildren(Node node, List<Node> allNodes, List<Link> links)
        {
            var childLinks = links.Where(link => link.OutputPinId == node.Outputs.First().Id);
            return childLinks
                .Select(link => FindNodeByInputPinId(link.InputPinId, allNodes))
                .Where(n => n != null)
                .ToList();
        }

        private Node FindNodeByInputPinId(int inputPinId, List<Node> allNodes)
        {
            return allNodes.FirstOrDefault(n => n.Inputs.Any(p => p.Id == inputPinId));
        }
    }

}
