using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Controllers
{
    public class TimeController: MonoBehaviour
    {
        [SerializeField] private double timer = 1;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private List<DateTime> _dateTimeColapces;
        private DateTime date;
        private bool startTimer = false;
        private double timerIncrementValue;
        private double startTime;
        private int epoxe = 0;
        ExitGames.Client.Photon.Hashtable CustomeValue;
        public event Action<int> GiveDateTime;
        
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CustomeValue = new ExitGames.Client.Photon.Hashtable();
                startTime = PhotonNetwork.Time;
                startTimer = true;
                CustomeValue.Add("StartTime", startTime);
                PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            }
            else
            {
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                startTimer = true;
            }
        }

        void Update()
        {
            if (!startTimer) return;
            timerIncrementValue = PhotonNetwork.Time - startTime;
            if (timerIncrementValue >= timer)
            {
                var newMil =timerIncrementValue - (date.Hour*60*60+date.Minute*60+date.Second);
                if (newMil >= 0)
                    date = date.AddSeconds(newMil);
                _text.text = date.ToLongTimeString();
                if (_dateTimeColapces!=null)
                    if (epoxe<_dateTimeColapces.Count)
                        if (_dateTimeColapces[epoxe].Minute == date.Minute)
                        {
                            GiveDateTime?.Invoke(epoxe);
                            epoxe++;
                        }
            }
        }
    }
}