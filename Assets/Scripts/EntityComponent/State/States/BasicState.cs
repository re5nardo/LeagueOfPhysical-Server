
namespace State
{
    public class BasicState : StateBase
    {
        private BasicStateParam param;

        #region StateBase
        protected override bool OnStateUpdate()
        {
            if (Lifespan == -1)
            {
                return true;
            }

            return CurrentUpdateTime < Lifespan;
        }

        public override void Initialize(StateParam stateParam)
        {
            base.Initialize(stateParam);

            param = stateParam as BasicStateParam;
            Lifespan = param.lifespan;
        }

        public override void OnAccumulate(StateParam stateParam)
        {
            param = stateParam as BasicStateParam;

            Lifespan = param.lifespan == -1 ? -1 : Lifespan + param.lifespan;
        }
        #endregion
    }
}
