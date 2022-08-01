using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRoomStatusRequest
{
    public string roomId;
    public RoomStatus status;
}

public enum RoomStatus
{
    None = 0,
    Spawning = 1,
    Spawned = 2,
    Ready = 3,
    Playing = 4,
    Finished = 5,
}
