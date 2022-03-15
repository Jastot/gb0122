using System.Collections.Generic;
using Data;
using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
	[SerializeField] private Transform parent;
	[SerializeField] private List<Transform> _spawnPointsCOOP;
	[SerializeField] private List<Transform> _spawnPointsTwoTeams;
	[SerializeField] private List<Transform> _spawnPointsHateAll;
	[SerializeField] private CharactersLocalData _charactersLocalData;
	[HideInInspector]
	public PhotonLogin.GameType GameType;
	
	public void Awake()
	{
		GameType = (PhotonLogin.GameType)
			PhotonNetwork.CurrentRoom.CustomProperties["GameType"];
		int count = 0;
		foreach (var player in  PhotonNetwork.PlayerList)
		{
			if (_charactersLocalData.InRoomId == player.UserId)
			{
				break;
			}
			count++;
		}
		GameObject go;
		switch (GameType)
		{
			case PhotonLogin.GameType.COOP:
				go = PhotonNetwork.Instantiate("Character", _spawnPointsCOOP[count].position,
					_spawnPointsCOOP[count].rotation);
				go.transform.parent = parent;
				break;
			case PhotonLogin.GameType.TwoTeams:
				go = PhotonNetwork.Instantiate("Character", _spawnPointsTwoTeams[count].position,
					_spawnPointsTwoTeams[count].rotation);
				go.transform.parent = parent;
				break;
			case PhotonLogin.GameType.HateAll:
				go = PhotonNetwork.Instantiate("Character", _spawnPointsHateAll[count].position,
					_spawnPointsHateAll[count].rotation);
				go.transform.parent = parent;
				break;
		}
	}
}