using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class PacketMonitor : MonoBehaviour
{
    [SerializeField] private PacketChecker sendPacketChecker;
    [SerializeField] private PacketChecker receivePacketChecker;

    private void Awake()
    {
        SceneMessageBroker.AddSubscriber<CommonMessage.PacketSend>(OnPacketSend);
        SceneMessageBroker.AddSubscriber<CommonMessage.PacketReceive>(OnPacketReceive);
    }

    private void OnDestroy()
    {
        SceneMessageBroker.RemoveSubscriber<CommonMessage.PacketSend>(OnPacketSend);
        SceneMessageBroker.RemoveSubscriber<CommonMessage.PacketReceive>(OnPacketReceive);
    }

    private void OnPacketSend(CommonMessage.PacketSend message)
    {
        sendPacketChecker.packets.Add((message.time, message.packet.GetType()));
    }

    private void OnPacketReceive(CommonMessage.PacketReceive message)
    {
        receivePacketChecker.packets.Add((message.time, message.packet.GetType()));
    }
}
