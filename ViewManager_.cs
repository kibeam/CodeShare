using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Util.Usefull;
using Sirenix.OdinInspector;
using SPODY;

namespace Util
{
    public struct Winner
    {
        public enum TEAM : int
        {
            RED_TEAM, BLUE_TEAM, BOTH_TEAM,
        }

        public static int red_team_score { get; set; }
        public static int blue_team_score { get; set; }
    }

    public enum ContentsValue
    {
        EduDefault, None, Dart
    }

    public class ViewManager_ : MonoBehaviour
    {
        public ContentsValue contentsValue;

        [Title("Touch Image")]
        public GameObject rootObject;
        public GameObject hitEffectImage;


        /// <summary>
        /// 2022년 3월 22일 -작성자 성기범
        /// 기존 뷰 매니저에 지장이 가지 않고 하트이펙트의 소리와 온오프를 위해 만든 변수
        [Title("Touch Image Info")]

        //0으로하면 터치 시 하트 이펙트가 안나옴 spody의 기본 값은 0.35
        public float scaleValue;

        public AudioClip changeClip;

        /// </summary>
        
        private PopupPanel_ PopupPanel_;
        private PopupPanel_ vs_Result_panel;

        [Title("If You Don't Use Vs\"Mode\", Just Keep Empty")]
        public PopupPanel_ vs_Result_panel_right;
        public PopupPanel_ vs_Result_Draw;

        [Title("Get Touch Coordinate")]
        public bool check_for_touch_coordinate = false;

        [Title("One / Two Camera")]
        public bool isOneCamera = true;
        [SerializeField]private Camera nowCam;
        [SerializeField]private Camera redCamera;
        [SerializeField]private Camera blueCamera;

        private ContentEndingSelector endingSelector;
        private void Awake()
        {
            GameManager.Instance.ViewManager_ = this;
            ContentsEffectScale();
        }

        public void ContentsEffectScale()
        {
            #region ContentsType
            switch (contentsValue)
            {
                case ContentsValue.EduDefault:
                    scaleValue = 0.35f;
                    break;
                case ContentsValue.None:
                    scaleValue = 0f;
                    break;
                case ContentsValue.Dart:
                    scaleValue = 1f;
                    break;
                default:
                    break;
            }
            #endregion
        } //Enum값에 해당하는 이펙트 스케일 변경

        private void OnEnable()
        {
            UIManager uiManager = GameObject.FindObjectOfType<UIManager>();

#if EDU
            PopupPanel_ = uiManager.singlePanelEdu;
            vs_Result_panel = uiManager.versusPanelEdu;
#elif EXP

            PopupPanel_ = uiManager.singlePanelExp;
            vs_Result_panel = uiManager.versusPanelExp;
#endif

        }

        private void Start()
        {
            Function.SensorReset();
            Function.SensorInit(new UnityAction<int, int>(BallClick)); //센서 초기화

            switch (isOneCamera)
            {
                case true:
                    nowCam = Camera.main;
                    break;
            }
        }

        public Ray ray;
        public RaycastHit hit;
        private void BallClick(int x, int y) //주요 터치 매서드
        {
            if (hitEffectImage == null || rootObject.activeSelf == false)
            {
                return;
            }

            var obj = Instantiate(hitEffectImage, rootObject.transform);

            obj.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            
            if(changeClip != null)
            {
                var objClip = obj.GetComponent<AudioSource>();
                objClip.clip = changeClip;
                objClip.Play();
            }

            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);

            switch (isOneCamera)
            {
                case true:
                    nowCam = Camera.main;
                    break;
                case false:

                    if (DeviceModelManager.Instance.IsRedCamera(rect.anchoredPosition.x))
                    {
                        nowCam = redCamera;
                    }
                    else
                    {
                        nowCam = blueCamera;
                    }
                    break;
            }
            
            ray = nowCam.ScreenPointToRay(rect.position);
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.CompareTag("Player"))
                {
                    //line.SetPosition(1, hit.transform.position);
                    Debug.Log(hit.collider.gameObject);
                    Debug.DrawRay(nowCam.transform.position, ray.direction * 100.0f, Color.red, 3.0f);

                    var charBehavior = hit.collider.GetComponent<CharacterBehavior>();

                    if (isOneCamera)
                    {
                        charBehavior.hitPoint = hit.point;
                    }
                    else
                    {
                        charBehavior.hitPoint = ray.direction;
                    }
                    
                    charBehavior.Touch();
                    
                    obj.SetActive(false);
                }
            }
        }
    }
}