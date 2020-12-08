using ExitGames.Client.Photon;

public class PhotonTypeRegister
{
    public static void Register()
    {
        //  Server to client
        PhotonPeer.RegisterType(typeof(SC_EnterRoom), CustomSerializationCode.SC_EnterRoom, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_EntitySkillInfo), CustomSerializationCode.SC_EntitySkillInfo, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_EmotionExpression), CustomSerializationCode.SC_EmotionExpression, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_EntityAppear), CustomSerializationCode.SC_EntityAppear, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_EntityDisAppear), CustomSerializationCode.SC_EntityDisAppear, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_Ping), CustomSerializationCode.SC_Ping, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_SelectableFirstStatusCount), CustomSerializationCode.SC_SelectableFirstStatusCount, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_CharacterStatusChange), CustomSerializationCode.SC_CharacterStatusChange, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_SelectableAbilityInfo), CustomSerializationCode.SC_SelectableAbilityInfo, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_GameEvents), CustomSerializationCode.SC_GameEvents, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_SyncTick), CustomSerializationCode.SC_SyncTick, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(SC_Synchronization), CustomSerializationCode.SC_Synchronization, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);

        //  Client to server
        PhotonPeer.RegisterType(typeof(CS_NotifyMoveInputData), CustomSerializationCode.CS_NotifyMoveInputData, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(CS_NotifyPlayerLookAtPosition), CustomSerializationCode.CS_NotifyPlayerLookAtPosition, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(CS_NotifySkillInputData), CustomSerializationCode.CS_NotifySkillInputData, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(CS_RequestEmotionExpression), CustomSerializationCode.CS_RequestEmotionExpression, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(CS_Ping), CustomSerializationCode.CS_Ping, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(CS_FirstStatusSelection), CustomSerializationCode.CS_FirstStatusSelection, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
        PhotonPeer.RegisterType(typeof(CS_AbilitySelection), CustomSerializationCode.CS_AbilitySelection, Util.CommonDataCompressionSerialize, Util.CommonDataCompressionDeserialize);
    }
}
