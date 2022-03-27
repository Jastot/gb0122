using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreatorKitCode;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    
    [SerializeField] private List<int> HowMany;
    [SerializeField] private List<string> WhatToSpawn;
    private List<EnemiesSpawnerPoint> _enemiesSpawnerPoints;
    private Queue<int> _queueSpawn = new Queue<int>();
    private Dictionary<int,int> _counter;
    
    private int _allCountOfEnemy;
    public int StartCountOfEnemies;
    private List<CharacterData> _characterDataList;
    
    public event Action<int> CountOfEnemyChanged; 
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var VARIABLE in HowMany)
            {
                _allCountOfEnemy += VARIABLE;
            }

            StartCountOfEnemies = _allCountOfEnemy;
            _enemiesSpawnerPoints = this.gameObject.GetComponentsInChildren<EnemiesSpawnerPoint>().ToList();
            _counter = new Dictionary<int, int>();
            _characterDataList = new List<CharacterData>();
            while (_allCountOfEnemy != _queueSpawn.Count)
            {
                GenerateQueue();
                var s = "";
                foreach (var VARIABLE in _queueSpawn)
                {
                    s += VARIABLE + " ";
                }
            }

            SpawnEnemies();
        }
    }

    public List<CharacterData> GetEnemiesCharData()
    {
        return _characterDataList;
    }

    private void GenerateQueue()
    {
        var gen = Random.Range(0, HowMany.Count-1);
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
                var go = PhotonNetwork.InstantiateRoomObject(prefab,_enemiesSpawnerPoints[usedSpawnPoints].transform.position ,
                    _enemiesSpawnerPoints[usedSpawnPoints].transform.rotation  ,0);
                _characterDataList.Add(go.GetComponent<CharacterData>());
                _characterDataList.Last().DeathRattle += LessEnemy;
                usedSpawnPoints++;
            }
            whatToSpawnIndex++;
        }
    }

    private void LessEnemy(int s,CharacterData characterData)
    {
        _allCountOfEnemy--;
        CountOfEnemyChanged?.Invoke(_allCountOfEnemy);
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var character in _characterDataList)
            {
                character.DeathRattle -= LessEnemy;
            }
        }
    }
}
