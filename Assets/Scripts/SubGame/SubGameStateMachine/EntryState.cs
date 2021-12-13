using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

namespace SubGameState
{
    public class EntryState : MonoStateBase
    {
        public override void Enter()
        {
            base.Enter();

            StopCoroutine("Procedure");
            StartCoroutine("Procedure");
        }

        public override void Exit()
        {
            base.Exit();

            StopCoroutine("Procedure");
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
                case SubGameStateInput.StateDone:
                    return gameObject.GetOrAddComponent<SubGameState.PrepareState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {subGameStateInput}");
        }

        private IEnumerator Procedure()
        {
            yield return SubGameBase.Current.Initialize();

            FSM.MoveNext(GameStateInput.StateDone);
        }
    }
}