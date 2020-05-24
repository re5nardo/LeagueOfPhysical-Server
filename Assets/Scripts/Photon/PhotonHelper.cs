using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class PhotonHelper
{
    public static int[] GetActorIDs(Vector3 vec3Position, float fRadius)
    {
        List<int> actorIDs = new List<int>();
        foreach (IEntity entity in EntityManager.Instance.GetEntities(vec3Position, fRadius, EntityRole.Player))
        {
            
            if (LOP.Room.Instance.dicEntityIDPlayerUserID.ContainsKey(entity.EntityID))
            {
                string strPlayerUserID = LOP.Room.Instance.dicEntityIDPlayerUserID[entity.EntityID];
                if (LOP.Room.Instance.dicPlayerUserIDPhotonPlayer.ContainsKey(strPlayerUserID))
                {
                    var player = LOP.Room.Instance.dicPlayerUserIDPhotonPlayer[strPlayerUserID];
                    if (player.IsAlive)
                    {
                        int actorID = (player.Target as PhotonPlayer).ID;
                        actorIDs.Add(actorID);
                    }
                }
            }
        }

        return actorIDs.ToArray();
    }

    public static int GetActorID(int nEntityID)
    {
        string strPlayerUserID = "";
        if (LOP.Room.Instance.dicEntityIDPlayerUserID.TryGetValue(nEntityID, out strPlayerUserID))
        {
            WeakReference target = null;
            if (LOP.Room.Instance.dicPlayerUserIDPhotonPlayer.TryGetValue(strPlayerUserID, out target))
            {
                if (target.IsAlive)
                {
                    return (target.Target as PhotonPlayer).ID;
                }
            }
        }

        return -1;
    }

    public static PhotonPlayer GetPhotonPlayer(int nEntityID)
    {
        return LOP.Room.Instance.dicPlayerUserIDPhotonPlayer[LOP.Room.Instance.dicEntityIDPlayerUserID[nEntityID]].Target as PhotonPlayer;
    }
}
