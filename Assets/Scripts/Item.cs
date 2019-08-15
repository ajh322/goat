using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Item : MonoBehaviour
{
    /// <summary>
    /// 획득시 재생되는 파티클
    /// </summary>
    public GameObject ParticleItemGain;

    public void ItemGain()
    {
        if (ParticleItemGain)
            ParticleManager.Instance.MakeParticle(ParticleItemGain, transform.position, 1f);
        //TODO:아이템 얻었을때 효과를 작성해야함.
        Debug.Log("Item Gain");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        Debug.Log(other.gameObject.tag);
        //충돌 오브젝트가 플레이어인경우 아이템 획득
        if (other.gameObject.CompareTag("Player"))
            ItemGain();
    }

    private void Awake()
    {
        StartCoroutine("EnumSpin");
    }

    private IEnumerator EnumSpin()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, 1, 0));
            yield return new WaitForSeconds(0.01f);
        }
    }
}
