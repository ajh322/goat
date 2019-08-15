using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    [SerializeField] public float ChargePower=0;
    [SerializeField] private float Multiplier=1;
    [SerializeField] private Text TextMikePower;
    
    private static GaugeController _instance;
    public static GaugeController Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }
    public void SetMultiplier(Slider slider)
    {
        Multiplier = slider.value;
        TextMikePower.text = "마이크인식힘x" + slider.value + "";
    }
    //게이지 컨트롤러 작동원리
    //1. 입력이 게이지가 쌓이면 누적
    //2. 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //펀치 하기위한 최소 누적  파워
    [SerializeField] public float MinChargeShotPower=0;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Charge(float loundess)
    {
        ChargePower += loundess * Multiplier;
    }
    
    public void ResetChargePower()
    {
        ChargePower = 0;
        MouseController.Instance.DisableArrow();
    }
    public bool IsChargeAttack()
    {
        return MinChargeShotPower <= ChargePower;
    }
}
