using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveTrigger : MonoBehaviour
{
    //플레이어가 안정지역 밖으로 나가면 사망판정
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Out");
            GameManager.Instance.Death();
        }
    }
}
