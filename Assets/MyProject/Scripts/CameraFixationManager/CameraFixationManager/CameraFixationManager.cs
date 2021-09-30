using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Time:2017-4-27
 * author:韦卓升
 * 相机注视管理，适用与相机注视事件，如相机焦点进入某物体，在某物体中，离开某物体
 * 用处：用于处理射线碰撞的事件，可以移植到大多数VR SDK上，因为每个平台都封装有了自己的注视管理，但这个引擎自身的拓展开发，可以一次学习
 * 多个VR SDK部署，如果某物体想监听到该类所发出的消息，必须挂载BaseListeningUI基类脚本
 * 我这里设置物体的Layer可碰撞是在第5层
 * time:2017-05-24
 * 修改消息机制，把SendMessage 改成用委托
 * time:2017-06-27
 * 优化碰撞遮罩层
 * time:2017-08-21
 * 修改了射线生成，把射线抽象出来，提供抽象方法，让其他继承它的类实现，这样的作用是可以把相机注视拆开出来，然后可以用HTC的手柄来实现注视
 * 并把碰撞点坐标暴露出来,让继承该类的实例能够拿到信息进一步处理
 * 
 */
namespace CameraFixationManager
{
    public abstract class CameraFixationManager : MonoBehaviour
    {
        /// <summary>
        /// 射线是否撞到了有触发事件的碰撞体
        /// </summary>
        public static bool IsImpactCollider = false;
        /// <summary>
        /// 撞击点的位置
        /// </summary>
        public Vector3 HitPosition = Vector3.zero;
        /// <summary>
        /// 射线可以碰撞到的层
        /// </summary>
        public int RayColliderLayer = 5;

        #region 事件回调
        /// <summary>
        /// 射线进入回调
        /// </summary>
        public Action<GameObject> RayEnter;
        /// <summary>
        /// 射线离开的回调
        /// </summary>
        public Action<GameObject> RayLeave;
        /// <summary>
        /// 射线正在射中的回调
        /// </summary>
        public Action<GameObject> RayEntering;
        #endregion


        /// <summary>
        /// 响应的标签层，所有的交互事件都需要放到该层
        /// </summary>
        public string ResponseTag;
        /// <summary>
        /// 是否播放音乐特效
        /// </summary>
        public bool IsPlayAudio = true;
        /// <summary>
        /// 射线缓冲，意思就是射线当前碰撞套的物体
        /// </summary>
        private Transform _transform = null;
        /// <summary>
        /// 是否碰撞到
        /// </summary>
        protected bool IsCollider = true;
       
        
       protected   virtual void Awake()
        {
           
        }
        /// <summary>
        /// 放在延迟更新是因为，输入事件一般是在Update之前，所以，处理完输入逻辑后，在 Update里判断输入状态，再在延迟更新里判断碰撞状态，这样就不用手动
        /// edit/ProjectSettings/ScriptExecutionOrder的编排脚本的方法执行顺序  
        /// </summary>
        void LateUpdate()
        {
            if (!IsCollider) return;
            RaycastHit hit;
            //只能向屏幕中点发射线
            var ray = GetRay();//这里进一步抽象,因为有相机注视，还有手柄射线注视

           if (Physics.Raycast(ray, out hit,2000f,1<< RayColliderLayer))//这里把5层设置为可碰撞层,其他层都要忽略掉   
            {
                //  Debug.Log("碰撞到的物体名字是：" + hit.transform.name);
                HitPosition = hit.point;//碰撞点事世界位置，切记   切记

                if (!hit.transform.CompareTag(ResponseTag) )//如果不属于ResponseTag标签，则不进行消息触发
                {
                    IsImpactCollider = false;
                   

                    if (_transform != null)//物体离开回调
                    {
                        if (RayLeave != null)
                            RayLeave(_transform.gameObject);
                     //   Debug.Log("离开了" + _transform.name);
                    }
                    _transform = null;

                    return;
                }
                IsImpactCollider = true;
                //被碰撞的物体第一次进入触发回调,第二个逻辑是射线从一个物体移动到另外一个物体的判断
                if (_transform == null || (_transform != hit.transform && _transform != null))
                {
                    if (RayEnter != null)
                        RayEnter(hit.transform.gameObject);
                   
                  
                 //   Debug.Log("进入了" + hit.transform.name);

                    if(_transform!=null)
                    {
                        if (RayLeave != null)
                            RayLeave(_transform.gameObject);
                     //   Debug.Log("离开了" + _transform.name);
                    }
                    _transform = hit.transform;
                }
                else if(_transform == hit.transform)//在进入物体的第二帧以上
                {
                    if (RayEntering != null)
                        RayEntering(hit.transform.gameObject);
                   // Debug.Log("持续碰撞物体：" + hit.transform.name);
                    _transform = hit.transform;
                }
            }
            else
            {
                IsImpactCollider = false;

                HitPosition = Vector3.zero;

                 if ( _transform!= null)//物体离开回调
                {
                    if (RayLeave != null)
                        RayLeave(_transform.gameObject);
                   // Debug.Log("离开了" + _transform.name);
                }
                _transform = null;
            }
        }

        protected abstract Ray GetRay();

        

        protected virtual void OnDestroy()
        {
            
        }
    }
}

