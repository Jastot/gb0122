using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using Photon.Pun;
using PlayFab;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CreatorKitCodeInternal 
{
    /// <summary>
    /// Main class that handle the Game UI (health, open/close inventory)
    /// </summary>
    public class UISystem : MonoBehaviour
    {
        public static UISystem Instance { get; private set; }
    
        [Header("Player")]
        public CharacterControl PlayerCharacter;
        public Slider PlayerHealthSlider;
        public Text MaxHealth;
        public Text CurrentHealth;
        public EffectIconUI[] TimedModifierIcones;
        public Text StatsText;

        [Header("GameStart")] 
        public StartGameUI StartGameUI;
        
        [Header("Enemy")]
        public Slider EnemyHealthSlider;
        public Text EnemyName;
        public EffectIconUI[] EnemyEffectIcones;
        public AudioClip FightClip;
        
        [Header("Inventory")]
        public InventoryUI InventoryWindow;
        public Button OpenInventoryButton;
        public AudioClip OpenInventoryClip;
        public AudioClip CloseInventoryClip;

        Sprite m_ClosedInventorySprite;
        Sprite m_OpenInventorySprite;

        void Awake()
        {
            Instance = this;
        
            InventoryWindow.Init();
        }

        void Start()
        {
            var players = FindObjectsOfType<CharacterControl>();
            foreach (var player in players)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    PlayerCharacter = player;
                    break;
                }
            }
            
            m_ClosedInventorySprite = ((Image)OpenInventoryButton.targetGraphic).sprite;
            m_OpenInventorySprite = OpenInventoryButton.spriteState.pressedSprite;

            for (int i = 0; i < TimedModifierIcones.Length; ++i)
            {
                TimedModifierIcones[i].gameObject.SetActive(false);
            }
        
            for (int i = 0; i < EnemyEffectIcones.Length; ++i)
            {
                EnemyEffectIcones[i].gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (PlayerCharacter == null)
            {
                return;
                
            }
            UpdatePlayerUI();
        }

        private bool isInBattleAlready = false;
        private AudioSource NowBattleMusicPlaying;
        
        void UpdatePlayerUI()
        {
            CharacterData data = PlayerCharacter.Data;
        
            PlayerHealthSlider.value = PlayerCharacter.Data.Stats.CurrentHealth / (float) PlayerCharacter.Data.Stats.stats.health;
            MaxHealth.text = PlayerCharacter.Data.Stats.stats.health.ToString();
            CurrentHealth.text = PlayerCharacter.Data.Stats.CurrentHealth.ToString();
        
            if (PlayerCharacter.CurrentTarget != null)
            {
                if (!isInBattleAlready)
                {
                    NowBattleMusicPlaying = SFXManager.PlaySound(SFXManager.Use.Sound2D, 
                        new SFXManager.PlayData(){ Clip = FightClip},true);
                    isInBattleAlready = true;
                }
                UpdateEnemyUI(PlayerCharacter.CurrentTarget);
            }
            else
            {
                if (isInBattleAlready)
                {
                    NowBattleMusicPlaying.Stop();
                    isInBattleAlready = false;
                }
                EnemyHealthSlider.gameObject.SetActive(false);
            }

            int maxTimedEffect = data.Stats.TimedModifierStack.Count;
            for (int i = 0; i < maxTimedEffect; ++i)
            {
                var effect = data.Stats.TimedModifierStack[i];

                TimedModifierIcones[i].BackgroundImage.sprite = effect.EffectSprite;
                TimedModifierIcones[i].gameObject.SetActive(true);
                TimedModifierIcones[i].TimeSlider.value = effect.Timer / effect.Duration;
            }

            for (int i = maxTimedEffect; i < TimedModifierIcones.Length; ++i)
            {
                TimedModifierIcones[i].gameObject.SetActive(false);
            }
        
                
            var stats = data.Stats.stats;
            StatsText.text = $"Str : {stats.strength} Def : {stats.defense} Agi : {stats.agility}";
        }

        void UpdateEnemyUI(CharacterData enemy)
        {
            EnemyHealthSlider.gameObject.SetActive(true);
            EnemyHealthSlider.value = enemy.Stats.CurrentHealth / (float) enemy.Stats.stats.health;
            EnemyName.text = enemy.CharacterName;

            int top = enemy.Stats.ElementalEffects.Count;
        
            for (int i = 0; i < top; ++i)
            {
                var effect = enemy.Stats.ElementalEffects[i];
            
                EnemyEffectIcones[i].gameObject.SetActive(true);
                EnemyEffectIcones[i].TimeSlider.value = effect.CurrentTime / effect.Duration;
            }

            for (int i = top; i < EnemyEffectIcones.Length; ++i)
            {
                EnemyEffectIcones[i].gameObject.SetActive(false);
            }
        }

        public void ToggleInventory()
        {
            if (InventoryWindow.gameObject.activeSelf)
            {
                ((Image)OpenInventoryButton.targetGraphic).sprite = m_ClosedInventorySprite;
                InventoryWindow.gameObject.SetActive(false);
                SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData(){ Clip = CloseInventoryClip});
            }
            else
            {
                ((Image)OpenInventoryButton.targetGraphic).sprite = m_OpenInventorySprite;
                InventoryWindow.gameObject.SetActive(true);
                InventoryWindow.Load(PlayerCharacter.Data);
                SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData(){ Clip = OpenInventoryClip});
            }
        }
    }
}