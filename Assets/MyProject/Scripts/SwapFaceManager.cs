using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnityEngine.Video;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading;
using System.IO;
using RenderHeads.Media.AVProMovieCapture;
using DG.Tweening;

// Token: 0x020000A0 RID: 160
public class SwapFaceManager : MonoBehaviour
{


	private string _mp4Path;

	// Token: 0x040006BC RID: 1724
	public Transform UIroot;

	// Token: 0x040006BD RID: 1725
	public Transform LoadingTipUI;

	// Token: 0x040006BE RID: 1726
	private string _modelName;

	// Token: 0x040006BF RID: 1727
	public UserBodyBlender userBodyBlender;

	// Token: 0x040006C0 RID: 1728
	public BaseListeningUI wenguanButton;

	// Token: 0x040006C1 RID: 1729
	public BaseListeningUI jiangjunButton;

	// Token: 0x040006C2 RID: 1730
	public BaseListeningUI gongnvButton;

	// Token: 0x040006C3 RID: 1731
	public BaseListeningUI shinvButton;


    public BaseListeningUI PhotoBtn;

    public BaseListeningUI ClosePhotoBtn;

	// Token: 0x040006C4 RID: 1732
	public GameObject Photopanel;

	// Token: 0x040006C5 RID: 1733
	public GameObject movieGameObject;

	// Token: 0x040006C6 RID: 1734
	public GameObject closeGameObject;

	// Token: 0x040006C7 RID: 1735
	public GameObject ShooterPictreuGameObject;

	// Token: 0x040006C8 RID: 1736
	public GameObject QrImage;

	public RawImage ShooterImage;

	public RawImage MovieImage;

	// Token: 0x040006C9 RID: 1737
	public Image LogoWhite;

	// Token: 0x040006CA RID: 1738
	public Image Logo;

	// Token: 0x040006CB RID: 1739
	public Transform[] countdown;

	// Token: 0x040006CC RID: 1740
	private ModelData _curModelData;

	// Token: 0x040006CD RID: 1741
	private GameObject _curSwapFaceHead;

	// Token: 0x040006CE RID: 1742
	private long _curId;

	private Coroutine _coroutineCapture;

	private Texture2D _movieTex;

