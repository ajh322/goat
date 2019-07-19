using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Vector3 originPosition;
    public GameObject currentGoat;
    public Rigidbody GoatRigidBody;
    public bool IsGoatStopped;
    public float PowerMultiplier = 1f;
    public Camera camera;
    public Rigidbody spine;
    public GameObject Wall1;
    public GameObject Wall2;
    //최소 인식 볼륨
    [SerializeField] private float MinRecognizeVolume = 1f;
    [SerializeField] private Text MinRecognizeVolumeText;
    //인식 주기 -> 해당 값이 높을수록 인식이 더 후해짐
    [SerializeField] private float RecognizeInterval = 0.05f;
    //음성 얼마나 올라가있는지 업데이트 주기
    [SerializeField] private float UpdateGaugeInterval = 0.2f;
    [SerializeField] private Text UpdateFrame;
    public float CurrentVolume;

    public Text TextManualDB;
    public float ManualDB;
    
    //고트가 멈춘 시간을 카운트시작하는지 확인
    public bool IsStartGoatStopCount;
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
    }
    public void Restart()
    {
        Freeze();
        Invoke("UnFreeze",0.1f);
    }
    

    public void Freeze()
    {
        GoatRigidBody.isKinematic = true;
        currentGoat.transform.position = originPosition;
        GoatRigidBody.velocity = Vector3.zero;
        GoatRigidBody.angularVelocity = Vector3.zero;
    }
    public void UnFreeze()
    {
        GoatRigidBody.isKinematic = false;
    }
   

    void Start()
    {
        originPosition = currentGoat.transform.position;
    }

    public void SetMinRecognizeVolume(Slider slider)
    {
        MinRecognizeVolume = slider.value;
        MinRecognizeVolumeText.text = MinRecognizeVolume + "";
    }
    public void SetManualDB(Slider slider)
    {
        ManualDB = slider.value;
        TextManualDB.text = slider.value + "";
    }
    void FixedUpdate()
    {
      
        Vector3 tempPos = spine.transform.position;
        tempPos.z = camera.transform.position.z;
        camera.transform.position = tempPos;
        
        tempPos.z = Wall1.transform.position.z;
        Wall1.transform.position = tempPos;
        
        tempPos.z = Wall2.transform.position.z;
        Wall2.transform.position = tempPos;
    }

    void Update()
    {
        UpdateFrame.text = val++ + "";
        timetest += Time.deltaTime;
        if (timetest >= 1)
        {
            val = 0;
            timetest = 0;
        }

        if (GoatRigidBody.velocity.magnitude < 0.1)
        {
            IsStartGoatStopCount = true;
        }
        else
        {
            IsGoatStopped = false;
            IsStartGoatStopCount = false;
            CountGoatStopTime = 0f;
        }

        if (IsStartGoatStopCount)
        {
            CountGoatStopTime += Time.deltaTime;
            if (CountGoatStopTime >= GoatStopCheckTime)
            {
                currentGoat.SetActive(false);
                IsGoatStopped = true;
            }
            else
            {
                currentGoat.SetActive(true);
                IsGoatStopped = false;
            }
        }
        //입력 시작 최소 볼륨보다 크면

        if (IsGoatStopped)
        {
            MouseController.Instance.transform.localScale = Vector3.one;
        }
        else
        {
            MouseController.Instance.transform.localScale = Vector3.zero;
        }
        
        if (CurrentVolume >= MinRecognizeVolume && IsGoatStopped)
        {
            //기존 코루틴 정지, 새로 시작
            StopCoroutine("StartRecord");
            StartCoroutine("StartRecord");
        }
    }

    public void EndRecord()
    {
        Debug.Log(GaugeController.Instance.IsChargeAttack());
        if (GaugeController.Instance.IsChargeAttack())
        {
            currentGoat.SetActive(true);
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
