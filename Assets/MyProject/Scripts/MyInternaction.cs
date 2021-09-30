using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InteractionManager;

public class MyInternaction : MonoBehaviour
{

    public static MyInternaction Instance;

    List<RaycastResult> list;

    public Image MouseLeftImage;

    public Image MouseRightImage;

    public EventSystem _mEventSystem;
    public GraphicRaycaster gra;

    public Camera mainCamera;


    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    // tracked userId
    private long playerUserID = 0;

    // hand states
    private KinectInterop.HandState _leftHandState = KinectInterop.HandState.Unknown;
    private KinectInterop.HandState _rightHandState = KinectInterop.HandState.Unknown;


    private HandEventType _leftHandStateTemp = HandEventType.None;
    private HandEventType _rightHandStateTemp = HandEventType.None;


    public Action<GameObject, bool> RayEnter;



    public Action<GameObject, bool> RayEntering;

    public Action<GameObject, bool> RayLeave;

    private RectTransform _leftTrigger;

    private RectTransform _rightTrigger;

    private void Awake()
    {
        if (Instance != null) throw new UnityException("已经有单例");

        Instance = this;
    }



    void Start()
    {
        list = new List<RaycastResult>();
    }
    void Update()
    {


        KinectManager kinectManager = KinectManager.Instance;

        // update Kinect interaction
        if (kinectManager && kinectManager.IsInitialized())
        {
            playerUserID = kinectManager.GetUserIdByIndex(playerIndex);


            if (playerUserID != 0)
            {

                list = GraphicRaycaster(MouseLeftImage.rectTransform.anchoredPosition);

                list.Sort(((result, raycastResult) => result.depth.CompareTo(raycastResult.depth)));

                RectTransform tfLeft = null;
                foreach (RaycastResult result in list)
                {
                    var BaseListening = result.gameObject.GetComponent<BaseListeningUI>();

                    if (BaseListening != null)
                    {
                        tfLeft = BaseListening.GetComponent<RectTransform>();
                        break;
                    }
                }
                RayLeftLoginc(tfLeft);




                list = GraphicRaycaster(MouseRightImage.rectTransform.anchoredPosition);

                list.Sort(((result, raycastResult) => result.depth.CompareTo(raycastResult.depth)));

                RectTransform tfRight = null;
                foreach (RaycastResult result in list)
                {
                    var BaseListening = result.gameObject.GetComponent<BaseListeningUI>();

                    if (BaseListening != null)
                    {
                        tfRight = BaseListening.GetComponent<RectTransform>();
                        break;
                    }
                }

                RayRightLoginc(tfRight);
            }
        }

        UpdateUi();
    }

    private void RayLeftLoginc(RectTransform rectTransform)
    {
        if (rectTransform != null)
        {
            //被碰撞的物体第一次进入触发回调,第二个逻辑是射线从一个物体移动到另外一个物体的判断
            if (_leftTrigger == null || (_leftTrigger != rectTransform && _leftTrigger != null))
            {
                if (RayEnter != null)
                    RayEnter(rectTransform.gameObject, true);



                Debug.Log("左手进入了" + rectTransform.name);

                if (_leftTrigger != null)
                {
                    if (RayLeave != null)
                        RayLeave(_leftTrigger.gameObject, true);
                    //Debug.Log("左手离开了" + _leftTrigger.name);
                }
                _leftTrigger = rectTransform;
            }
            else if (_leftTrigger == rectTransform)//在进入物体的第二帧以上
            {
                if (RayEntering != null)
                    RayEntering(rectTransform.gameObject, true);
                //Debug.Log("左手持续碰撞物体：" + _leftTrigger.name);
                _leftTrigger = rectTransform;
            }
        }
        else
        {
            if (_leftTrigger != null)//物体离开回调
            {
                if (RayLeave != null)
                    RayLeave(_leftTrigger.gameObject, true);
                //Debug.Log("离开了" + _leftTrigger.name);
            }
            _leftTrigger = null;
            if(_isEnableHand)
             HandleProgress(true, 1f);
        }

    }


    private void SetMouseImageAlpha(float alpha)
    {
        MouseRightImage.color = new Color(1f, 1f, 1f, alpha);
        MouseLeftImage.color = new Color(1f, 1f, 1f, alpha);
        MouseRightImage.transform.Find("hand").GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        MouseLeftImage.transform.Find("hand").GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);

