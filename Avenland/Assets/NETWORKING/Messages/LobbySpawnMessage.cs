using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Networking.Transport;
using UnityEngine;

namespace ChatClientExample
{
    public class LobbySpawnMessage : MessageHeader
    {
		public override NetworkMessageType Type { 
			get {
				return NetworkMessageType.LOBBY_SPAWN;
			}
		}

		public NetworkSpawnObject objectType;
		public uint networkId;

		public Unity.Collections.FixedString128Bytes playerName;

		public uint isReady = 0; //0 is not ready, 1 is ready.
        public SpecializationType selectedSpecialization = SpecializationType.Warrior;

        public override void SerializeObject(ref DataStreamWriter writer) {
			// very important to call this first
			base.SerializeObject(ref writer);
			
			writer.WriteUInt(networkId);
			writer.WriteUInt((uint)objectType);

			writer.WriteFixedString128(playerName);

			writer.WriteUInt(isReady);

			writer.WriteUInt((uint)selectedSpecialization);
		}

		public override void DeserializeObject(ref DataStreamReader reader) {
			// very important to call this first
			base.DeserializeObject(ref reader);

			networkId = reader.ReadUInt();
			objectType = (NetworkSpawnObject)reader.ReadUInt();

			playerName = reader.ReadFixedString128();

			isReady = reader.ReadUInt();

			selectedSpecialization = (SpecializationType)reader.ReadUInt();
		}
	}
}