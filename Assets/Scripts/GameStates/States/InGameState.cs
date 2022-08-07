using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{

    public class InGameState : Core.FSMBaseState<GStateManager.eState>
    {

        public InGameState(Core.FSMStateMachine<GStateManager.eState> machine) : base(machine) { }

        public override void OnEnter(System.Object context)
        {
            base.OnEnter(context);
            ExtractContext(context);

            session = new Session(sharedObjects);

            Core.Updater.Instance.FixedUpdater += FixedUpdate;
            Core.Updater.Instance.LateUpdater += LateUpdate;
        }

        public void FixedUpdate()
        {
            if (session != null)
                session.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();
            if (session != null)
                session.Update();
        }

        public void LateUpdate()
        {
            if (session != null)
                session.LateUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
            Core.Updater.Instance.FixedUpdater -= FixedUpdate;
            Core.Updater.Instance.LateUpdater -= LateUpdate;
        }

        private void ExtractContext(object context)
        {
            Core.QLogger.Assert(context != null && context is Core.SharedObjects<System.Object>);
            sharedObjects = context as Core.SharedObjects<System.Object>;
        }

        private Core.SharedObjects<System.Object> sharedObjects = null;
        private Session session = null;
    }
}