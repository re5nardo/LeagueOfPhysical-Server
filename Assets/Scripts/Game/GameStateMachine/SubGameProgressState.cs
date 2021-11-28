﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

namespace GameState
{
    public class SubGameProgressState : MonoStateBase
    {
        public override void Enter()
        {
            base.Enter();

            SubGameBase.Current.StartGame();
        }

        public override void Execute()
        {
            base.Execute();

            if (SubGameBase.Current.IsGameEnd)
            {
                FSM.MoveNext(GameStateInput.StateDone);
            }
        }

        public override IState GetNext<I>(I input)
        {
            if (!Enum.TryParse(input.ToString(), out GameStateInput gameStateInput))
            {
                Debug.LogError($"Invalid input! input : {input}");
                return default;
            }

            switch (gameStateInput)
            {
                case GameStateInput.StateDone:
                    return gameObject.GetOrAddComponent<GameState.SubGameClearState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }
    }
}
