using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class MouseController : MonoBehaviour
    {
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

        //염소 rigidbody2d
        [SerializeField] private Rigidbody goatRigidbody;
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
            Vector3 tempPos = Camera.main.WorldToScreenPoint(goatRigidbody.transform.position + goatRigidbody.transform.TransformVector(0,0,0.3f));
            tempPos.z -= 1;
            transform.position = tempPos;
            if (!GaugeController.Instance.IsChargeAttack())
            {
                transform.rotation = Quaternion.AngleAxis(currentDegree, Vector3.forward);
                currentDegree -= 3f;
            }

            if (GaugeController.Instance.IsChargeAttack())
            {
                UpdateArrow();
            }
        }

        public void UpdateArrow()
        {

            diffVectorManitude = Mathf.Clamp(1 + GaugeController.Instance.ChargePower / 500f, 1, 5);
            diffVector = (Vector2)(Quaternion.Euler(0,0,currentDegree) * Vector2.up) * diffVectorManitude*1.25f;
            Vector3 tempScale = Laser.transform.localScale;
            tempScale.y = diffVectorManitude;
            Laser.transform.localScale = tempScale;
            Laser.GetComponent<Image>().color = Color.red;
            Laser.SetActive(true);
        }

        public void DisableArrow()
        {
            Laser.transform.localScale = new Vector3(0.05f, 1, 1);
            Laser.GetComponent<Image>().color = Color.white;
        }

        public void Fire()
        {
            diffVector = new Vector3(diffVector.x * -1, diffVector.y);
            goatRigidbody.AddForce(diffVector * 5000 * GameManager.Instance.PowerMultiplier);
            //goatRigidbody.AddTorque(diffVectorManitude * -10);
        }

        private bool CheckHasPower()
        {
            return true;
        }
    }
