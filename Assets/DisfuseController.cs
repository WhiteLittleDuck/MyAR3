using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisfuseController : MonoBehaviour
{
	private AudioSource ads;
    public static DisfuseController instance;
    
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
