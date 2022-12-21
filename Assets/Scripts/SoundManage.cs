using System.Collections.Generic;
using UnityEngine;

public class SoundManage : MonoBehaviour
{
    public List<AudioClip> audioClipList;
    public AudioSource bgmSource;

    float waitTime, timer = 0;
    public bool isBgmEnd = true;

    private void Update()
    {
        if(isBgmEnd)
        {
            bgmSource.clip = audioClipList[0];
            waitTime = audioClipList[0].length;
            AudioClip tempClip = audioClipList[0];
            audioClipList.RemoveAt(0);
            audioClipList.Add(tempClip);

            bgmSource.Play();
            isBgmEnd = false;
        }

        timer += Time.deltaTime;

        if(timer >= waitTime)
        {
            isBgmEnd = true;
            timer = 0;
        }
    }
}
