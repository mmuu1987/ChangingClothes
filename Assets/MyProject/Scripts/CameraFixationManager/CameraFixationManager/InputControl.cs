using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 鼠标或者蓝牙的按钮是否按下的状态，目前只设定支持一个
/// </summary>
public enum PressState : int
{
    /// <summary>
    /// 没有按键状态
    /// </summary>
    None,
    /// <summary>
    /// 按下状态
    /// </summary>
    ButtonDown,
    /// <summary>
    /// 弹起状态
    /// </summary>
    ButtonUp,
    /// <summary>
    /// 按下状态中
    /// </summary>
    Buttoning
}
public interface InputControl
{

    /// <summary>
    /// 监听器只监听这个属性就好
    /// </summary>
    PressState State
    {
        get;
    }

}
