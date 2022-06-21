using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerated : MonoBehaviour
{
    public GameObject GoldPosition;
    public GameObject[] Positions;

    public void SpawnCharacters()
    {
        var bank = GameObject.Find("CharacterBank").GetComponent<CharacterBank>();
        foreach (var position in Positions)
        {
            var character = bank.Characters[Random.Range(0, bank.Characters.Length - 1)];
            Instantiate(character, position.transform.position, Quaternion.identity);
        }
        
        if (GoldPosition != null && Random.Range(0, 20) == 20)
            Instantiate(bank.GoldCharacter, GoldPosition.transform.position, Quaternion.identity);
    }
}
