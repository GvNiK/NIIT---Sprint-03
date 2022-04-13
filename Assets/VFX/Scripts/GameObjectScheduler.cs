using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectScheduler : MonoBehaviour
{
    public float LifeTime = 5;

    private List<Transform> children = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            children.Add(child);
            child.gameObject.SetActive(false);
        }

        StartCoroutine(Scheduler());
    }

    IEnumerator Scheduler()
    {
        WaitForSecondsRealtime w = new WaitForSecondsRealtime(4f);

        while(true)
        {
            for(int i = 0; i < children.Count; ++i)
            {
                if(!children[i].gameObject.activeInHierarchy)
                {
                    if(Random.Range(0,2) == 1)
                    {
                        StartCoroutine(StartObject(i));
                    }
                }
            }
            

            yield return w;
        }
    }

    IEnumerator StartObject(int id) 
    {
        children[id].gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(LifeTime);

        children[id].gameObject.SetActive(false);
    }

}
