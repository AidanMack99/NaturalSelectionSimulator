using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float Timer = 0;
    public float DaysGone = 0;
    public Text Timeleft;
    public Text Days;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine("GainTime");
        Timeleft.text = ("0");
        Days.text = ("Days = 0");



       
        
    }

    // Update is called once per frame
    void Update()
    {
        Timeleft.text = ("" + Timer);    
        
        if(Timer > 60)
        {
            Timer = 0;
            DaysGone++;
            Days.text = ("Days = " + DaysGone);
            

        }
    }

    IEnumerator GainTime()
    {
        yield return new WaitForSeconds(3.5f);
        while (true)
        {
            yield return new WaitForSeconds(1);
            Timer++;
        }
    }
}
