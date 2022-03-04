using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{ 
    private int _allCountOfEnemy;
    [SerializeField] private List<int> HowMany;
    [SerializeField] private List<string> WhatToSpawn;
    private List<EnemiesSpawnerPoint> _enemiesSpawnerPoints;
    private Queue<int> _queueSpawn = new Queue<int>();
    private Dictionary<int,int> _counter;
    void Start()
    {
        foreach (var VARIABLE in HowMany)
        {
            _allCountOfEnemy += VARIABLE;
        }
        _enemiesSpawnerPoints = this.gameObject.GetComponentsInChildren<EnemiesSpawnerPoint>().ToList();
        _counter = new Dictionary<int,int>();
        while (_allCountOfEnemy != _queueSpawn.Count)
        {
            GenerateQueue();
            var s = "";
            foreach (var VARIABLE in _queueSpawn)
            {
                s += VARIABLE + " ";
            }
            Debug.Log(s);
        }
        SpawnEnemies();
    }

    private void GenerateQueue()
    {
        var gen = Random.Range(0, HowMany.Count);
        if (!_counter.ContainsKey(gen))
        {
            _counter.Add(gen,1);
        }
        else
        {
            if (_counter[gen] != HowMany[gen])
            {
                _counter[gen]++;
            }
            else
            {
                return;
            }
        }
        _queueSpawn.Enqueue(gen);
    }

    private void SpawnEnemies()
    {
        var usedSpawnPoints = 0;
        var whatToSpawnIndex = 0;
        foreach (var prefab in WhatToSpawn)
        {
            for (int i = 0; i < HowMany[whatToSpawnIndex]; i++)
            {
                PhotonNetwork.InstantiateRoomObject(prefab,_enemiesSpawnerPoints[usedSpawnPoints].transform.position ,
                    _enemiesSpawnerPoints[usedSpawnPoints].transform.rotation  ,0);
                usedSpawnPoints++;
            }
            whatToSpawnIndex++;
        }
    }
}
