﻿using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class StartGameUI: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        private Tween _fadeTween;

        private void Start()
        {
            StartCoroutine(StartGameShowPanel());
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

        private IEnumerator StartGameShowPanel()
        {
            yield return new WaitForSeconds(1f);
            FadeIn(1f);
            yield return new WaitForSeconds(5f);
            FadeOut(1f);
        }
    }
}