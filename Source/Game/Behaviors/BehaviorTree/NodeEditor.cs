using ImGuiNET;
using Moonborn.Game.Behaviors;
using Moonborne.Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;

public class NodeEditor
{
    private List<Node> nodes = new();
    private List<Link> links = new();
    private Pin activeOutputPin = null;
    private int nextNodeId = 1;
    private int nextPinId = 100;
    private Node activeDraggedNode = null;
    public bool Active = false;
    public static NodeEditor Instance;
    private BehaviorTreeAsset currentAsset;

    /// <summary>
    /// Open a behavior tree asset
    /// </summary>
    /// <param name="behaviorTree"></param>
    public static void Open(BehaviorTreeAsset behaviorTree)
    {
        Instance.Active = !Instance.Active;
        Instance.currentAsset = behaviorTree;
        Instance.LoadFromBehaviorTree(behaviorTree);
    }

    public void LoadFromBehaviorTree(BehaviorTreeAsset asset)
    {
        nodes.Clear();
        links.Clear();
        nextNodeId = 1;
        nextPinId = 100;

        if (asset.Tree?.Root == null)
            return;

        Dictionary<BehaviorTreeNode, int> nodeToId = new();
        Queue<BehaviorTreeNode> queue = new();
        queue.Enqueue(asset.Tree.Root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (nodeToId.ContainsKey(node)) continue;

            node.Id = nextNodeId++;
            node.Title = node.Name;
            node.Inputs = new() {
            new Pin { Id = nextPinId++, Name = "In", Offset = new Vector2(0, 30) }
            };
            node.Outputs = new() {
            new Pin { Id = nextPinId++, Name = "Out", Offset = new Vector2(140, 30) }
            };

            nodes.Add(node);
            nodeToId[node] = node.Id;

            // Handle children
            if (node is SequenceNode seq)
            {
                foreach (var child in seq.Children)
                {
                    queue.Enqueue(child);
                    links.Add(new Link
                    {
                        OutputPinId = node.Outputs[0].Id,
                        InputPinId = child.Inputs[0].Id
                    });
                }
            }
            else if (node is SelectorNode sel)
            {
                foreach (var child in sel.Children)
                {
                    queue.Enqueue(child);
                    links.Add(new Link
                    {
                        OutputPinId = node.Outputs[0].Id,
                        InputPinId = child.Inputs[0].Id
                    });
                }
            }
        }
    }

    private void UpdateAsset()
    {
        if (currentAsset == null)
            return;

        currentAsset.Tree = BuildBehaviorTree(); // builds and sets .Root too
    }


    public void Draw()
    {
        if (!Active)
            return;

        ImGui.Begin("Behavior Tree Editor");

        // Draw right-click menu for adding nodes
        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            ImGui.OpenPopup("NodeContext");

        if (ImGui.BeginPopup("NodeContext"))
        {
            if (ImGui.BeginMenu("Create New Node"))
            {
                foreach (var node in BehaviorNodeLibrary.Nodes)
                {
                    if (ImGui.MenuItem($"{node.Key}"))
                    {
                        AddNode(node.Key, ImGui.GetMousePos());
                        UpdateAsset();
                    }
                }
                ImGui.EndMenu();
            }

            ImGui.EndPopup();
        }


        // Draw links
        foreach (var link in links)
        {
            var fromPin = FindPinById(link.OutputPinId, out Node fromNode);
            var toPin = FindPinById(link.InputPinId, out Node toNode);

            if (fromPin != null && toPin != null)
            {
                var from = Pin.GetPinWorldPosition(fromNode, fromPin);
                var to = Pin.GetPinWorldPosition(toNode, toPin);
                Node.DrawLink(from, to);
            }
        }

        // Drag to move
        if (activeDraggedNode != null)
        {
            activeDraggedNode.Position += ImGui.GetIO().MouseDelta;
        }

        // Draw nodes and handle interaction
        foreach (var node in nodes)
        {
            // Right-click node to open context menu
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && Node.MouseOverNode(node.Position, node.Size))
            {
                ImGui.OpenPopup($"NodeContext_{node.Id}");
            }

            if (ImGui.BeginPopup($"NodeContext_{node.Id}"))
            {
                if (ImGui.MenuItem("Delete Node"))
                {
                    // Remove links connected to this node
                    links.RemoveAll(link =>
                        node.Inputs.Any(p => p.Id == link.InputPinId) ||
                        node.Outputs.Any(p => p.Id == link.OutputPinId)
                    );

                    nodes.Remove(node);
                    UpdateAsset();
                    ImGui.EndPopup();
                    break; 
                }
                ImGui.EndPopup();
            }

            node.DrawNode();

            foreach (var output in node.Outputs)
            {
                if (HandlePinSelection(node, output))
                    break;
                Vector2 size = new Vector2(150, 60);
                if (activeDraggedNode == null && activeOutputPin == null && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && Node.MouseOverNode(node.Position, node.Size))
                {
                    activeDraggedNode = node;
                }
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    activeDraggedNode = null;
                }
                var pos = Pin.GetPinWorldPosition(node, output);
                if (Pin.IsMouseOverPin(pos) && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    activeOutputPin = output;
                }
            }

