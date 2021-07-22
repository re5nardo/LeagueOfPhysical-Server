using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

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
        if (LOP.Application.IsApplicationQuitting)
        {
            return;
        }

        var gameEvents = new SC_GameEvents(new List<IGameEvent> { gameEvent });
        RoomNetwork.Instance.Send(gameEvents, nTargetID, bReliable, bInstant);
    }

    public void SendToAll(IGameEvent gameEvent, bool bReliable = true, bool bInstant = false)
    {
        if (LOP.Application.IsApplicationQuitting)
        {
            return;
        }

        var gameEvents = new SC_GameEvents(new List<IGameEvent> { gameEvent });
        RoomNetwork.Instance.SendToAll(gameEvents, bReliable, bInstant);
    }

    public void SendToNear(IGameEvent gameEvent, Vector3 center, float fRadius = LOP.Game.BROADCAST_SCOPE_RADIUS, bool bReliable = true, bool bInstant = false)
    {
        if (LOP.Application.IsApplicationQuitting)
        {
            return;
        }

        var gameEvents = new SC_GameEvents(new List<IGameEvent> { gameEvent });
        RoomNetwork.Instance.SendToNear(gameEvents, center, fRadius, bReliable, bInstant);
    }
}
