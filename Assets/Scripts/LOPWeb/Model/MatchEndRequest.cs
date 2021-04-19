using System.Collections;
using System.Collections.Generic;
using System;

public class MatchEndRequest
{
    public string matchId;
    public string matchType;
    public string subGameId;
    public string mapId;
    public List<string> playerIds = new List<string>();
    public List<int> winnerPlayerIds = new List<int>();
    public List<int> loserPlayerIds = new List<int>();
    public List<RankingData> rankingDataList = new List<RankingData>();

    [Serializable]
    public struct RankingData
    {
        public string playerId;
        public int ranking;

        public RankingData(string playerId, int ranking)
        {
            this.playerId = playerId;
            this.ranking = ranking;
        }
    }
}
