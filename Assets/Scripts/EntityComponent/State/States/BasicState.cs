
namespace State
{
    public class BasicState : StateBase
    {
        private float lifespan;

        #region StateBase
        protected override bool OnStateUpdate()
        {
            if (lifespan == -1)
            {
                return true;
            }

            return CurrentUpdateTime < lifespan;
        }

        public override void Initialize(StateParam stateParam)
        {
            base.Initialize(stateParam);

            var basicStateParam = stateParam as BasicStateParam;

            lifespan = basicStateParam.lifespan;
        }
        #endregion
    }
}
