
namespace State
{
    public class EntitySelfDestroy : StateBase
    {
        private float lifespan;

        #region StateBase
        protected override bool OnStateUpdate()
        {
            if (lifespan == -1)
            {
                return true;
            }

            if (CurrentUpdateTime >= lifespan)
            {
                LOP.Game.Current.DestroyEntity(Entity.EntityID);
            }

            return CurrentUpdateTime < lifespan;
        }

        public override void SetData(int nStateMasterID, params object[] param)
        {
            base.SetData(nStateMasterID, param);

            if (param.Length > 0)
            {
                lifespan = (float)param[0];
            }
            else
            {
                lifespan = MasterData.Lifespan;
            }
        }
        #endregion
    }
}
