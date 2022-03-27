using System;
using System.Collections;
using Data;
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
        
        public void SetStartText(PhotonLogin.GameType gameType)
        {
            switch (gameType)
            {
                case PhotonLogin.GameType.COOP:
                    _StartText.text = "Условия победы:\n" +
                                      "Убить всех кактусов!\n" +
                                      "Условия поражения:\n"+
                                      "смерть от рук кактусов";
                    break;
                case PhotonLogin.GameType.TwoTeams:
                    _StartText.text = "Условия победы:\n" +
                                      "Убить всех кактусов!\n" +
                                      "Условия поражения:\n"+
                                      "Кол-во убитых кактусов вашей командой меньше,\n" +
                                      "чем кол-во убитых кактусов командой соперников\n" +
                                      "или\n" +
                                      "смерть от рук кактусов или противников";
                    break;
                case PhotonLogin.GameType.HateAll:
                    _StartText.text = "Условия победы:\n" +
                                      "Убить всех кактусов!\n" +
                                      "Условия поражения:\n"+
                                      "Кол-во убитых кактусов меньше,чем кол-во убитых кактусов соперниками\n" +
                                      "или\n" +
                                      "смерть от рук кактусов или противников";
                    break;
            }
            
        }

        public void SetEndText(MatchStatistics matchStatistics,
            PhotonLogin.GameType gameType)
        {
            string endFraze = "";
            switch (matchStatistics.WinOrLoose)
            {
                case GameController.PlayerState.Waiting:
                    endFraze = "Ожидайте исхода матча\n";
                    break;
                case GameController.PlayerState.Win:
                    endFraze = "ПОБЕДА!!!\n";
                    break;
                case GameController.PlayerState.Loose:
                    endFraze = "Поражение...\n";
                    break;
            }

            var teamColor = "";
            switch (matchStatistics.WinTeamColor)
            {
                case TeamColor.None:
                    break;
                case TeamColor.Red:
                    teamColor = "Красная команда";
                    break;
                case TeamColor.Blue:
                    teamColor = "Синяя команда";
                    break;
            }
            endFraze += teamColor;
            switch (gameType)
                    {
                        case PhotonLogin.GameType.COOP:
                            endFraze += "Статистика матча:\n" +
                                        $"Противников убито: {matchStatistics.KillEnemy}\n" +
                                        $"Опыта получено: {matchStatistics.Exp}";
                            break;
                        case PhotonLogin.GameType.TwoTeams:
                            endFraze += $"{teamColor} доминирует!:\n" +
                                        "Статистика матча:\n" +
                                        $"Противников убито: {matchStatistics.KillEnemy}\n" +
                                        $"Игроков убито: {matchStatistics.KillPlayers}\n" +
                                        $"Опыта получено: {matchStatistics.Exp}";
                            
                            break;
                        case PhotonLogin.GameType.HateAll:
                            endFraze += "Статистика матча:\n" +
                                        $"Противников убито: {matchStatistics.KillEnemy}\n" +
                                        $"Игроков убито: {matchStatistics.KillPlayers}\n" +
                                        $"Опыта получено: {matchStatistics.Exp}";
                            break;
                    }

            _StartText.text = endFraze;
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