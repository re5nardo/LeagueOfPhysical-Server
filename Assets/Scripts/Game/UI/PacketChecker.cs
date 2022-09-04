using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Mirror;
using System.Text;

public class PacketChecker : MonoBehaviour
{
    [SerializeField] private Text Text;

    public List<(DateTime, Type)> packets = new List<(DateTime, Type)>();

    private void Update()
    {
        if (!NetworkServer.active) return;

        packets.RemoveAll(x => (DateTime.Now - x.Item1).TotalSeconds > 1);

        Dictionary<Type, int> counts = new Dictionary<Type, int>();
        packets.ForEach(kv =>
        {
            if (counts.ContainsKey(kv.Item2))
            {
                counts[kv.Item2]++;
            }
            else
            {
                counts.Add(kv.Item2, 1);
            }
        });

        var sb = new StringBuilder();
        foreach (var kv in counts)
        {
            sb.Append(kv.Key.Name).Append(": ").Append(kv.Value).AppendLine();
        }
        Text.text = $"{sb.ToString()}";
    }
}
