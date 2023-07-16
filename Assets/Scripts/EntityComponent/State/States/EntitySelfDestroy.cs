
namespace State
{
    public class EntitySelfDestroy : StateBase
    {
        #region StateBase
        protected override void OnInitialize(StateParam stateParam)
        {
            var basicStateParam = stateParam as BasicStateParam;

            Lifespan = basicStateParam.lifespan;
        }

        protected override bool OnStateUpdate()
        {
            if (Lifespan == -1)
            {
                return true;
            }

            if (CurrentUpdateTime >= Lifespan)
            {
                LOP.Game.Current.DestroyEntity(Entity.EntityId);
            }

            return CurrentUpdateTime < Lifespan;
        }
        #endregion
    }
}
