using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollision : MonoBehaviour
{
    public Transform ParentModel;
    private Dictionary<string, Vector3> originalPosList;

    // Start is called before the first frame update
    void Start()
    {
        originalPosList = new Dictionary<string, Vector3>();

        foreach (Transform child in ParentModel)
        {
            originalPosList.Add(child.gameObject.name, child.transform.localPosition);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "food")
        {
            collision.gameObject.SetActive(false);
            foreach (KeyValuePair<string, Vector3> item in originalPosList)
            {
                if (collision.gameObject.name == item.Key)
                {
                    collision.transform.position = item.Value;
                    collision.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    collision.gameObject.SetActive(true);
                }
            }
          
        }
        
        Debug.Log("Hit"); // ログを表示する
       

    }
}
