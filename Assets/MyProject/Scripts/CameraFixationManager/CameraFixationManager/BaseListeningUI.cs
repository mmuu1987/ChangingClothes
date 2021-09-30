using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraFixationManager;


/// <summary>
/// 有互动事件的UI
/// </summary>
public class BaseListeningUI : MonoBehaviour
{

    #region 定义的委托
    /// <summary>
    /// 射线进入
    /// </summary>
    public delegate void OnRayEnterDelegate();
    /// <summary>
    /// 射线正在改物体中
    /// </summary>
    public delegate void OnRayEnteringDelegate();
    /// <summary>
    /// 射线离开
    /// </summary>
    public delegate void OnRayLeaveDelegate();

    public delegate void OnClick();

    public delegate void OnClickObject(string name);
    /// <summary>
    /// 射线碰撞到物体后，并且按下回调
    /// </summary>
    /// <param name="name">碰撞到的物体的名字</param>
    public delegate void OnButtonDown();
    /// <summary>
    /// 监听到按键是否弹起，这个跟是否碰撞到物体无关,这里约束在这里，是让类分工明确
    /// </summary>
    public delegate void OnButtonUp();
#endregion

    public OnRayEnterDelegate OnRayEnter;

    public OnRayEnteringDelegate OnRayEntering;

    public OnRayLeaveDelegate OnRayLeave;
    /// <summary>
    /// 点击回调
    /// </summary>
    public Action OnRayClick;
    /// <summary>
    /// 点击带参数的回调
    /// </summary>
    public OnClickObject OnClikObject;
    /// <summary>
    /// 射线碰撞到物体后，并且按下回调
    /// </summary>
    public OnButtonDown OnBtnDown;
    /// <summary>
    /// 按键弹起的回调
    /// </summary>
    public OnButtonUp OnBtnUp;

    private Collider _collider;

    /// <summary>
    /// 左边进入，还是右边进入该UI
    /// </summary>
    private bool _isLeft;

    /// <summary>
    /// 是否松开左手
    /// </summary>
    private bool _isLeftRelease = true;

    /// <summary>
    /// 是否松开左手
    /// </summary>
    private bool _isRightRelease = true;
    // hand states
    private KinectInterop.HandState _leftHandState = KinectInterop.HandState.Unknown;
    private KinectInterop.HandState _rightHandState = KinectInterop.HandState.Unknown;

    /// <summary>
    /// 停留UI的计时缓存
    /// </summary>
    private float _timeTempLeft = 0f;

    /// <summary>
    /// 停留UI的计时缓存
    /// </summary>
    private float _timeTemRight = 0f;

    /// <summary>
    /// 点击进度
    /// </summary>
    public event Action<bool,float> ClickProgress; 

