using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace NetworkModel.Mirror
{
    public static class CustomMirrorMessageSerializer
    {
        public static void WriteCustomMirrorMessage(this NetworkWriter writer, CustomMirrorMessage value)
        {
            writer.WriteByte(value.id);
            writer.WriteBytesAndSize(Util.CommonDataCompressionSerialize(value.payload));
        }

        public static CustomMirrorMessage ReadCustomMirrorMessage(this NetworkReader reader)
        {
            byte id = reader.ReadByte();
            byte[] data = reader.ReadBytesAndSize();
            var payload = GetMirrorMessage(id, data);
            
            return new CustomMirrorMessage
            {
                id = id,
                payload = payload,
            };
        }

        public static IMirrorMessage GetMirrorMessage(byte id, byte[] data)
        {
            switch (id)
            {
                case MessageIds.SC_EnterRoom: return Util.CommonDataCompressionDeserialize(data) as SC_EnterRoom;
                case MessageIds.SC_ProcessInputData: return Util.CommonDataCompressionDeserialize(data) as SC_ProcessInputData;
                case MessageIds.SC_EntitySkillInfo: return Util.CommonDataCompressionDeserialize(data) as SC_EntitySkillInfo;
                case MessageIds.SC_EmotionExpression: return Util.CommonDataCompressionDeserialize(data) as SC_EmotionExpression;
                case MessageIds.SC_EntityAppear: return Util.CommonDataCompressionDeserialize(data) as SC_EntityAppear;
                case MessageIds.SC_EntityDisAppear: return Util.CommonDataCompressionDeserialize(data) as SC_EntityDisAppear;
                case MessageIds.SC_GameEvents: return Util.CommonDataCompressionDeserialize(data) as SC_GameEvents;
                case MessageIds.SC_Synchronization: return Util.CommonDataCompressionDeserialize(data) as SC_Synchronization;
                case MessageIds.SC_GameEnd: return Util.CommonDataCompressionDeserialize(data) as SC_GameEnd;
                case MessageIds.SC_OwnerChanged: return Util.CommonDataCompressionDeserialize(data) as SC_OwnerChanged;
                case MessageIds.SC_SyncController: return Util.CommonDataCompressionDeserialize(data) as SC_SyncController;
                case MessageIds.SC_SubGameReadyNotice: return Util.CommonDataCompressionDeserialize(data) as SC_SubGameReadyNotice;
                case MessageIds.SC_PlayerEntity: return Util.CommonDataCompressionDeserialize(data) as SC_PlayerEntity;

                case MessageIds.CS_NotifyMoveInputData: return Util.CommonDataCompressionDeserialize(data) as CS_NotifyMoveInputData;
                case MessageIds.CS_NotifySkillInputData: return Util.CommonDataCompressionDeserialize(data) as CS_NotifySkillInputData;
                case MessageIds.CS_NotifyJumpInputData: return Util.CommonDataCompressionDeserialize(data) as CS_NotifyJumpInputData;
                case MessageIds.CS_RequestEmotionExpression: return Util.CommonDataCompressionDeserialize(data) as CS_RequestEmotionExpression;
                case MessageIds.CS_GamePreparation: return Util.CommonDataCompressionDeserialize(data) as CS_GamePreparation;
                case MessageIds.CS_SubGamePreparation: return Util.CommonDataCompressionDeserialize(data) as CS_SubGamePreparation;
                case MessageIds.CS_Synchronization: return Util.CommonDataCompressionDeserialize(data) as CS_Synchronization;
                case MessageIds.CS_SyncController: return Util.CommonDataCompressionDeserialize(data) as CS_SyncController;
            }

            Debug.LogError($"The id is invalid! id: {id}");
            return default;
        }
    }
}
