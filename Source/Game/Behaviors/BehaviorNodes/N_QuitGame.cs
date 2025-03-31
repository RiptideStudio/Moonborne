
using Moonborne.Engine;

namespace Moonborne.Game.Behaviors
{
    public class N_QuitGame : ActionNode
    {
        public override string Name => "Quit Game";

        public override BehaviorStatus Tick()
        {
            GameManager.Exit();
            return BehaviorStatus.Success;
        }
    }
}
