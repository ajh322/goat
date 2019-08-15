using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goat : MonoBehaviour
{
    private int stopCount;
    public bool IsGoatStop;
    public GameObject GoatRagdoll;
    public Rigidbody spine;
    public GameObject Wall1;
    public GameObject Wall2;

    public Vector3 originPosition;
    private void Start()
    {
        GoatRagdoll.SetActive(false);
        StartCoroutine("EnumCheckIsGoatStop");
    }

    private void FixedUpdate()
    {
        Vector3 tempPos = spine.transform.position;
        
        tempPos.z = Wall1.transform.position.z;
        Wall1.transform.position = tempPos;
        
        tempPos.z = Wall2.transform.position.z;
        Wall2.transform.position = tempPos;
    }

    //계속해서 확인하는 코루틴
    private IEnumerator EnumCheckIsGoatStop()
    {
        while (true)
        {
            if (originPosition!=null)
            {
                //이전 저장위치와의 거리차이가 거의 없는 경우
                if ((spine.transform.position - originPosition).magnitude < 0.01f)
                {
//                    Debug.Log(stopCount);
                    //카운트 증가
                    stopCount++;
                    //카운트가 10번 연속으로 증가하면 멈춤상태
                    if (stopCount > 10)
                        IsGoatStop = true;
                    else
                        IsGoatStop = false;
                }
                else
                {
                    IsGoatStop = false;
                    stopCount = 0;
                }
            }
            originPosition = spine.transform.position;   
            yield return new WaitForSeconds(0.01f);
        }
    }
}
