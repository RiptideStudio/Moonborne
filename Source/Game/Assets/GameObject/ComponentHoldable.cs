using Moonborne.Engine.Components;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{
    public interface IComponentContainer
    {
        List<ObjectComponent> Components { get; }
        void AddComponent(ObjectComponent component);
        void RemoveComponent(ObjectComponent component);
        public T GetComponent<T>() where T : ObjectComponent;
    }
}
