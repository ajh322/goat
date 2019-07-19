using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ragdoll : MonoBehaviour
{
    public Camera camera;
    public GameObject charObj;
    public GameObject ragdollObj;

    public Rigidbody spine;

    public bool IsActivateRagdoll;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))   // Space 키를 누르면 캐릭터가 사망한다고 가정하자.
        {
            ChangeRagdoll();
            spine.AddForce(new Vector3(0f, 2500000f, 2500000f));
            IsActivateRagdoll = true;
        }
        Debug.Log(ragdollObj.transform.position);

        if (IsActivateRagdoll)
        {
            Vector3 tempPos = spine.transform.position;
            tempPos.x = camera.transform.position.x;
            camera.transform.position = tempPos;
        }
    }

    public void ChangeRagdoll()
    {
        charObj.gameObject.SetActive(false);
        ragdollObj.gameObject.SetActive(true);
    }
}
