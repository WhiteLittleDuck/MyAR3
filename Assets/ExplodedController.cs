using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodedController : MonoBehaviour
{
    public static ExplodedController instance;
	private AudioSource ads;
    
    void Awake()
    {
        ads = GetComponent<AudioSource>();
        instance=this;
    }
    public void play()
    {
    	ads.Play();
    }

}
