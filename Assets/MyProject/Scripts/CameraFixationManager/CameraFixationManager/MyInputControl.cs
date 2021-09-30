using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;

/// <summary>
/// 游戏输入全局控制，输入监听在这里捕获并再进一步转化到状态信息给事件监听器
/// </summary>
public class MyInputControl : MonoBehaviour , InputControl
{
    /// <summary>
    /// 按下事件
    /// </summary>
    public delegate void ButtonDown(bool isRight);
    /// <summary>
    /// 弹起事件
    /// </summary>
    public delegate void ButtonUp(bool isRight);
    /// <summary>
    /// 正在按事件
    /// </summary>
    public delegate void Buttoning();

    

    public ButtonDown ButtonDownCallBack;
   /// <summary>
   /// 按键弹起的回调
   /// </summary>
    public static Action<bool> ButtonUpCallBack;

    /// <summary>
    /// 按钮持续碰撞中的回调
    /// </summary>
    public Buttoning ButtoningCallBack;
    /// <summary>
    /// 鼠标或者蓝牙按键是否按下的状态  
    /// </summary>
    protected PressState PressState = PressState.None;
    /// <summary>
    /// 设置静态，这样就不会产生垃圾了  ，我们需要它来处理最后的输入逻辑判断
    /// </summary>
    protected static WaitForEndOfFrame _waitForEndOfFrame;
    /// <summary>
    /// 鼠标或者蓝牙按键是否按下的状态
    /// </summary>
    public PressState State
    {
        get { return PressState; }
    }

    private Coroutine _coroutine;
    private void Awake()
    {
    }

    protected  virtual   void Start()
    {
    }

    protected virtual void OnEnable()
    {
       
        _waitForEndOfFrame = new WaitForEndOfFrame();
        if(_coroutine!=null)StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(WaitForEndOfFramed());                             
    }
    /// <summary>
    /// 输入设置是在Update后检测  
    /// </summary>
    protected  virtual void Update()
    {
    }
    /// <summary>
    /// 整个工作循环结束后才执行的操作，这个方法在update，fixedUpdate,LateUpdate等操作完成之后才会执行，具体可查看unity的方法执行顺序
    /// 也就是说，按钮点击后的事件，比手柄的扳机键按下还要晚,这样的话，我们就可以从扳机键是否按下的消息，来给点击后的逻辑来作为判断
    /// </summary>
    /// <returns></returns>
    protected IEnumerator WaitForEndOfFramed()
    {
        while (true)
        {
            yield return _waitForEndOfFrame;
            //延迟执行状态，等待其他update方法查询
            //一个脚本周期结束后如果状态不是按下，或者持续按下中，状态都要强制转换为空状态，其实排除了ButtonDown，Buttoning，就只有ButtonUp状态了buttonUp是标记着按键按下的标记
            //
            if (PressState != PressState.ButtonDown && PressState != PressState.Buttoning)
                PressState = PressState.None;
        }
        // ReSharper disable once IteratorNeverReturns
    }

    protected virtual void OnDestroy()
    {
      
    }

    protected virtual void OnDisable()
    {
       
    }
}
