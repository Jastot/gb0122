using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
	private void Awake()
	{
		PhotonNetwork.Instantiate("Character", gameObject.transform.position, gameObject.transform.rotation);
	}
}