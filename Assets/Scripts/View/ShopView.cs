using System;
using System.Collections;
using CreatorKitCode;
using Unity.VisualScripting;
using UnityEngine;

namespace View
{
    public class ShopView: MonoBehaviour
    {
        [SerializeField] private float _economicCoefficient;
        [SerializeField] private string _shopID;
        public event Action<string,ShopView> ActivateStoreEvent;
        public event Action DeactivateStoreEvent;
        private CharacterData customer = null;
        private float IsCustomerDyingCoefficient = 1f;
        private Collider PlayersCollider = null;
        public bool IsActive=false;
        private bool ButtonIsBlocked = false;
        
        private void FixedUpdate()
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                if(!IsActive && !ButtonIsBlocked)
                {
                    if (customer == null && PlayersCollider )
                    {
                        StartCoroutine(BlockButton());
                        customer = PlayersCollider.GetComponent<CharacterData>();
                        IsActive = true;
                        ActivateStore();
                    }
                }
                else
                {
                    DeactivateStore();
                    IsActive = false;
                }
                
            }
        }

        private IEnumerator BlockButton()
        {
            ButtonIsBlocked = true;
            yield return new WaitForSeconds(3);
            ButtonIsBlocked = false;
        }

        private void OnTriggerStay(Collider other)
        {
            PlayersCollider = other;
        }

        private void ActivateStore()
        {
            CheckCustomerDying();
            ActivateStoreEvent?.Invoke(_shopID,this);
        }

        public void DeactivateStore()
        {
            customer = null;
            DeactivateStoreEvent?.Invoke();
        }

        public void SetEconomicCoefficient(int economicCoefficient)
        {
            _economicCoefficient = economicCoefficient;
        }

        private void CheckCustomerDying()
        { 
            if (customer.Stats.CurrentHealth * 100f / (float) customer.Stats.stats.health <= 80f) 
            {
                if (customer.Stats.CurrentHealth * 100f / (float) customer.Stats.stats.health <= 60f) 
                {
                    if (customer.Stats.CurrentHealth * 100f / (float) customer.Stats.stats.health <= 30f)
                    {
                        IsCustomerDyingCoefficient = 2.4f;
                        return;
                    }
                    IsCustomerDyingCoefficient = 2f;
                    return;
                }
                IsCustomerDyingCoefficient = 1.4f;
                return;
            }
            else
            {
                IsCustomerDyingCoefficient = 1f;
            }
        }

        public float GetIsCustomerDyingCoefficient()
        {
            return IsCustomerDyingCoefficient;
        }
    }
}