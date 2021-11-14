
namespace State
{
    public class BasicState : StateBase
    {
        private BasicStateParam param;

        #region StateBase
        protected override void OnInitialize(StateParam stateParam)
        {
            param = stateParam as BasicStateParam;
            Lifespan = param.lifespan;
        }

        protected override void OnAccumulate(StateParam stateParam)
        {
            param = stateParam as BasicStateParam;
            Lifespan = param.lifespan == -1 ? -1 : Lifespan + param.lifespan;
        }

        protected override bool OnStateUpdate()
        {
            if (Lifespan == -1)
            {
                return true;
            }

            return CurrentUpdateTime < Lifespan;
        }
        #endregion
    }
}
