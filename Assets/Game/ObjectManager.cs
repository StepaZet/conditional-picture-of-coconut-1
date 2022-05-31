using System;
using UnityEngine;

namespace Game
{
	public class ObjectManager : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.transform.CompareTag("Respawn"))
				return;
			for (var i = 0; i < other.transform.childCount; i++) 
				other.transform.GetChild(i).gameObject.SetActive(true);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.transform.CompareTag("Respawn"))
				return;
			for (var i = 0; i < other.transform.childCount; i++)
				other.transform.GetChild(i).gameObject.SetActive(false);
		}
	}
}