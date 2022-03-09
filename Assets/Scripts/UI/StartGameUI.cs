using System;
using System.Collections;
using DG.Tweening;
using Interfaces;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StartGameUI: MonoBehaviour, IFadeUI
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _StartText;
        [SerializeField] private bool _IsItStart;
        [SerializeField] private float _firstWait = 1f;
        [SerializeField] private float _secondWait = 5f;
        /*Условия победы:

        Условия поражения:*/

        private Tween _fadeTween;

        private void Start()
        {
            if (_IsItStart)
                StartCoroutine(ShowHidePanel());
        }

        public void ShowEndUI()
        {
            StartCoroutine(ShowAndStayPanel());
        }
        
        public void SetText(PhotonLogin.GameType gameType)
        {
            switch (gameType)
            {
                case PhotonLogin.GameType.COOP:
                    //_StartText.text =
                    break;
                case PhotonLogin.GameType.TwoTeams:
                    //_StartText.text =
                    break;
                case PhotonLogin.GameType.HateAll:
                    //_StartText.text =
                    break;
            }
            
        }
        
        public void FadeIn(float duration)
        {
            Fade(1f,duration, () =>
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            });
        }

        public void FadeOut(float duration)
        {
            Fade(0f,duration, () =>
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            });
        }
        
        private void Fade(float endValue, float duration, TweenCallback onEnd)
        {
            if (_fadeTween!=null)
            {
                _fadeTween.Kill(false);
            }

            _fadeTween = _canvasGroup.DOFade(endValue, duration);
            _fadeTween.onComplete += onEnd;
        }

        public IEnumerator ShowAndStayPanel()
        {
            yield return new WaitForSeconds(_firstWait);
            FadeIn(1f);
        }

        public IEnumerator ShowHidePanel()
        {
            yield return new WaitForSeconds(_firstWait);
            FadeIn(1f);
            yield return new WaitForSeconds(_secondWait);
            FadeOut(1f);
        }
    }
}