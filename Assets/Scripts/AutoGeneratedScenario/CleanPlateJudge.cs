using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanPlateJudge : MonoBehaviour
{
    List<string> _plateList;

    void Start()
    {
        _plateList = new List<string>();    
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("plate") && !_plateList.Contains(other.name))
        {
            _plateList.Add(other.name);
        }    
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("plate") && _plateList.Contains(other.name))
        {
            _plateList.Remove(other.name);
        }
    }

    public bool IsPlatePicked(string plateName)
    {
        return _plateList.Contains(plateName);
    }

    public int GetPlateCount()
    {
        return _plateList.Count;
    }
}
