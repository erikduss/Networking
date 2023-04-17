using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace ChatClientExample
{
    public class ReadyStatusUpdateMessage : MessageHeader
    {
		public override NetworkMessageType Type { 
			get {
				//return NetworkMessageType.READY_STATUS_UPDATE;
				return NetworkMessageType.PING;
			}
		}

		public uint status; //0 is not ready, 1 is ready
		public uint networkId;

		public override void SerializeObject(ref DataStreamWriter writer) {
			// very important to call this first
			base.SerializeObject(ref writer);

			writer.WriteUInt(networkId);
			writer.WriteUInt(status);
		}

		public override void DeserializeObject(ref DataStreamReader reader) {
			// very important to call this first
			base.DeserializeObject(ref reader);

			networkId = reader.ReadUInt();
			status = reader.ReadUInt();
		}
	}
}