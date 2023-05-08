using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Networking.Transport;
using UnityEngine;

namespace ChatClientExample
{
    public class GameSpawnMessage : MessageHeader
    {
		public override NetworkMessageType Type { 
			get {
				return NetworkMessageType.GAME_SPAWN;
			}
		}

		public NetworkSpawnObject objectType;
		public uint networkId;

		public Unity.Collections.FixedString128Bytes playerName;
        public SpecializationType selectedSpecialization = SpecializationType.Warrior;

        public uint isPlayersTurn = 0; //0 is not, 1 is player turn.

        public override void SerializeObject(ref DataStreamWriter writer) {
			// very important to call this first
			base.SerializeObject(ref writer);
			
			writer.WriteUInt(networkId);
			writer.WriteUInt((uint)objectType);

			writer.WriteFixedString128(playerName);
			writer.WriteUInt((uint)selectedSpecialization);

            writer.WriteUInt(isPlayersTurn);
        }

		public override void DeserializeObject(ref DataStreamReader reader) {
			// very important to call this first
			base.DeserializeObject(ref reader);

			networkId = reader.ReadUInt();
			objectType = (NetworkSpawnObject)reader.ReadUInt();

			playerName = reader.ReadFixedString128();
			selectedSpecialization = (SpecializationType)reader.ReadUInt();

            isPlayersTurn = reader.ReadUInt();
        }
	}
}