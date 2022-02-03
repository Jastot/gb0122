using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
	private void Start()
	{
		PhotonNetwork.Instantiate("Character", gameObject.transform.position, gameObject.transform.rotation);
	}
}