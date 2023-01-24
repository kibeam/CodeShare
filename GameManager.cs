using System.Collections;
using UnityEngine;
using DG.Tweening;
using Util;
using Util.Usefull;
using UnityEngine.UI;
using TMPro;

namespace SPODY_3_6_4_UP
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;
        public static GameManager Instance { get { return instance; } set { instance = value; } }


        public IngameEnum IngameEnum;
        public Countdown countObject;

        private int totalScore;
        public int TotalScore
        {
            get
            { return totalScore; }
            set
            {
                totalScore = value;
                scoreText.text = totalScore.ToString();
            }
        }
        public Text scoreText;
        public void Awake()
        {
            if (instance == null)
                instance = this;
        }
        private void Start()
        {
            if (IngameEnum == IngameEnum.CountDown)
            {
                countObject.gameObject.SetActive(true);
            }
            StartCoroutine(GameSequence());
        }

        IEnumerator GameSequence()
        {

            Function.TouchEnable(false);
            yield return new WaitUntil(() => !countObject.IsPlay);
            IngameEnum = IngameEnum.Ingame;
            UpDownTimer.instance.isTimerOn = true;

            QuestionManager.Instance.ShuffleQuestion();

            while (IngameEnum != IngameEnum.Ending && UpDownTimer.instance.gameObject.activeSelf == true)
            {
                yield return null;
            }

            IngameEnum = IngameEnum.Ending;
            Winner.red_team_score = totalScore;
        }
    }
}