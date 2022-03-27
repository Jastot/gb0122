using System;
using System.Collections.Generic;
using System.Linq;
using CreatorKitCode;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CreatorKitCodeInternal {
    public class SimpleEnemyController : MonoBehaviourPun, 
        AnimationControllerDispatcher.IAttackFrameReceiver,
        AnimationControllerDispatcher.IFootstepFrameReceiver
    {
        public enum State
        {
            IDLE,
            PURSUING,
            ATTACKING
        }

        private List<CharacterControl> _playersInGame;
        private CharacterData _playerData = null;
        private CharacterControl _playerPosition = null;
        public float Speed = 6.0f;
        public float detectionRadius = 10.0f;

        public AudioClip[] SpottedAudioClip;

        Vector3 m_StartingAnchor;
        Animator m_Animator;
        NavMeshAgent m_Agent;
        CharacterData m_CharacterData;

        CharacterAudio m_CharacterAudio;
        int m_SpeedAnimHash;
        int m_AttackAnimHash;
        int m_DeathAnimHash;
        int m_HitAnimHash;
        bool m_Pursuing;
        float m_PursuitTimer = 0.0f;

        State m_State;

        LootSpawner m_LootSpawner;
    
        // Start is called before the first frame update
        void Start()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_Agent = GetComponent<NavMeshAgent>();
            
            m_SpeedAnimHash = Animator.StringToHash("Speed");
            m_AttackAnimHash = Animator.StringToHash("Attack");
            m_DeathAnimHash = Animator.StringToHash("Death");
            m_HitAnimHash = Animator.StringToHash("Hit");

            m_CharacterData = GetComponent<CharacterData>();
            m_CharacterData.Init();

            m_CharacterAudio = GetComponentInChildren<CharacterAudio>();
        
            m_CharacterData.OnDamage += () =>
            {
                m_Animator.SetTrigger(m_HitAnimHash);
                m_CharacterAudio.Hit(transform.position);
                GetComponent<PhotonView>().RPC("SyncHP", RpcTarget.OthersBuffered, m_CharacterData.Stats.CurrentHealth);
            };
        
            m_Agent.speed = Speed;

            m_LootSpawner = GetComponent<LootSpawner>();
        
            m_StartingAnchor = transform.position;
            _playersInGame = FindObjectsOfType<MonoBehaviour>().OfType<CharacterControl>().ToList();
        }

        [PunRPC]
        public void DeleteSomeCharacter(int id)
        {
            foreach (var playerInGame in _playersInGame)
            {
                if (playerInGame.photonView.ViewID == id)
                {
                    _playersInGame.Remove(playerInGame);
                }
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            if (m_CharacterData.Stats.CurrentHealth == 0)
            {
                m_Animator.SetTrigger(m_DeathAnimHash);
            
                m_CharacterAudio.Death(transform.position);
                m_CharacterData.Death();
            
                if(m_LootSpawner != null)
                    m_LootSpawner.SpawnLoot();
                Destroy(m_Agent);
                Destroy(GetComponent<Collider>());
                Destroy(this);
                return;
            }
            switch (m_State)
            {
                case State.IDLE:
                {
                    foreach (var characterControl in _playersInGame)
                    {
                        if (Vector3.SqrMagnitude(characterControl.gameObject.transform.position -transform.position) <
                            detectionRadius * detectionRadius)
                        {
                            if (SpottedAudioClip.Length != 0)
                            {
                                SFXManager.PlaySound(SFXManager.Use.Enemies, new SFXManager.PlayData()
                                {
                                    Clip = SpottedAudioClip[Random.Range(0, SpottedAudioClip.Length)],
                                    Position = transform.position
                                });
                            }

                            m_PursuitTimer = 4.0f;
                            m_State = State.PURSUING;
                            m_Agent.isStopped = false;
                            _playerPosition = characterControl;
                            _playerData = characterControl.Data;
                            break;
                        }
                    }
                }
                    break;
                case State.PURSUING:
                {
                    float distToPlayer = Vector3.SqrMagnitude(_playerPosition.gameObject.transform.position - transform.position);
                    if (distToPlayer < detectionRadius * detectionRadius)
                    {
                        m_PursuitTimer = 4.0f;

                        if (m_CharacterData.CanAttackTarget(_playerData))
                        {
                            m_CharacterData.AttackTriggered();
                            m_Animator.SetTrigger(m_AttackAnimHash);
                            m_State = State.ATTACKING;
                            m_Agent.ResetPath();
                            m_Agent.velocity = Vector3.zero;
                            m_Agent.isStopped = true;
                        }
                    }
                    else
                    {
                        if (m_PursuitTimer > 0.0f)
                        {
                            m_PursuitTimer -= Time.deltaTime;

                            if (m_PursuitTimer <= 0.0f)
                            {
                                m_Agent.SetDestination(m_StartingAnchor);
                                m_State = State.IDLE;
                            }
                        }
                    }
                
                    if (m_PursuitTimer > 0)
                    {
                        m_Agent.SetDestination(_playerPosition.gameObject.transform.position);
                    }
                }
                    break;
                case State.ATTACKING:
                {
                    if (!m_CharacterData.CanAttackReach(_playerData))
                    {
                        m_State = State.PURSUING;
                        m_Agent.isStopped = false;
                    }
                    else
                    {
                        if (m_CharacterData.CanAttackTarget(_playerData))
                        {
                            m_CharacterData.AttackTriggered();
                            m_Animator.SetTrigger(m_AttackAnimHash);
                        }
                    }
                }
                    break;
            }
        
            m_Animator.SetFloat(m_SpeedAnimHash, m_Agent.velocity.magnitude/Speed);
        }

        public void AttackFrame()
        {
            //if we can't reach the player anymore when it's time to damage, then that attack miss.
            if (!m_CharacterData.CanAttackReach(_playerData))
                return;
            
            m_CharacterData.Attack(_playerData);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        public void FootstepFrame()
        {
            Vector3 pos = transform.position;
        
            m_CharacterAudio.Step(pos);
            VFXManager.PlayVFX(VFXType.StepPuff, pos); 
        }
    }
}