            foreach (var input in node.Inputs)
            {
                if (HandlePinSelection(node, input))
                    break;
                var pos = Pin.GetPinWorldPosition(node, input);
                if (activeOutputPin != null && Pin.IsMouseOverPin(pos) && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    // Create a new link
                    links.Add(new Link
                    {
                        OutputPinId = activeOutputPin.Id,
                        InputPinId = input.Id
                    });
                    activeOutputPin = null;
                }
            }
        }

        // Draw dragging link preview
        if (activeOutputPin != null)
        {
            var fromNode = nodes.First(n => n.Outputs.Contains(activeOutputPin));
            var from = Pin.GetPinWorldPosition(fromNode, activeOutputPin);
            var to = ImGui.GetIO().MousePos;
            Node.DrawLink(from, to);
        }

        ImGui.End();
    }

    private bool HandlePinSelection(Node node, Pin pin)
    {
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && Pin.IsMouseOverPin(pin.ScreenPosition))
        {
            ImGui.OpenPopup($"PinContext_{node.Id}_{pin.Id}");
        }

        if (ImGui.BeginPopup($"PinContext_{node.Id}_{pin.Id}"))
        {
            if (ImGui.MenuItem("Delete Links"))
            {
                links.RemoveAll(link => link.InputPinId == pin.Id || link.OutputPinId == pin.Id);
                UpdateAsset();
                return true;
            }
            ImGui.EndPopup();
        }

        return false;
    }

    /// <summary>
    /// Adds a node given the type
    /// </summary>
    /// <param name="node"></param>
    /// <param name="position"></param>
    private void AddNode(string node, Vector2 position)
    {
        Type nodeType = BehaviorNodeLibrary.Nodes.GetValueOrDefault(node);
        BehaviorTreeNode newNode = (BehaviorTreeNode)Activator.CreateInstance(nodeType);

        newNode.Id = nextNodeId++;
        newNode.Position = position;
        newNode.Title = newNode.Name;
        newNode.Inputs = new() {
            new Pin { Id = nextPinId++, Name = "In", Offset = new Vector2(0, 30) }
        };
        newNode.Outputs = new() {
            new Pin { Id = nextPinId++, Name = "Out", Offset = new Vector2(140, 30) }
        };

        nodes.Add(newNode);
    }

    private Pin FindPinById(int id, out Node owningNode)
    {
        foreach (var node in nodes)
        {
            foreach (var pin in node.Inputs)
                if (pin.Id == id) { owningNode = node; return pin; }

            foreach (var pin in node.Outputs)
                if (pin.Id == id) { owningNode = node; return pin; }
        }
        owningNode = null;
        return null;
    }

    private Node FindRootNode()
    {
        var allInputPinIds = new HashSet<int>(links.Select(l => l.InputPinId));
        return nodes.FirstOrDefault(n => n.Inputs.Any(p => !allInputPinIds.Contains(p.Id)));
    }

    public BehaviorTree BuildBehaviorTree()
    {
        var tree = new BehaviorTree();

        var rootNode = FindRootNode();
        if (rootNode == null)
        {
            Console.WriteLine("No root node found!");
            return tree;
        }

        tree.Root = (BehaviorTreeNode)rootNode;

        var pinToNode = nodes
            .SelectMany(n => n.Outputs.Concat(n.Inputs), (n, pin) => (pin, node: n))
            .ToDictionary(x => x.pin.Id, x => x.node);

        foreach (var link in links)
        {
            if (!pinToNode.TryGetValue(link.OutputPinId, out var parent) ||
                !pinToNode.TryGetValue(link.InputPinId, out var child))
            {
                continue;
            }

            if (parent is SequenceNode seq)
                seq.Children.Add((BehaviorTreeNode)child);
            else if (parent is SelectorNode sel)
                sel.Children.Add((BehaviorTreeNode)child);
        }

        return tree;
    }
}
