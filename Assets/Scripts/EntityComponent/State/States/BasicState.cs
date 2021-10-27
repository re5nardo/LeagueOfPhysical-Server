
namespace State
{
    public class BasicState : StateBase
    {
        private BasicStateParam param;

        #region StateBase
        protected override bool OnStateUpdate()
        {
            if (param.lifespan == -1)
            {
                return true;
            }

            return CurrentUpdateTime < param.lifespan;
        }

        public override void Initialize(StateParam stateParam)
        {
            base.Initialize(stateParam);

            param = stateParam as BasicStateParam;
        }
        #endregion
    }
}
