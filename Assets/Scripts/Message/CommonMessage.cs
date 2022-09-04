using UnityEngine;
using GameFramework;
using System;

namespace CommonMessage
{
    public struct PacketSend
    {
        public DateTime time;
        public IMessage packet;

        public PacketSend(DateTime time, IMessage packet)
        {
            this.time = time;
            this.packet = packet;
        }
    }

    public struct PacketReceive
    {
        public DateTime time;
        public IMessage packet;

        public PacketReceive(DateTime time, IMessage packet)
        {
            this.time = time;
            this.packet = packet;
        }
    }
}
