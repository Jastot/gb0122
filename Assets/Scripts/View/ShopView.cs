using System;
using CreatorKitCode;
using UnityEngine;

namespace View
{
    public class ShopView: MonoBehaviour
    {
        [SerializeField] private float _economicCoefficient;
        [SerializeField] private string _shopID;
        public event Action<string,ShopView> ActivateStoreEvent;
        public event Action DeactivateStoreEvent;
        private CharacterData customer;
        private float IsCustomerDyingCoefficient = 1f;
       
        private void OnTriggerStay(Collider other)
        {
            Debug.Log("Stay");
            if (Input.GetKeyDown(KeyCode.E) && !customer)
            {
                Debug.Log("OpenShop");
                customer = other.GetComponent<CharacterData>();
                ActivateStore();
            }
        }

        private void ActivateStore()
        {
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