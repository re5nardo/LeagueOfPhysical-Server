
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

        public override void SetData(int nStateMasterID, params object[] param)
        {
            base.SetData(nStateMasterID, param);

            if (param.Length > 0)
            {
                lifespan = (float)param[0];
            }
            else
            {
                lifespan = m_MasterData.Lifespan;
            }
        }
        #endregion
    }
}
