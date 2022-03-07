using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
	[SerializeField] private Transform parent;
	
	private void Awake()
	{
		var go = PhotonNetwork.Instantiate("Character", gameObject.transform.position, gameObject.transform.rotation);
		go.transform.parent = parent;
	}
}