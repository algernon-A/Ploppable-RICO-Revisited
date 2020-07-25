using System;
using ColossalFramework;
using System.IO;
using ColossalFramework.IO;
using ICities;
using UnityEngine;

namespace PloppableRICO
{
    /// <summary>
    /// The data object that tracks what buildings are plopped, and which were grown. We can use this data to apply differnt AI logic to each.
    /// </summary>

    public class RICOBuildingManager : SerializableDataExtensionBase
	{

		public static RICOInstanceData[] RICOInstanceData;
	    public static RICOBuildingManager Instance;


		// The key for our data in the savegame
		public const string DataId = "PRCIO";

		// Version of data save format
		public const uint DataVersion = 0;

		public override void OnCreated(ISerializableData serializableData)
		{

            RICOInstanceData = new RICOInstanceData[63000];

            for (uint i = 0; i < 62000; i++)
            {
                RICOInstanceData[i] = new RICOInstanceData();
            }

            base.OnCreated(serializableData);

		}

        public static void AddBuilding(BuildingInfo prefab, uint ID)
        {
            //This is called from building tool. The data it sets is read by methods in the RICO AIs and BuildingTool detours.

            Debug.Log("Add Building Called with ID = " + ID);

            if (prefab.m_buildingAI is PloppableOfficeAI || prefab.m_buildingAI is PloppableExtractorAI || prefab.m_buildingAI is PloppableResidentialAI || prefab.m_buildingAI is PloppableCommercialAI || prefab.m_buildingAI is PloppableIndustrialAI)
            {

                RICOInstanceData data = RICOBuildingManager.RICOInstanceData[(int)ID];
                if (data != null)
                {
                    data.Name = prefab.name;
                    data.plopped = true; //since this is called from building tool, it must be plopped.
                }

            }
        }

        public static void RemoveBuilding(uint ID)
        {


        }

        public static bool IsPlopped(uint ID) {

            return RICOInstanceData[ID].plopped;

        }

        public override void OnLoadData()
		{
			base.OnLoadData();

			// Get bytes from savegame
			byte[] bytes = serializableDataManager.LoadData(DataId);
			if (bytes == null) return;

			// Convert the bytes to an array of BuildingData objects
			using (var stream = new MemoryStream(bytes))
			{
                RICOInstanceData = DataSerializer.DeserializeArray<RICOInstanceData>(stream, DataSerializer.Mode.Memory);
			}

			Debug.LogFormat("Data loaded (Size in bytes: {0})", bytes.Length);


		}

		public override void OnSaveData()
		{
			base.OnSaveData();

			byte[] bytes;

			// Convert the array of BuildingData objects to bytes
			using (var stream = new MemoryStream())
			{
				DataSerializer.SerializeArray(stream, DataSerializer.Mode.Memory, DataVersion, RICOInstanceData);
				bytes = stream.ToArray();
			}

			// Save bytes in savegame
			serializableDataManager.SaveData(DataId, bytes);

			Debug.LogFormat("Data Saved (Size in bytes: {0})", bytes.Length);
			//Debug.Log (data.fieldB.ToString ());

		}

		public override void OnReleased()
		{
			base.OnReleased();

            // Clear to save memory
            RICOInstanceData = null;
		}
	}

	/// <summary>
	/// The data class that holds the extra data for each building.
	/// </summary>
	public class RICOInstanceData : IDataContainer
	{
		public string Name;
		public bool plopped;

		// This serializes the object (to bytes)
		public void Serialize(DataSerializer s)
		{
			s.WriteSharedString(Name);
			//s.WriteInt32(Depth);
			s.WriteBool(plopped);

		}

		// This reads the object (from bytes)
		public void Deserialize(DataSerializer s)
		{
			Name = s.ReadSharedString();
			//Depth = s.ReadInt32();
			plopped = s.ReadBool();

		}

		public void AfterDeserialize(DataSerializer s) {}
	}

}

