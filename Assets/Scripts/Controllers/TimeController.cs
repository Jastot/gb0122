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
        //private DateTime date;
        private bool startTimer = false;
        private double timerIncrementValue;
        private double startTime;
        private int epoxe = 0;
        ExitGames.Client.Photon.Hashtable CustomeValue;
        public event Action<int> GiveDateTime;

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CustomeValue = new ExitGames.Client.Photon.Hashtable();
                startTime = PhotonNetwork.Time;
                startTimer = true;
                CustomeValue.Add("StartTime", startTime);
                PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            }
        }

        void Start()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
                startTimer = true;
            }
        }

        void Update()
        {
            if (!startTimer) return;
            timerIncrementValue = PhotonNetwork.Time - startTime;
            if (timerIncrementValue >= timer)
            {
                TimeSpan time = TimeSpan.FromSeconds(timerIncrementValue);
                
                /*var newMil =timerIncrementValue - (date.Hour*60*60+date.Minute*60+date.Second);
                if (newMil >= 0)
                    date = date.AddSeconds(newMil);*/
                _text.text = time.ToString(@"hh\:mm\:ss");
                if (_dateTimeColapces!=null)
                    if (epoxe<_dateTimeColapces.Count)
                        if (_dateTimeColapces[epoxe].Minute == time.Minutes)
                        {
                            GiveDateTime?.Invoke(epoxe);
                            epoxe++;
                        }
            }
        }
    }
}