        //if (alpha >= 1f)
        //{
        //    MouseLeftImage.fillAmount = 0f;
        //    MouseRightImage.fillAmount = 0f;
        //}
       
    }
    public void ExitUI()
    {
        if (!_isEnableHand)
        {
            if (_isShowProgress)
            {
                SetMouseImageAlpha(0.1f);
            }
        }
    }
    private void RayRightLoginc(RectTransform rectTransform)
    {
        if (rectTransform != null)
        {
            //被碰撞的物体第一次进入触发回调,第二个逻辑是射线从一个物体移动到另外一个物体的判断
            if (_rightTrigger == null || (_rightTrigger != rectTransform && _rightTrigger != null))
            {
                if (RayEnter != null)
                    RayEnter(rectTransform.gameObject, false);


                //   Debug.Log("进入了" + hit.transform.name);

                if (_rightTrigger != null)
                {
                    if (RayLeave != null)
                        RayLeave(_rightTrigger.gameObject, false);
                    //   Debug.Log("离开了" + _transform.name);
                }
                _rightTrigger = rectTransform;
            }
            else if (_rightTrigger == rectTransform)//在进入物体的第二帧以上
            {
                if (RayEntering != null)
                    RayEntering(rectTransform.gameObject, false);
                // Debug.Log("持续碰撞物体：" + hit.transform.name);
                _rightTrigger = rectTransform;
            }
        }
        else
        {
            if (_rightTrigger != null)//物体离开回调
            {
                if (RayLeave != null)
                    RayLeave(_rightTrigger.gameObject, false);
                // Debug.Log("离开了" + _transform.name);
            }
            _rightTrigger = null;
            if (_isEnableHand)
                HandleProgress(false, 1f);
        }

    }
    private void UpdateUi()
    {
        KinectManager kinectManager = KinectManager.Instance;
        if (kinectManager && kinectManager.IsInitialized())
        {
            playerUserID = kinectManager.GetUserIdByIndex(playerIndex);

            if (playerUserID != 0)
            {
                playerUserID = KinectManager.Instance.GetUserIdByIndex(playerIndex);

                Rect backgroundRect = this.mainCamera.pixelRect;
                PortraitBackground portraitBack = PortraitBackground.Instance;
                bool flag = portraitBack && portraitBack.enabled;

                if (flag)
                {
                    backgroundRect = portraitBack.GetBackgroundRect();
                }

                Vector3 leftpos = KinectManager.Instance.GetJointPosColorOverlay(playerUserID, (int)KinectInterop.JointType.HandLeft, this.mainCamera, backgroundRect);
                Vector3 screenPosLeft = this.mainCamera.WorldToScreenPoint(leftpos);

                Vector3 rightPos = KinectManager.Instance.GetJointPosColorOverlay(playerUserID, (int)KinectInterop.JointType.HandRight, this.mainCamera, backgroundRect);
                Vector3 screenPosRight = this.mainCamera.WorldToScreenPoint(rightPos);




                MouseLeftImage.rectTransform.anchoredPosition = screenPosLeft;
                MouseRightImage.rectTransform.anchoredPosition = screenPosRight;
            }
        }


    }

    /// <summary>
    /// 是否可以显示进度
    /// </summary>
    private bool _isShowProgress;
    /// <summary>
    /// 手部UI是否可用
    /// </summary>
    private bool _isEnableHand;
    /// <summary>
    /// 是否显示手部UI
    /// </summary>
    public void EnableHand(bool isEnable,float alpha, bool isShowProgress = true)
    {

        _isShowProgress = isShowProgress;
        _isEnableHand = isEnable;

        
            SetMouseImageAlpha(alpha);
       
        // MouseLeftImage.gameObject.SetActive(isEnable);
       // MouseRightImage.gameObject.SetActive(isEnable);


    }
    public void HandleProgress(bool isleft, float progress)
    {

        if (_isShowProgress)
        {
           SetMouseImageAlpha(1f);

            if (isleft)
            {
                MouseLeftImage.fillAmount = progress;
            }
            else
            {
                MouseRightImage.fillAmount = progress;
            }

            if (progress >= 1f)
            {
                if (!_isEnableHand)
                {
                    SetMouseImageAlpha(0.1f);
                }
            }


        }

    }




    private List<RaycastResult> GraphicRaycaster(Vector2 pos)
    {
        var mPointerEventData = new PointerEventData(_mEventSystem);
        mPointerEventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();

        gra.Raycast(mPointerEventData, results);
        return results;
    }
}
