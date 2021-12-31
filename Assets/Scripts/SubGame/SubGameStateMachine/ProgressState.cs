using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

namespace SubGameState
{
    public class ProgressState : MonoStateBase
    {
        public override void OnEnter()
        {
            SubGameBase.Current.StartGame();
        }

        public override IEnumerator OnExecute()
        {
            yield return new WaitUntil(() => SubGameBase.Current.IsGameEnd);

            FSM.MoveNext(SubGameStateInput.StateDone);
        }

        public override IState GetNext<I>(I input)
        {
            if (!Enum.TryParse(input.ToString(), out SubGameStateInput subGameStateInput))
            {
                Debug.LogError($"Invalid input! input : {input}");
                return default;
            }

            switch (subGameStateInput)
            {
                case SubGameStateInput.StateDone: return gameObject.GetOrAddComponent<SubGameState.ClearState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {subGameStateInput}");
        }
    }
}