	// Token: 0x0600066B RID: 1643 RVA: 0x000466F4 File Offset: 0x000448F4
	private void Awake()
    {

        this.wenguanButton.OnRayClick += () =>
        {
          
                bool isRestCapture = false;
                if (_movieTex != null && _movieTex.name == wenguanButton.name)
                {
                    isRestCapture = false;

                }
                else
                {
                    isRestCapture = true;
                    StopCaptureVideo();
                }
                this._modelName = this.wenguanButton.name;
                this.LoadingTip(true, this._modelName);
                bool flag = this._curSwapFaceHead == null;
                if (flag)
                {
                    ChangeingClothesManager.Instance.StartSwapFace();
                }
                else
                {

                    this.LoadModel(isRestCapture);

                }
        };
        this.jiangjunButton.OnRayClick += () =>
        {

            Debug.Log("将军");
            bool isRestCapture = false;
            if (_movieTex != null && _movieTex.name == jiangjunButton.name)
            {
                isRestCapture = false;

            }
            {
                isRestCapture = true;
                StopCaptureVideo();
            }

            this._modelName = this.jiangjunButton.name;
            this.LoadingTip(true, this._modelName);
            bool flag = this._curSwapFaceHead == null;
            if (flag)
            {
                ChangeingClothesManager.Instance.StartSwapFace();
            }
            else
            {

                this.LoadModel(isRestCapture);
            }
		};
        this.gongnvButton.OnRayClick += () =>
        {

            bool isRestCapture = false;//是否需要重新录制  
            if (_movieTex != null && _movieTex.name == gongnvButton.name)
            {
                isRestCapture = false;

            }
            else
            {
                isRestCapture = true;
                StopCaptureVideo();
            }

            this._modelName = this.gongnvButton.name;
            this.LoadingTip(true, this._modelName);
            bool flag = this._curSwapFaceHead == null;
            if (flag)
            {
                ChangeingClothesManager.Instance.StartSwapFace();
            }
            else
            {

                this.LoadModel(isRestCapture);
            }
		};
        this.shinvButton.OnRayClick += () =>
        {
            bool isRestCapture = false;
            if (_movieTex != null && _movieTex.name == shinvButton.name)
            {
                isRestCapture = false;

            }
            else
            {
                isRestCapture = true;
                StopCaptureVideo();
            }

            this._modelName = this.shinvButton.name;
            this.LoadingTip(true, this._modelName);
            bool flag = this._curSwapFaceHead == null;
            if (flag)
            {
                ChangeingClothesManager.Instance.StartSwapFace();
            }
            else
            {

                this.LoadModel(isRestCapture);
            }
		};

        PhotoBtn.OnRayClick += () =>
        {
            PhotoShooter photoShooter = gameObject.GetComponent<PhotoShooter>();
            if (photoShooter && photoShooter.enabled)
            {
                photoShooter.CountdownAndMakePhoto();
            }
		};
        ClosePhotoBtn.OnRayClick += CloseShooterImage;

        ClosePhotoBtn.GetComponent<Button>().onClick.AddListener((CloseShooterImage));


		closeGameObject.GetComponent<BaseListeningUI>().OnRayClick = CleanKinect;

		movieGameObject.GetComponent<BaseListeningUI>().OnRayClick = ShowMovie;

		_path = Application.streamingAssetsPath + "/Video/";
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00046778 File Offset: 0x00044978
	private void Start()
	{
		this.QrImage.SetActive(false);
		KinectManager.Instance.RemoveUserEvent += this.RemoveUserEvent;
		NetManager.Instance.PostPictureCompleted += this.PostPictureCompleted;
		NetManager.Instance.PostMp4Completed += PostMp4Completed;
		this.ShowOtherBtn(false);
		this.ShowLogo(true);
	}


	private void PostMp4Completed(Texture2D tex)
	{
		if (tex!=null)
		{
			Debug.Log("视频上传成功");
			if (_movieTex != null)
				Destroy(_movieTex);
			_movieTex = null;

			_movieTex = tex;
			_movieTex.name = _modelName;

			this.QrImage.GetComponent<RawImage>().texture = tex;
			MovieButtonEffect(true);
			ShowProgress(false);
		}
	}

	private Tween _tweenAlpha;

	private Tween _tweenScale;
	/// <summary>
	/// 影片按钮动画提示特效
	/// </summary>
	/// <param name="isOpen"></param>
	private void MovieButtonEffect(bool isOpen)
    {
		Image effect = movieGameObject.transform.Find("effect").GetComponent<Image>();

		if( isOpen && _movieTex!=null)
        {
			_tweenAlpha= effect.DOFade(0f, 2f).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutQuad);

			_tweenScale =effect.rectTransform.DOScale(1.5f, 2f).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutQuad); 

		}
		else
        {
			_tweenAlpha.Kill();
			_tweenScale.Kill();
			effect.DOFade(0.5f, 0f);
			effect.rectTransform.DOScale(1f, 0f);
			

		}
    }
	private Coroutine _progerssCoroutine;
	private float allTime = 0f;
	/// <summary>
	/// 显示视频制作进度  
	/// </summary>
	private void ShowProgress(bool isShow)
    {
		if (isShow)
        {
			_progerssCoroutine= StartCoroutine(Progress(allTime));
		}
        else
        {
			StopCoroutine(_progerssCoroutine);
			movieGameObject.transform.Find("Slider").gameObject.SetActive(false);
		}
    }
	
	private IEnumerator Progress(float time)
    {
		float timeTemp = 0f;
		Slider slider = movieGameObject.transform.Find("Slider").GetComponent<Slider>();
		slider.gameObject.SetActive(true);
		while (true)
        {
			yield return null;

			timeTemp += Time.deltaTime;
			slider.value = timeTemp / time;
			if (timeTemp>=time)
            {
				yield break;
            }

		}
    }
	public void ShowMovie()
	{

		if(_mp4Path!=null && _movieTex!=null)
        {
			this.QrImage.gameObject.SetActive(true);

			this.ShooterImage.gameObject.SetActive(false);
			this.MovieImage.gameObject.SetActive(true);
			this.MovieImage.GetComponent<VideoPlayer>().url = _mp4Path;
			this.ShooterPictreuGameObject.SetActive(true);
			this.UIroot.gameObject.SetActive(false);
		}
		else
        {
			Debug.Log("Mp4Path _movieTex  有一个或者两个为null" );
        }

		
	}
	// Token: 0x0600066D RID: 1645 RVA: 0x000467D1 File Offset: 0x000449D1
	private IEnumerator CoCountdownAndMakePhoto()
	{
		bool flag = this.countdown != null && this.countdown.Length != 0;
		if (flag)
		{
			int num;
			for (int i = 0; i < this.countdown.Length; i = num + 1)
			{
				bool flag2 = this.countdown[i];
				if (flag2)
				{
					this.countdown[i].gameObject.SetActive(true);
				}
				yield return new WaitForSeconds(1f);
				bool flag3 = this.countdown[i];
				if (flag3)
				{
					this.countdown[i].gameObject.SetActive(false);
				}
				num = i;
			}
		}
		yield return null;
		yield break;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x000467E0 File Offset: 0x000449E0
	private void PostPictureCompleted(Texture2D obj)
	{
		bool flag = obj != null;
		if (flag)
		{
			this.QrImage.gameObject.SetActive(true);
			this.QrImage.GetComponent<RawImage>().texture = obj;
			this.ShooterImage.gameObject.SetActive(true);
			this.MovieImage.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0004681F File Offset: 0x00044A1F
	private void RemoveUserEvent(long obj)
	{
		this.CloseTipUI();
		StopCaptureVideo();
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00046829 File Offset: 0x00044A29
	private void ShowOtherBtn(bool isShow)
	{
		this.Photopanel.SetActive(isShow);
		this.movieGameObject.SetActive(isShow);
		
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00046854 File Offset: 0x00044A54
	public void ShowRoleUi(bool isShow, long curId)
	{
		this.UIroot.gameObject.SetActive(isShow);
		this.closeGameObject.SetActive(isShow);
		ShowOtherBtn(false);
		if (isShow)
		{
			this._curId = curId;
		}
		this.ShowLogo(true);
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0004688C File Offset: 0x00044A8C
	public void ShowShooterImage(Texture2D tex)
	{
		this.ShooterPictreuGameObject.transform.Find("ShooterPicture").GetComponent<RawImage>().texture = tex;
		this.ShooterPictreuGameObject.SetActive(true);
		this.UIroot.gameObject.SetActive(false);
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x000468DC File Offset: 0x00044ADC
	public void CloseShooterImage()
	{
		this.ShooterPictreuGameObject.transform.Find("ShooterPicture").GetComponent<RawImage>().texture = null;
		this.ShooterPictreuGameObject.SetActive(false);
		Resources.UnloadUnusedAssets();
		this.UIroot.gameObject.SetActive(true);
		
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00046930 File Offset: 0x00044B30
	public void ShowLogo(bool isShow)
	{
		this.Logo.gameObject.SetActive(isShow);
		this.LogoWhite.gameObject.SetActive(!isShow);
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0004695C File Offset: 0x00044B5C
	public void CloseTipUI()
	{
		this.LoadingTip(false, null);
		bool flag = this._curSwapFaceHead != null;
		if (flag)
		{
			this._curSwapFaceHead.transform.parent = null;
			Object.Destroy(this._curSwapFaceHead);
			this._curSwapFaceHead = null;
		}
		bool flag2 = this._curModelData != null;
		if (flag2)
		{
			Object.Destroy(this._curModelData.gameObject);
		}
		this._curModelData = null;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x000469D4 File Offset: 0x00044BD4  
	public void LoadingTip(bool isShow, string str = null)
	{
		this.ShowLogo(false);
		Text text = this.LoadingTipUI.transform.Find("Image/Text").GetComponent<Text>();
		bool flag = str != null;
		if (flag)
		{
			bool isTip = false;
			GameObject tipText = this.LoadingTipUI.transform.Find("Image/Loading").gameObject;
			bool flag2 = str == "文官";
			if (flag2)
			{
				text.text = "文人雅士，稍等莫急。。。";
			}
			else
			{
				bool flag3 = str == "将军";
				if (flag3)
				{
					text.text = "勇士稍等，让你看看威武的自己。。。";
				}
				else
				{
					bool flag4 = str == "宫女";
					if (flag4)
					{
						text.text = "美人何处寻，只需你等一等。。。";
					}
					else
					{
						bool flag5 = str == "侍女";
						if (flag5)
						{
							text.text = "美人何处寻，只需你等一等。。。";
						}
						else
						{
							bool flag6 = str == "未知错误";
							if (flag6)
							{
								text.text = "未知错误";
								isTip = true;
							}
							else
							{
								bool flag7 = str == "网络断开";
								if (flag7)
								{
									text.text = "请确认服务器是否开启。";
									isTip = true;
								}
								else
								{
									bool flag8 = str == "脸部数据错误";
									if (flag8)
									{
										text.text = "请正视前方屏幕，重新识别脸部。";
										isTip = true;
									}
								}
							}
						}
					}
				}
			}
			bool flag9 = !isTip;
			if (flag9)
			{
				tipText.SetActive(true);
			}
			else
			{
				tipText.SetActive(false);
				base.StartCoroutine(this.CoCountdownAndMakePhoto());
			}
		}
		this.LoadingTipUI.gameObject.SetActive(isShow);
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00046B5C File Offset: 0x00044D5C  
	/// <summary>
	/// 导入模型
	/// </summary>
	/// <param name="isRestCapture">是否需要重新录制动画数据</param>
	private void LoadModel(bool isRestCapture)
	{
		bool flag = this._curSwapFaceHead == null;
		float targetPos = 10f;
		float time = 10f;
		if (flag)
		{
			throw new UnityException("没有找到换脸后的头部");
		}
		bool flag2 = this._curModelData != null;
		if (flag2)
		{
			Object.Destroy(this._curModelData.gameObject);
		}
		this._curModelData = null;
		string path = null;
		bool flag3 = this._modelName == "文官";
		if (flag3)
		{
			path = "Clothing/0001/model";
			this.userBodyBlender.ChangeBackGround(1);
			time = 10f;
			targetPos = 2.2f;
		}
		else
		{
			bool flag4 = this._modelName == "将军";
			if (flag4)
			{
				path = "Clothing/0000/model";
				this.userBodyBlender.ChangeBackGround(0);
				time = 4.5f;
				targetPos = 3111f;
			}
			else
			{
				bool flag5 = this._modelName == "宫女";
				if (flag5)
				{
					path = "Clothing/0002/model";
					this.userBodyBlender.ChangeBackGround(1);
					time = 10f;
					targetPos = 2.15f;
				}
				else
				{
					bool flag6 = this._modelName == "侍女";
					if (flag6)
					{
						path = "Clothing/0003/model";
						this.userBodyBlender.ChangeBackGround(1);
						targetPos = 2.12f;
						time = 4.7f;
					}
				}
			}
		}

		Debug.Log(path);

		GameObject go = Resources.Load<GameObject>(path);
		GameObject temp = Object.Instantiate<GameObject>(go);
		this._curModelData = temp.GetComponent<ModelData>();
        if (_curModelData == null)
        {
			Debug.LogError("加载的模型获取不到数据");
        }
        else
        {
			this._curModelData.SetHead(this._curSwapFaceHead, targetPos, time);
            if (isRestCapture)
            {
                _coroutineCapture = StartCoroutine(StartCaptureVideo(time + 3f));
                allTime = time + 3 + 3;
            }
            else
            {
                allTime = -1f;

            }

            this.LoadingTip(false, null);
            //提示录制视频，时间是time +3  +3  3为站立动画   3位转码所用的大概时间
		}



	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00046CA8 File Offset: 0x00044EA8
	public void CleanKinect()
	{
		KinectManager.Instance.ClearKinectUsers();
		this.ShowOtherBtn(false);
		if (_movieTex != null)
			Destroy(_movieTex);
		_movieTex = null;
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00046CC0 File Offset: 0x00044EC0
	public void LoadModel(GameObject headGameObject, long curId)
	{
		bool flag = curId != this._curId;
		if (flag)
		{
			Object.Destroy(headGameObject);
		}
		else
		{
			this.ShowOtherBtn(true);
			this._curSwapFaceHead = headGameObject;
			this.LoadModel(true);
			MyInternaction.Instance.EnableHand(false,0.1f,true);

		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00046CFE File Offset: 0x00044EFE
	private void OnDestroy()
	{
		NetManager.Instance.PostPictureCompleted -= this.PostPictureCompleted;
		NetManager.Instance.PostMp4Completed -= PostMp4Completed;
	}
	#region  录制视频相关
	public IEnumerator StartCaptureVideo(float captureTime)
	{
		
		
       
        yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(1f);
        //录屏先清空文件夹内容
        string[] fileNames = Directory.GetFiles(_path, "*.*");

        foreach (string fileName in fileNames)
        {
            File.Delete(fileName);
        }


        ShowProgress(true);

        AvProMovieCaptureFromCamera._codecName = "Media Foundation H.264(MP4)";
        AvProMovieCaptureFromCamera._useMediaFoundationH264 = true;
        AvProMovieCaptureFromCamera._noAudio = true;

        AvProMovieCaptureFromCamera._outputFolderPath = _path;//指定视频输出路径

		
                                                              //开始相机录制视频   
        bool isStart =   AvProMovieCaptureFromCamera.StartCapture();
		
		Debug.LogError("开始录制 " + isStart);

		if (isStart) Debug.LogError("开始录制成功");
		else Debug.LogError("开始录制失败");

		yield return new WaitForSeconds(captureTime);

		yield return new WaitForEndOfFrame();

		Debug.LogError("录制视频结束");

		AvProMovieCaptureFromCamera.StopCapture();

        _coroutineCapture = StartCoroutine(CompressVideoTest("mp4"));
    }

	public void StopCaptureVideo()
    {
        Debug.Log("取消录制视频");

        MovieButtonEffect(false);
        if (_coroutineCapture != null) StopCoroutine(_coroutineCapture);
		
		AvProMovieCaptureFromCamera.CancelCapture();
        if (_p != null)
        {
            _p.Close();//关闭进程  
            _p.Dispose();//释放资源  
            _p = null;
        }

        if (_t != null)
        {
            _t.Abort();//强制停止

            _t = null;
        }

        StartCoroutine(GlobalSettings.WaitEndFarme(() => {
            
        }));
		_mp4Path = null;




	}

	/// <summary>
	/// 录屏保存的文件夹路径
	/// </summary>
	private string _path;

	/// <summary>
	/// 录制好视频后保存的目录
	/// </summary>
	private string _destFileName;

	public CaptureFromCamera AvProMovieCaptureFromCamera;

	Thread _t;
	Process _p;
	/// <summary>
	/// 用ffmpeg2theora压缩OGV
	/// </summary>
	/// <param name="tension"></param>
	/// <returns></returns>
	private IEnumerator CompressVideoTest(string tension)
	{
		yield return null;

		bool isCompressCompleted = false;
		string srcFileName = null;
		_t = new Thread(() =>
		{
			string[] fileNames = Directory.GetFiles(_path, "*.mp4");

			if (fileNames.Length <= 0) throw new UnityException("没有找到视频文件");

			_p = new Process();



			//_p.StartInfo.FileName = Application.streamingAssetsPath + "/ffmpeg2theora.exe";
			_p.StartInfo.FileName = Application.streamingAssetsPath + "/ffmpeg-20200601-dd76226-win64-static/bin/ffmpeg.exe";

			//p.StartInfo.FileName = "cmd";
			_p.StartInfo.UseShellExecute = false;


			srcFileName = fileNames[0];

			_destFileName = _path + "completed." + tension;



        	//_p.StartInfo.Arguments = "-i "+srcFileName + " --videoquality 8 --pp default --audiobitrate 128 -o " + _destFileName;   //执行参数
			_p.StartInfo.Arguments = "-i " + srcFileName + " -y -b 1300000 -an " + _destFileName;    //执行参数

			//Debug.Log("执行参数是： " + _p.StartInfo.Arguments);

			//p.StartInfo.Arguments ="/c title C:\\Users\\Administrator\\Desktop\\MovieCapture-2017-03-28-64257s-1200x900.ogv && ffmpeg2theora \"H:\\WZS_work\\Smilewall\\Smilewall20170310\\smilewallClineDmeo1020\\smilewallClineDmeo1020\\Assets\\StreamingAssets\\Video\\MovieCapture-2017-03-28-64257s-1200x900.avi\"  --videobitrate 1000 --pp default --audiobitrate 128 --two-pass --soft-target --contact \"http://sourceforge.net/projects/theoraconverter\" -o \"C:\\Users\\Administrator\\Desktop\\MovieCapture-2017-03-28-64257s-1200x900.ogv\"";

			_p.StartInfo.UseShellExecute = false;  ////不使用系统外壳程序启动进程
			_p.StartInfo.CreateNoWindow = true;  //不显示dos程序窗口

			_p.StartInfo.RedirectStandardInput = true;

			_p.StartInfo.RedirectStandardOutput = true;

			_p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中

			_p.StartInfo.UseShellExecute = false;

			_p.ErrorDataReceived += ErrorDateReceived;

			_p.OutputDataReceived += Output;

			_p.Start();

			_p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

			_p.BeginErrorReadLine();//开始异步读取

			_p.WaitForExit();//阻塞等待进程结束

			_p.Close();//关闭进程

			_p.Dispose();//释放资源


			isCompressCompleted = true;
		});
		_t.Start();

		//等待压缩完成
		while (!isCompressCompleted)
		{
			yield return null;
			//Debug.Log("正在压缩视频中...");
		}

		Debug.Log("压缩视频完成...");
		_mp4Path = srcFileName;

		yield return null;

		_coroutineCapture = StartCoroutine(	NetManager.Instance.PostMP4ToServer(_destFileName));

		//提交服务器    
	}

	private void Output(object sendProcess, DataReceivedEventArgs output)
	{
		if (!String.IsNullOrEmpty(output.Data))
		{
			Debug.Log(output.Data);
		}
	}

	private void ErrorDateReceived(object sendProcess, DataReceivedEventArgs eventArgs)
	{
		Debug.Log(eventArgs.Data);
	}
	#endregion
	private void TestMovie()
    {
		string path = "file:///" + Application.streamingAssetsPath + "/Video/Original1.mp4";

		this.QrImage.gameObject.SetActive(true);

		this.ShooterPictreuGameObject.transform.Find("ShooterPicture").gameObject.SetActive(false);
		this.ShooterPictreuGameObject.transform.Find("MovieImage").gameObject.SetActive(true);
		
		this.ShooterPictreuGameObject.transform.Find("MovieImage").GetComponent<VideoPlayer>().url =path;
		this.ShooterPictreuGameObject.transform.Find("MovieImage").GetComponent<VideoPlayer>().Play();
		this.ShooterPictreuGameObject.SetActive(true);
		this.UIroot.gameObject.SetActive(false);

		
	}
	
	
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
    //    {
    //        _coroutine = StartCoroutine(TestFun1());


    //    }
    //    if (GUI.Button(new Rect(100f, 0f, 100f, 100f), "test"))
    //    {
    //        StopCoroutine(_coroutine);


    //    }
    //}

}
