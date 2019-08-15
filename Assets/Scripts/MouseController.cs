using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class MouseController : MonoBehaviour
    {
        //현재 점프힘 값 나타내는 텍스트
        [SerializeField] private Text TextCurrentMikePower;
        //점프힘 최대값 기본 5
        [SerializeField] private float MaxMikePower = 5;
        //점프힘 최대값 나타내는 텍스트
        [SerializeField] private Text TextMaxMikePower;
        //최초 클릭 위치벡터
        private Vector3 originPosition;

        //현재(나중) 클릭 위치벡터
        private Vector3 currentMousePosition;

        //마우스가 클릭되었는지 여부
        private bool IsMouseClicked;

        //마우스 드래그 벡터
        private Vector3 diffVector;

        //마우스 드래그 벡터의 방향벡터
        private Vector3 diffNormalVector;

        //마우스 드래그 세기
        private float diffVectorManitude;

        //마우스 드래그 한 벡터 방향에 대한 z축 각도
        private float currentDegree;

        //발사 가능 여부
        private bool fireConfirm;

        //레이저 오브젝트
        [SerializeField] private GameObject Laser;

        float rot_z;
        
        private static MouseController _instance;
        public static MouseController Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            rot_z = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //염소가 멈췄으면 화살표 켜주기
            if (GameManager.Instance.goat.IsGoatStop)
            {
                transform.localScale = Vector3.one;
            }
            //염소가 이동중이면 화살표 꺼주기
            else
            {
                transform.localScale = Vector3.zero;
            }
            
            if (GameManager.Instance.IsGameStart && GameManager.Instance.goat)
            {
                Vector3 tempPos =
                    Camera.main.WorldToScreenPoint(GameManager.Instance.goat.spine.transform.position +
                                                   GameManager.Instance.goat.spine.transform.TransformVector(0, 0, 0.3f));
                tempPos.z -= 1;
                transform.position = tempPos;
                
                //최소 인식 소리를 내지 않으면 회전
                if (!GaugeController.Instance.IsChargeAttack() && GameManager.Instance.goat.IsGoatStop)
                {
                    transform.rotation = Quaternion.AngleAxis(currentDegree, Vector3.forward);
                    currentDegree -= 3f;
                }

                if (GaugeController.Instance.IsChargeAttack())
                {
                    UpdateArrow();
                }
            }
        }

        public void SetMaxMikePower(Slider slider)
        {
            MaxMikePower = slider.value;
            TextMaxMikePower.text = "MaxMikePower:" + slider.value;
        }
        public void UpdateArrow()
        {
            diffVectorManitude = Mathf.Clamp(1 + GaugeController.Instance.ChargePower / 500f, 1, MaxMikePower);
            TextCurrentMikePower.text = "CurrentMikePower:" + diffVectorManitude;
            diffVector = (Vector2)(Quaternion.Euler(0,0,currentDegree) * Vector2.up) * diffVectorManitude*1.25f;
            
            //0% ~ 30%까지는 0.735까지 채우기
            //30% ~ 65%까지는 0.82까지 채우기
            //65% ~ 100%까지는 1까지 채우기
            float fillAmount = (diffVectorManitude-1)/(MaxMikePower-1);
            Debug.Log(fillAmount);
            if (0 <= fillAmount && fillAmount < 0.3)
            {
                Laser.GetComponent<Image>().fillAmount = fillAmount * 10 / 3f * 0.735f;
            }
            else
            {
                Laser.GetComponent<Image>().fillAmount = 0.735f + (fillAmount -0.3f) * 100 / 70f * 0.265f;
            }
            Laser.SetActive(true);
        }
        public void DisableArrow()
        {
            Laser.GetComponent<Image>().fillAmount = 0;
        }
        public void DisableArrowold()
        {
            Laser.transform.localScale = new Vector3(0.05f, 1, 1);
            Laser.GetComponent<Image>().color = Color.white;
        }

        public void Fire()
        {
            diffVector = new Vector3(diffVector.x * -1, diffVector.y);
            GameManager.Instance.goat.spine.AddForce(diffVector * 1500 * GameManager.Instance.PowerMultiplier);
            //goatRigidbody.AddTorque(diffVectorManitude * -10);
        }

        private bool CheckHasPower()
        {
            return true;
        }
    }
