using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    /// <summary>
    /// 파티클 관리하는 매니저
    /// </summary>
    private static ParticleManager _instance;

    public static ParticleManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void MakeParticle(GameObject _particle, Vector3 _placePosition, float _playTime)
    {
        GameObject obj = Instantiate(_particle);
        obj.transform.position = _placePosition;
        Destroy(obj, _playTime);
    }
}