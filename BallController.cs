using UnityEngine;

using Util;
using DG.Tweening;
using System;
using Util.Usefull;

namespace SPODY_3_6_4_UP
{
    public class BallController : CharacterBehavior
    {
        [Header("Elements")]
        public SpriteRenderer m_Renderer;
        public Transform m_Transform;
        public Sprite[] ballImage;

        public GameObject answerObj;
        public GameObject wrongObj;

        public int Index { get; set; }
        public bool IsAnswer { get; set; }

        public void OnEnable()
        {
            this.transform.DORestart();
        }
        public void INIT()
        {
            m_Renderer.sprite = ballImage[Index-1];
        }
        public override bool IsTouch { get; set; }
        public override bool IsAnim { get; set; }
        public override Vector3 hitPoint { get; set; }

        public override void Active(bool active)
        {
        }

        public override void Move(bool isPause)
        {
        }

        public override void PlayAnim(string trigger)
        {
        }

        public override void PlayClip()
        {
        }

        private bool isTouch = false;

        public override void Touch() //오브젝트가 터치 되었을 때
        {
            if (isTouch)
                return;

            isTouch = true;
            m_Transform.SetParent(null);
            if(Index == QuestionManager.Instance.AnswerIndex)
            {
                answerObj.SetActive(true);

                m_Transform.DOShakeScale(.5f, 1, 10, 90, true);

                m_Transform.DOScale(0f, 1f)
                    .SetEase(Ease.InBack)
                    .SetDelay(.5f)
                    .OnComplete(() =>
                    {
                        isTouch = false;
                        QuestionManager.Instance.SuccessAnswer();
                    });
            }
            else
            {
                wrongObj.SetActive(true);

                m_Transform.DOScale(0f, .5f)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(.5f)
                    .OnComplete(() =>
                    {
                        isTouch = false;
                        this.gameObject.SetActive(false);
                    });
            }
            
        }
        private void OnDisable()
        {
            answerObj.SetActive(false);
            wrongObj.SetActive(false);
        }
        public void Active(float delay)
        {
            this.gameObject.SetActive(true);

            m_Transform.localScale = Vector3.zero;

            m_Transform.DOScale(1f, .5f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(delay);

            m_Transform.DOLocalMoveY(m_Transform.localPosition.y + .2f, .8f)
                .SetDelay(delay)
                .SetEase(Ease.InOutFlash)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void DeActive()
        {
            m_Transform.DOScale(0f, .7f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    this.gameObject.SetActive(false);
                });
        }
    }
}