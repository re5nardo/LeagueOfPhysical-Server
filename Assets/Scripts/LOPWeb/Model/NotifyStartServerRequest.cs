using System.Collections;
using System.Collections.Generic;
using System;

public class NotifyStartServerRequest
{
    public string roomId;
    public string matchId;
    public string[] expectedPlayerList;
    public MatchSetting matchSetting;
    public string ip;
    public int port;
}
