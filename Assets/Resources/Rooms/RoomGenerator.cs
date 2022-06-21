using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Mathematics;
using UnityEngine;

namespace Resources.Rooms
{
	public class RoomGenerator : MonoBehaviour
	{
		public List<Transform> openings;
		public Dictionary<Transform, bool> isOpeningGenerated = new Dictionary<Transform, bool>();
		[SerializeField] private MapGenerator mapGenerator;
		[SerializeField] private Collider2D collider2D;
		private GridObj grid;
		[SerializeField] private Transform leftDownPoint;
		[SerializeField] private Transform rightUpPoint;
		public bool noOpeningsLeft;

		public void OnEnable()
		{
			foreach (var opening in openings) 
				if (!isOpeningGenerated.ContainsKey(opening))
					isOpeningGenerated[opening] = false;
			
			grid = GameObject.Find("GridActualUnity").GetComponent<GridObj>();
			mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
			//GenerateAdjacentRooms();
		}

		public IEnumerable<RoomGenerator> GenerateAdjacentRooms(List<GameObject> prefabs)
		{
			prefabs.Shuffle();
			var isSuccessful = false;
			foreach (var prefab in prefabs)
			{
				var roomObj = Instantiate(prefab, grid.transform);
				foreach (var opening in isOpeningGenerated.Keys.Where(x => !isOpeningGenerated[x]))
				{
					roomObj.transform.position = new Vector3(roomObj.transform.position.x, roomObj.transform.position.y, 0);
					var room = roomObj.GetComponent<RoomGenerator>();
			
					if (room.isOpeningGenerated.Count == 0)
						foreach (var roomOpening in room.openings) 
							room.isOpeningGenerated[roomOpening] = false;

					foreach (var newOpening in room.isOpeningGenerated.Keys.Where(x => !room.isOpeningGenerated[x]))
					{
						room.transform.position += opening.position - newOpening.position;
						var hasCollidedWithOther = mapGenerator.generatedRooms
							.Except(new[] {room})
							.Any(r => IsColliding(room, r));
						if (!hasCollidedWithOther)
						{
							isSuccessful = true;
							room.isOpeningGenerated[newOpening] = true;
							isOpeningGenerated[opening] = true;
							mapGenerator.generatedRooms.Add(room);
							yield return room;
							break;
						}
					}

					if (isSuccessful)
						break;
				}

                if (isSuccessful)
                {
                    if (roomObj.GetComponent<CharacterGenerated>())
                    {
                        roomObj.GetComponent<CharacterGenerated>().SpawnCharacters();
                    }
                    break;
				}
					
				Destroy(roomObj);
			}
		}

		public (RoomGenerator room, bool isSuccessful) GenerateRoom(Transform opening, GameObject roomPrefabObj)
		{
			var roomObj = Instantiate(roomPrefabObj, grid.transform);
			roomObj.transform.position = new Vector3(roomObj.transform.position.x, roomObj.transform.position.y, 0);
			var room = roomObj.GetComponent<RoomGenerator>();
			
			if (room.isOpeningGenerated.Count == 0)
				foreach (var roomOpening in room.openings) 
					room.isOpeningGenerated[roomOpening] = false;

			foreach (var newOpening in room.isOpeningGenerated.Keys.Where(x => !room.isOpeningGenerated[x]))
			{
				room.transform.position += opening.position - newOpening.position;
				var hasCollidedWithOther = mapGenerator.generatedRooms
					.Except(new[] {room})
					.Where(x => IsInGridObj(room))
					.Any(r => IsColliding(room, r));
				if (!hasCollidedWithOther)
					return (room, true);
			}

			return (room, false);
		}

		private static bool IsColliding(RoomGenerator room1, RoomGenerator room2)
		{
			var l1 = room1.leftDownPoint.position;
			var r1 = room1.rightUpPoint.position;
			var l2 = room2.leftDownPoint.position;
			var r2 = room2.rightUpPoint.position;
			var delta = 0.1f;

			if (l1.x + delta >= r2.x || l2.x + delta >= r1.x)
			{
				return false;
			}
 
			if (r1.y <= l2.y + delta || r2.y <= l1.y + delta)
			{
				return false;
			}
			return true;
		}

		private static bool IsInGridObj(RoomGenerator room)
		{
			const float cellSize = 1.28f;
			var startPosition = new float2(cellSize * (-18), cellSize * (-40));
			const int width = 170;
			const int height = 130;
			var delta = 0.1f;
			
			var l1 = room.leftDownPoint.position;
			var r1 = room.rightUpPoint.position;
			var l2 = startPosition;
			var r2 = new Vector3(l2.x + width*cellSize, l2.y + height*cellSize, 0);
			
			if (l1.x + delta >= r2.x || l2.x + delta >= r1.x)
			{
				return false;
			}
 
			if (r1.y <= l2.y + delta || r2.y <= l1.y + delta)
			{
				return false;
			}
			return true;
		}
	}
}
