using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using CreatorKitCodeInternal;
using Photon.Pun;
using UnityEngine;

public class PotionView : InteractableObject, IPunObservable
{
    public override bool IsInteractable => m_AnimationTimer >= AnimationTime;
    float m_AnimationTimer = 0.0f;
    static float AnimationTime = 0.5f;
    public Item Item;

    Vector3 m_OriginalPosition;
    Vector3 m_TargetPoint;
    protected override void Start()
    {
        base.Start();
        //gameObject.GetPhotonView().RPC("SomeRPCView",RpcTarget.MasterClient);
    }

    void Update()
    {
        if (m_AnimationTimer < AnimationTime)
        {
            m_AnimationTimer += Time.deltaTime;

            float ratio = Mathf.Clamp01(m_AnimationTimer / AnimationTime);

            Vector3 currentPos = Vector3.Lerp(m_OriginalPosition, m_TargetPoint, ratio);
            currentPos.y = currentPos.y + Mathf.Sin(ratio * Mathf.PI) * 2.0f;
            
            transform.position = currentPos;

            if (m_AnimationTimer >= AnimationTime)
            {
                LootUI.Instance.NewLoot(this);
            }
        }
        
        Debug.DrawLine(m_TargetPoint, m_TargetPoint + Vector3.up, Color.magenta);
    }
    
    public override void InteractWith(CharacterData target)
    {
        target.Inventory.AddItem(Item);
        SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData(){Clip = SFXManager.PickupSound});
        
        UISystem.Instance.InventoryWindow.Load(target);
        var ph = this.gameObject.GetPhotonView();
        ph.RPC("SendDestroyToChildren", RpcTarget.All);
        ph.RPC("SendSomeInfoToMaster", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SendSomeInfoToMaster()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    public void SendDestroyToChildren()
    {
        LootUI.Instance.DestroyUI(this);
    }

    [PunRPC]
    public void GetSomePosFromSpawner(float x, float y ,float z)
    {
        m_OriginalPosition = m_TargetPoint = new Vector3(x, y, z);
        gameObject.transform.position = new Vector3(x, y, z);
    }
    
    [PunRPC]
    private void GetSomeInfoFromSpawner()
    {
        Item = Resources.Load<Item>("ItemDatabase/Potion");;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
