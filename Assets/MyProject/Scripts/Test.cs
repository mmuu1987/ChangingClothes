using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public GameObject tipGameObject;

    public Camera mainCamera;

    private Rect _backgroundRect;

    public RawImage ShotHeadImage;

    private Texture2D screenShot;

    public float ShotWidth = 300;

    public float ShotHeight = 350;
    // Start is called before the first frame update
    void Start()
    {
        tipGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        tipGameObject.transform.localScale = Vector3.one * 0.1f;

       // KinectManager.Instance.AddingUserEvent += AddingUserEvent;

        
    }

    private Coroutine _coroutine;
    private IEnumerator WaitTime(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        if (action != null) action();
    }
    private void AddingUserEvent(long obj)
    {
        
        
        _backgroundRect = mainCamera.pixelRect;
        PortraitBackground portraitBack = PortraitBackground.Instance;

        if (portraitBack && portraitBack.enabled)
        {
            _backgroundRect = portraitBack.GetBackgroundRect();
        }

        Vector3 pos = KinectManager.Instance.GetJointPosColorOverlay(obj, (int)KinectInterop.JointType.Head, mainCamera, _backgroundRect);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(pos);

         Debug.Log("screenPos  " + screenPos + "   Screen.width is" + Screen.width);

        Rect rect = Rect.zero;
        CheckBodyHeight();


        if (ShotHeadImage == null) return;

        //如果不满足截取头像情况,隔开一定的时间再次截图  h1t1t1p1s1:1/1/1t1o1u1h1o2u2.2t2e2l1/1u1s1e1r1
        if (screenPos.x + ShotWidth / 2f > Screen.width || screenPos.y + ShotHeight / 2f > Screen.height)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(WaitTime(1f, (() =>
            {
                AddingUserEvent(obj);
            })));
            Debug.Log("重新开始截图头像");
        }
        else
        {
            rect = new Rect(screenPos.x - ShotWidth / 2f, screenPos.y - ShotHeight / 2f, ShotWidth, ShotHeight);
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(CaptureScreenshot2(rect));
        }


    }
    /// <summary>  
    /// Captures the screenshot2.  
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="rect">Rect.截图的区域，左下角为o点</param>  
    IEnumerator CaptureScreenshot2(Rect rect)
    {
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        // 读取屏幕像素信息并存储为纹理数据，  
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        ShotHeadImage.texture = screenShot;
        ShotHeadImage.SetNativeSize();


        // 然后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Screenshot.png";

        
        //Thread t = new Thread((() =>
        //{
        //    System.IO.File.WriteAllBytes(filename, bytes);
        //}));
        //t.Start();
        if (_coroutine!=null)StopCoroutine(_coroutine);
        // Debug.Log(string.Format("截屏了一张图片: {0}", filename));
    }


    public Camera captureCamera;
    /// <summary>
    /// 对相机进行截图
    /// </summary>
    public void StartCameraCapture()
    {
        StartCoroutine(CaptureCamera(captureCamera,new Rect(0f,0f,1080f,1920f)));
    }

    /// <summary>  
    /// 对相机截图。   
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    IEnumerator  CaptureCamera(Camera camera, Rect rect)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
        //ps: camera2.targetTexture = rt;  
        //ps: camera2.Render();  
        //ps: -------------------------------------------------------------------  

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        
    }


    /// <summary>  
    /// 测试发送到服务器
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="rect">Rect.截图的区域，左下角为o点</param>  
    IEnumerator PostServer()
    {
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        Texture2D screenShot = new Texture2D(Screen.width, (int)Screen.height, TextureFormat.RGB24, false);

        yield return new WaitForEndOfFrame();
        // 读取屏幕像素信息并存储为纹理数据，  
        screenShot.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();



        yield return StartCoroutine(NetManager.Instance.PostPictureToServer(screenShot));



        // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  

    }
    /// <summary>
    /// 检测用户范围，如果超过，则停止用户的跟踪
    /// </summary>
    /// <param name="x">距离屏幕中心点的x轴的现实中心点的距离</param>
    /// <param name="y">距离屏幕中心点的y轴的现实的距离</param>
    /// <param name="z">距离屏幕中心点的z轴的现实中距离</param>
    private void CheckUserRange(float x, float y, float z)
    {
        if (KinectManager.Instance.GetAllUserIds().Count > 0)
        {
            List<long> ids = KinectManager.Instance.GetAllUserIds();

            foreach (long id in ids)
            {
                Vector3 pos = KinectManager.Instance.GetUserPosition(id);

                 Debug.Log(pos);//(0.1, 0.5, 2.2)

                if (Mathf.Abs(pos.x) > x) KinectManager.Instance.ClearKinectUsers();
                //else if (pos.z > z || pos.z < z - 1.75f) KinectManager.Instance.ClearKinectUsers();
                else if (pos.y < y || pos.y > 2 + y) KinectManager.Instance.ClearKinectUsers();
                // 获取左手位置
                // Debug.Log("left hand is " + KinectManager.Instance.GetJointKinectPosition(id, KinectManager.Instance.GetJointIndex(KinectInterop.JointType.HandLeft)));
                // 获取右手位置
                // Debug.Log("right hand is " + KinectManager.Instance.GetJointKinectPosition(id, KinectManager.Instance.GetJointIndex(KinectInterop.JointType.HandRight)));


            }
        }
    }


    /// <summary>
    /// 检测左手还是右手是否激活，还有所在的屏幕位置，left--->right  为0-->1  down--->up  为0-->1
    /// </summary>
    private void CheckUserHandle()
    {
        if (KinectManager.Instance.GetAllUserIds().Count == 0) return;
        Vector3 screenPos = default;
        bool isLeft = false;
        string str = null;
        isLeft = InteractionManager.Instance.IsLeftHandPrimary();
        if (isLeft)
        {
            screenPos = InteractionManager.Instance.GetLeftHandScreenPos();
            str = "Left is ";
        }
        else
        {
            screenPos = InteractionManager.Instance.GetRightHandScreenPos();
            str = "Right is ";
        }
        Debug.Log(str + screenPos);
    }
    // Update is called once per frame  
    void Update()
    {
        //CheckUserRange(0.5f, 0.45f, 3f);
        //CheckUserHandle();

    }

    public int playerIndex;
    public float CheckBodyHeight()
    {

        KinectManager manager = KinectManager.Instance;

        if (manager && manager.IsInitialized() && mainCamera)
        {
            Rect backgroundRect = mainCamera.pixelRect;
            PortraitBackground portraitBack = PortraitBackground.Instance;

            if (portraitBack && portraitBack.enabled)
            {
                backgroundRect = portraitBack.GetBackgroundRect();
            }

            // overlay all joints in the skeleton
            if (manager.IsUserDetected(playerIndex))
            {
                long userId = manager.GetUserIdByIndex(playerIndex);


                Vector3 neckVector3_2 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.Neck, mainCamera, backgroundRect);

                Vector3 headVector3_3 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.Head, mainCamera, backgroundRect);

                Vector3 spineShoulderVector3_20 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.SpineShoulder, mainCamera, backgroundRect);


                Vector3 spineMidVector3_1 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.SpineMid, mainCamera, backgroundRect);


                Vector3 spineBaseVector3_0 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.SpineBase, mainCamera, backgroundRect);


                Vector3 kneeLeftVector3_13 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.KneeLeft, mainCamera, backgroundRect);

                Vector3 ankleLeftVector3_14 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.AnkleLeft, mainCamera, backgroundRect);

                float d1 = Vector3.Distance(headVector3_3, neckVector3_2);

                float d2 = Vector3.Distance(neckVector3_2, spineShoulderVector3_20);

                float d3 = Vector3.Distance(spineShoulderVector3_20, spineMidVector3_1);

                float d4 = Vector3.Distance(spineMidVector3_1, spineBaseVector3_0);

                float d5 = Vector3.Distance(spineBaseVector3_0, kneeLeftVector3_13);

                float d6 = Vector3.Distance(kneeLeftVector3_13, ankleLeftVector3_14);

                float height = d1 + d2 + d3 + d4 + d5 + d6;

                //height *= 1.06667F;

                Debug.Log("身高是 is " + height + "  d1=" + d1 + "  d2=" + d2 + "  d3=" + d3 + "  d4=" + d4 + "  d5=" + d5 + "  d6=" + d6);

            }
        }

        CheckUserRange(0.5f, 0.45f, 3f);
        return 0f;
    }

    private void OnGUI()
    {
        //if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Test"))
        //{
        //  StartCameraCapture();
        //}
    }
}