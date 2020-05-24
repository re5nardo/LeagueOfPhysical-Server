using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class GameEventManager : MonoBehaviour
{
    private int seq = 0;

    //public void Clear()
    //{
    //    seq = 0;
    //}

    public int GenerateSeq()
    {
        return seq++;
    }

    public void Send(IGameEvent gameEvent, int nTargetID, bool bReliable = true, bool bInstant = false)
    {
        //  OnDestroy 타이밍에 호출될 수 있는 곳이기 때문에 Check..좀 더 좋은 방법 없나.. ?
        if (RoomNetwork.IsInstantiated() && LOP.Room.IsInstantiated() && EntityManager.IsInstantiated())
        {
            SC_GameEvents gameEvents = new SC_GameEvents(new List<IGameEvent> { gameEvent });
            RoomNetwork.Instance.Send(gameEvents, nTargetID, bReliable, bInstant);
        }
    }

    public void SendToAll(IGameEvent gameEvent, bool bReliable = true, bool bInstant = false)
    {
        if (RoomNetwork.IsInstantiated() && LOP.Room.IsInstantiated() && EntityManager.IsInstantiated())
        {
            SC_GameEvents gameEvents = new SC_GameEvents(new List<IGameEvent> { gameEvent });
            RoomNetwork.Instance.SendToAll(gameEvents, bReliable, bInstant);
        }
    }

    public void SendToNear(IGameEvent gameEvent, Vector3 center, float fRadius = LOP.Game.BROADCAST_SCOPE_RADIUS, bool bReliable = true, bool bInstant = false)
    {
        if (RoomNetwork.IsInstantiated() && LOP.Room.IsInstantiated() && EntityManager.IsInstantiated())
        {
            SC_GameEvents gameEvents = new SC_GameEvents(new List<IGameEvent> { gameEvent });
            RoomNetwork.Instance.SendToNear(gameEvents, center, fRadius, bReliable, bInstant);
        }
    }
}
