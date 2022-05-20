using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] protected GameObject Doors;
    [SerializeField] protected GameObject[] EnemyWaves;
    [SerializeField] private List<GameObject> Enemies;
    private int waveNumber;
    private bool isActive;
    private Stage currentStage;
    private float pauseStart;
    private float pauseTime = 1f;

    private enum Stage
    {
        None,
        Wave,
        Pause
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isActive)
            return;

        if (collider.gameObject.layer != LayerMask.NameToLayer("Character"))
            return;

        Doors.SetActive(true);
        isActive = true;
        Enemies = new List<GameObject>();
        StartPause();
    }

    private void FixedUpdate()
    {
        switch (currentStage)
        {
            case Stage.None:
                break;
            case Stage.Wave:
                CheckWave();
                break;
            case Stage.Pause:
                var difference = Time.time - pauseStart;
                if (difference >= pauseTime)
                    OpenNextWave();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void StartPause()
    {
        pauseStart = Time.time;
        currentStage = Stage.Pause;
    }

    private void CheckWave()
    {
        if (IsRoomClear())
        {
            waveNumber++;
            if (waveNumber >= EnemyWaves.Length)
                WinRoom();
            StartPause();
        }
    }

    private void OpenNextWave()
    {
        currentStage = Stage.Wave;
        EnemyWaves[waveNumber].gameObject.SetActive(true);
        UpdateEnemiesList();
    }

    private void UpdateEnemiesList()
    {
        Enemies.Clear();
        var newEnemies = Physics2D.OverlapAreaAll(
            transform.position - transform.localScale / 2,
            transform.position + transform.localScale / 2);

        foreach (var newEnemy in newEnemies)
        {
            if (newEnemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Enemies.Add(newEnemy.gameObject);
            }
        }
    }

    private bool IsRoomClear()
    {
        if (Enemies.Any(obj => obj != null))
            return false;
        UpdateEnemiesList();
        return Enemies.All(obj => obj == null);
    }

    private void WinRoom()
    {
        Doors.SetActive(false);
        Destroy(gameObject);
    }
}
