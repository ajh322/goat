using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollUnbreak : MonoBehaviour
{
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        transform.localPosition = startPos;
    }
}
