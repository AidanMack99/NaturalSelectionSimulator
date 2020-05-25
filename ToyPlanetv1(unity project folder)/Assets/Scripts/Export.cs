using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Export : MonoBehaviour
{
    public bool added =true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        GameObject Manage = GameObject.Find("_Manager");
        TimerScript timerscript = Manage.GetComponent<TimerScript>();
        float time = timerscript.Timer;
      

        if(time == 1)
        {
            added = false;
        }


        if(time == 2 && added == false)
        {
            addPrior();
            added = true;
        }
    }


    public static void addPrior()
    {
        GameObject Manage = GameObject.Find("_Manager");
        TimerScript timerscript = Manage.GetComponent<TimerScript>();
        float Days = timerscript.DaysGone;
        string Head = ("Day: " + Days + " --------------------------" + "\n" + "speed" + "," + "energy");


        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\aidan\ToyPlanetv1\Assets\Resources\Resources.csv", true))
        {
            file.WriteLine(Head);
            
            
        }
    }
}
