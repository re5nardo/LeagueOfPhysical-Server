using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameProcedureBase : MonoBehaviour
{
    public abstract IEnumerator Procedure();
}
