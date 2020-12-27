using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOP
{
    public class GameManager : MonoBehaviour
    {
        public bool IsMatchEnd { get; private set; } = false;

        private List<GameProcedureBase> gameProcedures = new List<GameProcedureBase>();

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            gameProcedures.Add(gameObject.AddComponent<SubGameCycle>());
        }

        public void StartGameManager()
        {
            StartCoroutine(GameManagerLoop());
        }
        
        private IEnumerator GameManagerLoop()
        {
            IsMatchEnd = false;

            foreach (var gameProcedure in gameProcedures)
            {
                yield return gameProcedure.Procedure();
            }

            IsMatchEnd = false;
        }
    }
}
