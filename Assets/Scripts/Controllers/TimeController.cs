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
                //CustomeValue = new ExitGames.Client.Photon.Hashtable();
                startTime = PhotonNetwork.Time;
                startTimer = true;
                ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
                prop.Add("Timer", timerIncrementValue);
                PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
            }
        }

        void Start()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["Timer"];
                startTimer = true;
            }
        }

        void Update()
        {
            if (!startTimer) return;
            if (PhotonNetwork.IsMasterClient)
            {
                timerIncrementValue = PhotonNetwork.Time - startTime;
                if (timerIncrementValue >= timer)
                {
                    TimeSpan time = TimeSpan.FromSeconds(timerIncrementValue);
                    _text.text = time.ToString(@"hh\:mm\:ss");
                    ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
                    prop.Add("Timer", timerIncrementValue);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
                    if (_dateTimeColapces != null)
                        if (epoxe < _dateTimeColapces.Count)
                            if (_dateTimeColapces[epoxe].Minute == time.Minutes)
                            {
                                GiveDateTime?.Invoke(epoxe);
                                epoxe++;
                            }
                }
            }
            else
            {
                timerIncrementValue = (double) PhotonNetwork.CurrentRoom.CustomProperties["Timer"];
                TimeSpan time = TimeSpan.FromSeconds(timerIncrementValue);
                _text.text = time.ToString(@"hh\:mm\:ss");
            }
        }
    }
}