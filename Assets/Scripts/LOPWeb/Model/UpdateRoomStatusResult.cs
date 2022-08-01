using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class UpdateRoomStatusResult : HttpResultBase
{
    public NetworkModel.Room room;
}

namespace NetworkModel
{
    [Serializable]
    public class Room
    {
        public string id;
        public string matchId;
        public MatchType matchType;
        public string subGameId;
        public string mapId;
        public RoomStatus status;
        public string ip;
        public int port;
    }
}
