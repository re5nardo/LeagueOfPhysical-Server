using System.Collections;
using System.Collections.Generic;
using System;

public class MatchEndRequest
{
    public string matchId;
    public MatchSetting matchSetting;
    public List<string> playerIds;
    public List<int> winnerPlayerIds;
    public List<int> loserPlayerIds;
    public List<RankingData> rankingDataList;

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
