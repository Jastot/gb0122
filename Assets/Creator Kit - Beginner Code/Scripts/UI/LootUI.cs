using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace CreatorKitCodeInternal 
{
    /// <summary>
    /// Handle display the names of every loot on screen (which are button for easy pickup)
    /// </summary>
    public class LootUI : MonoBehaviour
    {
        public static LootUI Instance { get; protected set; }

        const int BUTTON_OFFSET = 32;
    
        struct ButtonText
        {
            public Button LootButton;
            public Text LootName;
        }
    
        struct DisplayedLoot
        {
            public PotionView TargetLoot;
            public ButtonText TargetButton;
        }

        public Button ButtonPrefab;
        
        Queue<ButtonText> m_ButtonPool = new Queue<ButtonText>();
        List<PotionView> m_OffScreenLoot = new List<PotionView>();
        List<DisplayedLoot> m_OnScreenLoot = new List<DisplayedLoot>();
        public List<CharacterControl> m_CharacterControl = new List<CharacterControl>();
        
        void OnEnable()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            
            const int poolSize = 16;
            for (int i = 0; i < poolSize; ++i)
            {
                //probably more efficient to use a custom 
                Button b = Instantiate(ButtonPrefab, transform);
                b.gameObject.SetActive(false);
                Text t = b.GetComponentInChildren<Text>();
            
                m_ButtonPool.Enqueue(new ButtonText(){ LootButton = b, LootName = t});
            }
            
        }

        public void NewLoot(PotionView loot)
        {
            Vector3 screenPos;
            if (OnScreen(loot.transform.position, out screenPos))
            {
                AddButton(loot, screenPos);
            }
            else
            {
                m_OffScreenLoot.Add(loot);
            }
        }

        void AddButton(PotionView l, Vector3 screenPosition)
        {
            DisplayedLoot dl;

            dl.TargetLoot = l;
            dl.TargetButton = m_ButtonPool.Dequeue();
            dl.TargetButton.LootButton.gameObject.SetActive(true);
            dl.TargetButton.LootButton.transform.position = screenPosition + Vector3.up * BUTTON_OFFSET;

            dl.TargetButton.LootButton.onClick.RemoveAllListeners(); 
            dl.TargetButton.LootButton.onClick.AddListener(() =>
            {
                foreach (var variableCharacterControl in m_CharacterControl)
                {
                    if (variableCharacterControl.photonView.IsMine)
                    {
                        variableCharacterControl.InteractWith(l);
                    }
                }
            } );
            dl.TargetButton.LootName.text = l.Item.ItemName;
        
            m_OnScreenLoot.Add(dl);
        }

        public void DestroyUI(PotionView loot)
        {
            foreach (var variableDisplayedLoot in m_OnScreenLoot)
            {
                var displayedLoot = variableDisplayedLoot;
                if (displayedLoot.TargetLoot == loot)
                {
                    displayedLoot.TargetButton.LootButton.onClick.RemoveAllListeners();
                    displayedLoot.TargetLoot = null;
                    Destroy(displayedLoot.TargetButton.LootButton.gameObject);
                    
                    break;
                }
            }    
        }
        
        bool OnScreen(Vector3 position, out Vector3 screenPosition)
        {
            screenPosition = Camera.main.WorldToScreenPoint(position);
            return (screenPosition.x >= 0 && screenPosition.y >= 0 && screenPosition.x <= Screen.width && screenPosition.y <= Screen.height);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            List<PotionView> newOffscreen = new List<PotionView>();
        
            for (int i = 0; i < m_OnScreenLoot.Count; ++i)
            {
                Vector3 sp;
                var entry = m_OnScreenLoot[i];

                if (entry.TargetLoot != null && OnScreen(entry.TargetLoot.transform.position, out sp))
                {
                    entry.TargetButton.LootButton.transform.position = sp + Vector3.up * BUTTON_OFFSET;
                }
                else
                {
                    m_OnScreenLoot.RemoveAt(i);
                    if (entry.TargetButton.LootButton!=null)
                        entry.TargetButton.LootButton.gameObject.SetActive(false);
                    m_ButtonPool.Enqueue(entry.TargetButton);
                    newOffscreen.Add(entry.TargetLoot);
                    i--;
                }
            }

            for (int i = 0; i < m_OffScreenLoot.Count; ++i)
            {
                Vector3 sp;
                var loot = m_OffScreenLoot[i];

                if (loot != null)
                {
                    if (OnScreen(loot.transform.position, out sp))
                    {
                        AddButton(loot, sp);
                        m_OffScreenLoot.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    m_OffScreenLoot.RemoveAt(i);
                    i--;
                }
            }
        
            //do that at the end so we don't recompute their position in the second loop
            m_OffScreenLoot.AddRange(newOffscreen);
        }
    }
}