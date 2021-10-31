
namespace State
{
    public class EntitySelfDestroy : StateBase
    {
        #region StateBase
        protected override bool OnStateUpdate()
        {
            if (Lifespan == -1)
            {
                return true;
            }

            if (CurrentUpdateTime >= Lifespan)
            {
                LOP.Game.Current.DestroyEntity(Entity.EntityID);
            }

            return CurrentUpdateTime < Lifespan;
        }

        public override void Initialize(StateParam stateParam)
        {
            base.Initialize(stateParam);

            var basicStateParam = stateParam as BasicStateParam;

            Lifespan = basicStateParam.lifespan;
        }
        #endregion
    }
}
