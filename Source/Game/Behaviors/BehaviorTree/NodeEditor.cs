using ImGuiNET;
using Moonborn.Game.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class NodeEditor
{
    private List<Node> nodes = new();
    private List<Link> links = new();
    private Pin activeOutputPin = null;
    private int nextNodeId = 1;
    private int nextPinId = 100;
    private Node activeDraggedNode = null;
    private Vector2 panOffset = Vector2.Zero;
    private float zoom = 1.0f;

    public void Draw()
    {
   

        ImGui.Begin("Behavior Tree Editor");

        // Draw right-click menu for adding nodes
        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            ImGui.OpenPopup("NodeContext");

        if (ImGui.BeginPopup("NodeContext"))
        {
            if (ImGui.MenuItem("Add Action Node"))
            {
                AddActionNode(ImGui.GetMousePos());
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

        // Draw nodes and handle interaction
        foreach (var node in nodes)
        {
            node.DrawNode();

            foreach (var output in node.Outputs)
            {
                Vector2 size = new Vector2(150, 60);
                if (activeDraggedNode == null && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && Node.MouseOverNode(node.Position, size))
                {
                    activeDraggedNode = node;
                }
                if (activeDraggedNode == node)
                {
                    node.Position = ImGui.GetMousePos()-(ImGui.GetMousePos()-node.Position);
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

    private void AddActionNode(Vector2 position)
    {
        var node = new Node
        {
            Id = nextNodeId++,
            Title = "Action",
            Position = position,
            Inputs = new() {
                new Pin { Id = nextPinId++, Name = "In", Offset = new Vector2(0, 30) }
            },
            Outputs = new() {
                new Pin { Id = nextPinId++, Name = "Out", Offset = new Vector2(140, 30) }
            }
        };

        nodes.Add(node);
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
}
