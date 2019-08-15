using System.Collections;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int playCount=1;

    public int PlayCount
    {
        get { return playCount; }
        set
        {
            Debug.Log(playCount);
            //3회시 IS 호출, 카운트 초기화
            if (playCount >= 3)
            {
                SceneController.Instance.ShowCanvasAds();
                playCount = 1;
            }
            else
            {
                playCount = value;
            }
        }
    }

    public GameObject PrefabGoat;
    public Transform GoatSpawnPosition;
    
    private float playedTime;
    public TextMeshProUGUI TextTimer;
    //게임 시작 여부
    public bool IsGameStart;
    public Vector3 originPosition;
    public float PowerMultiplier = 1f;
    public Camera camera;
    public Goat goat;
    //최소 인식 볼륨
    [SerializeField] private float MinRecognizeVolume = 1f;
    [SerializeField] private Text MinRecognizeVolumeText;
    //인식 주기 -> 해당 값이 높을수록 인식이 더 후해짐
    [SerializeField] private float RecognizeInterval = 0.05f;
    //음성 얼마나 올라가있는지 업데이트 주기
    [SerializeField] private float UpdateGaugeInterval = 0.2f;
    [SerializeField] private Text UpdateFrame;
    public float CurrentVolume;

    public Text LabelManualDB;
    public float ManualDB;
    public Text LabelMaxPower;
    
    //염소가 멈춘시간을 카운트한다.
    public float CountGoatStopTime = 0f;
    //염소가 멈췄다고 판정하는 시간
    public float GoatStopCheckTime = 0.5f;
    
    [SerializeField] private float _time1;
    [SerializeField] private float timetest=0f;
    private int val = 0;
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        Debug.Log("goat stop");
        goat.IsGoatStop = true;
        CountGoatStopTime = GoatStopCheckTime;
        goat.GoatRagdoll.SetActive(false);
    }
    public void Restart()
    {
        Freeze();
        Invoke("UnFreeze",0.1f);
    }
    

    public void Freeze()
    {
        goat.spine.isKinematic = true;
        goat.GoatRagdoll.transform.position = originPosition;
        goat.spine.velocity = Vector3.zero;
        goat.spine.angularVelocity = Vector3.zero;
    }
    public void UnFreeze()
    {
        goat.spine.isKinematic = false;
    }
   
    private BannerView bannerView;

    void Start()
    {
     #if UNITY_ANDROID
                string appId = "ca-app-pub-3940256099942544~3347511713";
            #elif UNITY_IPHONE
                string appId = "ca-app-pub-3940256099942544~1458002511";
            #else
                string appId = "unexpected_platform";
            #endif
    
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);
    
            this.RequestBanner();
    
        originPosition = goat.GoatRagdoll.transform.position;
    }
 private void RequestBanner()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    public void SetMinRecognizeVolume(Slider slider)
    {
        MinRecognizeVolume = slider.value;
        MinRecognizeVolumeText.text = MinRecognizeVolume + "";
    }
    public void SetManualDB(Slider slider)
    {
        ManualDB = slider.value;
        LabelManualDB.text = "Volume:" + slider.value + "";
    }
    public void SetMaxPower(Slider slider)
    {
        PowerMultiplier = slider.value;
        LabelMaxPower.text = "발사힘x" + slider.value;
    }
    void FixedUpdate()
    {
        if (goat)
        {
            //카메라 위치 이동
            Vector3 tempPos = goat.spine.transform.position;
            tempPos.z = camera.transform.position.z;
            camera.transform.position = tempPos;
        }
    }

    public Goat SpawnGoat()
    {
        GameObject goat = Instantiate(PrefabGoat);
        goat.transform.position = GoatSpawnPosition.position;
        goat.GetComponent<Goat>().IsGoatStop = true;
        CountGoatStopTime = GoatStopCheckTime;
        return goat.GetComponent<Goat>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Death();
        }
        if (IsGameStart)
        {
            //중앙 상단 플레이 타임 설정
            playedTime += Time.deltaTime;
            SetTextTimer();
            
            UpdateFrame.text = val++ + "";
            timetest += Time.deltaTime;
            if (timetest >= 1)
            {
                val = 0;
                timetest = 0;
            }

            if (goat.IsGoatStop)
            {
                //goat.GoatRagdoll.SetActive(false);
            }
            else
            {
                //goat.GoatRagdoll.SetActive(true);
            }

            //입력 시작 최소 볼륨보다 크면
            if (CurrentVolume >= MinRecognizeVolume && goat.IsGoatStop)
            {
                //기존 코루틴 정지, 새로 시작
                StopCoroutine("StartRecord");
                StartCoroutine("StartRecord");
            }
        }
    }

    private void SetTextTimer()
    {
        TextTimer.text = Mathf.FloorToInt(playedTime / 60).ToString("00") + ":" + (playedTime % 60).ToString("00");
    }

    public void Death()
    {
        GameOver();
        Destroy(goat.gameObject);
    }

    public void CountDeath()
    {
        PlayCount++;
    }
    public void GameOver()
    {
        SceneController.Instance.GoGameOverScene();
        IsGameStart = false;
    }

    public void GameStart()
    {
        SceneController.Instance.GoInGameScene();
        if (goat)
        {
            Debug.Log("이미 염소 있음");
            Destroy(goat.gameObject);
        }
        goat = SpawnGoat();
        IsGameStart = true;
        playedTime = 0;
    }
    
    public void EndRecord()
    {
        Debug.Log(GaugeController.Instance.IsChargeAttack());
        if (GaugeController.Instance.IsChargeAttack())
        {
            goat.GoatRagdoll.SetActive(true);
            MouseController.Instance.Fire();
            GaugeController.Instance.ResetChargePower();
        }
    }

    IEnumerator StartRecord()
    {
        _time1 = RecognizeInterval;
        while (_time1 >= 0)
        {
            _time1 -= Time.deltaTime;
            GaugeController.Instance.Charge(CurrentVolume);
            yield return null;
        }
        EndRecord();
    }

    public bool IsManualRecordButtonClick;
    public void StartRecordManual()
    {
        IsManualRecordButtonClick = true;
        StartCoroutine("EnumRecordManual");
    }
    IEnumerator EnumRecordManual()
    {
        while (IsManualRecordButtonClick)
        {
            GaugeController.Instance.Charge(ManualDB);
            yield return null;
        }
        EndRecord();
    }
    public void EndRecordManual()
    {
        IsManualRecordButtonClick = false;
    }
}
