using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [SerializeField] public GameObject deadEnd;
    [SerializeField]private GameObject startRoom;
    [SerializeField] private int minNumberOfRoomsToBeGenerated;
    [SerializeField] private int waitingTime;
    private static System.Random random = new System.Random();
    private int counter;

    // Start is called before the first frame update
    private void Start()
    {
        prefabsGenerationChance = new List<(List<GameObject>, int)>
        {
            (roomsPrefabs, 5),
            (bossRoomsPrefabs, 1),
            (corridorStraightPrefabs, 9),
            (corridorCornerPrefabs, 1)
        };
        
        var spawnedRoom = Instantiate(startRoom, gridObj.transform);
        generatedRooms.Add(spawnedRoom.GetComponent<RoomGenerator>());
        spawnedRoom.GetComponent<RoomGenerator>().GenerateAdjacentRooms(corridorStraightPrefabs);
    }

    public void Spawn()
    {
        var tries = 0;
        while(generatedRoomsWithoutCorridors.Count <= 15 && tries < 8)
        {
            tries++;
            for (var index = 0; index < generatedRooms.Count; index++)
            {
                var generatedRoom = generatedRooms[index];
                var (chosenPrefabs, chance) = prefabsGenerationChance.SelectItem();
                var result = generatedRoom.GenerateAdjacentRooms(chosenPrefabs);
                if (chosenPrefabs != corridorCornerPrefabs && chosenPrefabs != corridorStraightPrefabs)
                {
                    generatedRoomsWithoutCorridors.AddRange(result);
                }

                if (result == null || !result.Any())
                {
                    //var result2 = generatedRoom.GenerateAdjacentRooms(corridorStraightPrefabs);
                    // TODO Вставить метод для генерации тупика
                }

                
            }
        }

        foreach (var openings in generatedRooms.Select(r => r.isOpeningGenerated.Where(o => !o.Value)))
        {
            foreach (var opening in openings)
            {
                var currentDeadEnd = Instantiate(deadEnd);
                currentDeadEnd.transform.position = opening.Key.position;
                currentDeadEnd.transform.localScale *= 2;
            }
        }
    }

    private void DestroyEverything()
    {
        for (var index = 0; index < generatedRooms.Count; index++)
        {
            var room = generatedRooms[index];
            Destroy(room.gameObject);
            generatedRooms = new List<RoomGenerator>();
            generatedRoomsWithoutCorridors = new List<RoomGenerator>();
            var spawnedRoom = Instantiate(startRoom, gridObj.transform);
            generatedRooms.Add(spawnedRoom.GetComponent<RoomGenerator>());
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
