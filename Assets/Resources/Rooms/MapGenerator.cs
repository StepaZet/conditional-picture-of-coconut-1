using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Player;
using Resources.Rooms;
using UnityEngine;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]private GameObject gridObj;
    [SerializeField]private PlayerObj player;
    [SerializeField] public List<GameObject> roomsPrefabs;
    [SerializeField] public List<GameObject> bossRoomsPrefabs;
    [SerializeField] public List<GameObject> corridorStraightPrefabs;
    [SerializeField] public List<GameObject> corridorCornerPrefabs;
    private List<(List<GameObject>, int)> prefabsGenerationChance;

    [SerializeField] public List<RoomGenerator> generatedRooms;
    [SerializeField] public List<RoomGenerator> generatedRoomsWithoutCorridors;
    //[SerializeField] public GameObject deadEnd;
    [SerializeField]private GameObject startRoom;
    [SerializeField] private int minNumberOfRoomsToBeGenerated;
    [SerializeField] private int waitingTime;
    private static System.Random random = new System.Random();

    // Start is called before the first frame update
    private void Start()
    {
        prefabsGenerationChance = new List<(List<GameObject>, int)>
        {
            (roomsPrefabs, 5),
            (bossRoomsPrefabs, 1),
            (corridorStraightPrefabs, 10),
            (corridorCornerPrefabs, 5)
        };
        
        var spawnedRoom = Instantiate(startRoom, gridObj.transform);
        generatedRooms.Add(spawnedRoom.GetComponent<RoomGenerator>());
        spawnedRoom.GetComponent<RoomGenerator>().GenerateAdjacentRooms(corridorStraightPrefabs);
    }

    public void Update()
    {
        while (generatedRoomsWithoutCorridors.Count < minNumberOfRoomsToBeGenerated && Time.timeSinceLevelLoad < waitingTime)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
            var (chosenPrefabs, chance) = prefabsGenerationChance.SelectItem();
            for (var index = 0; index < generatedRooms.Count; index++)
            {
                var generatedRoom = generatedRooms[index];
                var result = generatedRoom.GenerateAdjacentRooms(chosenPrefabs);
                if (chosenPrefabs != corridorCornerPrefabs && chosenPrefabs != corridorStraightPrefabs)
                    generatedRoomsWithoutCorridors.AddRange(result);
                if (result == null || !result.Any())
                {
                    var result2 = generatedRoom.GenerateAdjacentRooms(corridorStraightPrefabs);
                    // TODO Вставить метод для генерации тупика
                }
            }
    }

    public GameObject InstantiateRandomRoom()
    {
        var randomRoomIndex = random.Next(roomsPrefabs.Count - 1);
        var room = Instantiate(roomsPrefabs[randomRoomIndex]);
        generatedRooms.Add(room.GetComponent<RoomGenerator>());

        return room;
    }
}
