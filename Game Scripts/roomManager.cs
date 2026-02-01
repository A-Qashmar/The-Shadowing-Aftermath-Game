using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<Door> doors;
    public List<Health> enemies;

    int remainingEnemies;
    bool doorsOpened;

    void Start()
    {
        remainingEnemies = 0;
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            remainingEnemies++;
            enemy.onDeathWithReference.AddListener(OnEnemyDeath);
        }

        foreach (var door in doors)
        {
            if (door != null)
                door.Close();
        }
    }

    void OnEnemyDeath(GameObject source)
    {
        if (doorsOpened) return;

        remainingEnemies--;
        if (remainingEnemies <= 0)
            OpenDoors();
    }

    void OpenDoors()
    {
        doorsOpened = true;
        foreach (var door in doors)
        {
            if (door != null)
                door.Open();
        }
    }
}
