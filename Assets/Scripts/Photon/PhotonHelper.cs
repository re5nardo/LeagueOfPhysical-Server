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
            
            if (LOP.Game.Current.EntityIDPlayerUserID.ContainsKey(entity.EntityID))
            {
                string strPlayerUserID = LOP.Game.Current.EntityIDPlayerUserID[entity.EntityID];
                if (LOP.Game.Current.PlayerUserIDPhotonPlayer.ContainsKey(strPlayerUserID))
                {
                    var player = LOP.Game.Current.PlayerUserIDPhotonPlayer[strPlayerUserID];
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
        if (LOP.Game.Current.EntityIDPlayerUserID.TryGetValue(nEntityID, out strPlayerUserID))
        {
            WeakReference target = null;
            if (LOP.Game.Current.PlayerUserIDPhotonPlayer.TryGetValue(strPlayerUserID, out target))
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
        return LOP.Game.Current.PlayerUserIDPhotonPlayer[LOP.Game.Current.EntityIDPlayerUserID[nEntityID]].Target as PhotonPlayer;
    }
}