    protected virtual void Start()
    {
     

      


       // Debug.Log("注册消息了");
        //注册消息 
        MyInternaction.Instance.RayEnter += Enter;

        MyInternaction.Instance.RayEntering += Entering;

        MyInternaction.Instance.RayLeave += Leave;

        

    }
	/// <summary>
	/// 射线进入
	/// </summary>
    private void Enter(GameObject go,bool isLeft)
    {
        if (go != this.gameObject) return;

        _timeTempLeft = 0f;
        _timeTemRight = 0f;
        // Debug.Log("进入了该UI " +go.name);
        _isLeft = isLeft;

        KinectManager kinectManager = KinectManager.Instance;

       
        int playerUserID = (int)kinectManager.GetUserIdByIndex(0);


        _leftHandState = kinectManager.GetLeftHandState(playerUserID);

        _rightHandState = kinectManager.GetRightHandState(playerUserID);


        InteractionManager.HandEventType handEventLeft = InteractionManager.HandStateToEvent(_leftHandState, InteractionManager.HandEventType.Release);

        InteractionManager.HandEventType handEventRight = InteractionManager.HandStateToEvent(_rightHandState, InteractionManager.HandEventType.Release);

        if (isLeft)
        {
            if (handEventLeft == InteractionManager.HandEventType.Release)
                _isLeftRelease = true;
            else _isLeftRelease = false;
        }
        else
        {
            if (handEventRight == InteractionManager.HandEventType.Release)
                _isRightRelease = true;
            else _isRightRelease = false;
        }

        if (OnRayEnter != null)
            OnRayEnter();
        RayEnter(go);
    }
    /// <summary>
    /// 射线进入中
    /// </summary>
    private void Entering(GameObject go, bool isLeft)
    {
        if (go != this.gameObject) return;
       // Debug.Log("持续   进入了该UI " + go.name);
        _isLeft = isLeft;

        KinectManager kinectManager = KinectManager.Instance;

        // update Kinect interaction
        if (kinectManager && kinectManager.IsInitialized())
        {
            long playerUserID = kinectManager.GetUserIdByIndex(0);


           
            if (playerUserID != 0)
            {
                _leftHandState = kinectManager.GetLeftHandState(playerUserID);

                _rightHandState = kinectManager.GetRightHandState(playerUserID);


                InteractionManager.HandEventType handEventLeft = InteractionManager.HandStateToEvent(_leftHandState, InteractionManager.HandEventType.Release);

                InteractionManager.HandEventType handEventRight = InteractionManager.HandStateToEvent(_rightHandState, InteractionManager.HandEventType.Release);

                if (_isLeft)
                {

                    _timeTempLeft += Time.deltaTime;

                    if (_timeTempLeft >= GlobalSettings.ClickTime)
                    {
                        if (OnRayClick != null)
                            OnRayClick();
                        _timeTempLeft = 0f;
                        Debug.Log("左手点击了该UI " + go.name);
                    }

                    float progress = _timeTempLeft / GlobalSettings.ClickTime;
                  
                    MyInternaction.Instance.HandleProgress(true,progress);
                    if (handEventLeft == InteractionManager.HandEventType.Grip && _isLeftRelease)
                    {
                        //if (OnRayClick != null)
                        //    OnRayClick();
                        //if (OnClikObject != null)
                        //    OnClikObject(go.name);
                        //_isLeftRelease = false;

                        //Debug.Log("左手点击了该UI " +go.name);

                    }
                    else if (handEventLeft == InteractionManager.HandEventType.Release)
                    {
                        _isLeftRelease = true;
                    }

                }
                else
                {

                    _timeTemRight += Time.deltaTime;

                    if (_timeTemRight >= GlobalSettings.ClickTime)
                    {
                        if (OnRayClick != null)
                            OnRayClick();
                        _timeTemRight = 0f;
                        Debug.Log("右手点击了该UI " + go.name);
                    }
                    float progress = _timeTemRight / GlobalSettings.ClickTime;
                    MyInternaction.Instance.HandleProgress(false, progress);
                  

                    if (handEventRight == InteractionManager.HandEventType.Grip && _isRightRelease)
                    {
                        //if (OnRayClick != null)
                        //    OnRayClick();
                        //if (OnClikObject != null)
                        //    OnClikObject(go.name);
                        //_isRightRelease = false;

                        //Debug.Log("右手点击了该UI " + go.name);

                    }
                    else if (handEventRight == InteractionManager.HandEventType.Release)
                    {
                        _isRightRelease = true;
                    }
                }


                //if (HTCHandleInputControl.HtcHandleInputControRight == null) return;
                //// Debug.Log("输入的状态是：" + HTCHandleInputControl.HtcHandleInputControRight.State);

                //if (HTCHandleInputControl.HtcHandleInputControRight.State == PressState.ButtonUp)//弹起才算是一个点击操作,这里从输入控制中查询是否弹起
                //{
                //    //执行点击事件
                //    //  Debug.Log("你点击了该" + this.name + "物体");
                //    if (OnRayClick != null)
                //        OnRayClick();
                //    if (OnClikObject != null)
                //        OnClikObject(go.name);

                //}
                //if (HTCHandleInputControl.HtcHandleInputControRight.State == PressState.ButtonDown)//当按键按下的时候  
                //{
                //    if (OnBtnDown != null)
                //        OnBtnDown();
                //}
            }
        }

       
        if (OnRayEntering != null)
            OnRayEntering();
        RayEntering(go);
    }
    /// <summary>
    ///  射线离开
    /// </summary>
    private void Leave(GameObject go, bool isLeft)
    {
        if (go != this.gameObject) return;

       // Debug.Log("离开了   该UI " + go.name);
        if (OnRayLeave != null)
            OnRayLeave();
        RayLeave(go);
        MyInternaction.Instance.ExitUI();
        _timeTempLeft = 0f;
        _timeTemRight = 0f;
    }
    protected virtual void RayEnter(GameObject go)
    {
    }
    /// <summary>
    /// 射线正在射中中  
    /// </summary>
    protected virtual void RayEntering(GameObject go)
    {
    }
    /// <summary>
    /// 射线离开
    /// </summary>
    protected virtual void RayLeave(GameObject go)
    {
    }


    private void ButtonUp(bool isRight)
    {
        if (!isRight) return;
       // Debug.Log("鼠标弹起");
        if (OnBtnUp != null)
            OnBtnUp();
    }

    private void OnDestroy()
    {
        //注册消息
        //Debug.Log("注销了" + name + "物体上的碰撞消息");
        if (MyInternaction.Instance != null)
        {
            MyInternaction.Instance.RayEnter -= Enter;

            MyInternaction.Instance.RayEntering -= Entering;

            MyInternaction.Instance.RayLeave -= Leave;
        }
     
    }

    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
        //注册消息
       // Debug.Log("注销消息了");
        //if (HTCHandleRayEvent.Instance != null)
        //{
        //    HTCHandleRayEvent.Instance.RayEnter -= Enter;

        //    HTCHandleRayEvent.Instance.RayEntering -= Entering;

        //    HTCHandleRayEvent.Instance.RayLeave -= Leave;
        //}
       
    }
}
