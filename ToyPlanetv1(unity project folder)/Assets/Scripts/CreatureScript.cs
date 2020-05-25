using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CreatureScript : MonoBehaviour

{
    //navmesh for moving creature
    public NavMeshAgent navMeshAgent;

    //time taken for creature to choose a new random path
    public float timerForNewPath;

    //couroutines
    bool inCoroutine = false;
    bool testCoroutine = false;

    //used to make mutation occur only once at end of each day
    public bool mutated;


    //starting energy for all creatures
    public float energy = 90;


    //used for spawning/respawning food
    public bool fooddeleted;


    //used to add record to excel
    bool recordadded;


    //tracking distance travelled for every creature
    public float distanceTravelled = 0;


    //used to make creature reproduce only once per day
    bool reproduced;



    //used to calculate distance travelled
    Vector3 lastPosition;

    //creature
    public GameObject Blob;
    

    //mutation colour
    public Material yellow;

    //counter for amount of food eaten in day
    public int foodcount = 0;

    //position of food sensed by creature, used to send creature to position of food to eat it
    Vector3 foodposition;

    //food is detected
    public bool fooddetected;

    //detected food is stored as seperate gameobject to be destroyed when eaten
    public GameObject eatenfood;

    //energy mutation colour
    public Material purple;


    //filepath for CSV
    public string filepath = @"C:\Users\aidan\ToyPlanetv1\Assets\Resources\Resources.csv";


    
    public float energyremaining;

    //used to restrict mutations (creatures mutated to have higher speed can only further mutate speed) not a mixture of both
    public bool fasterspeed;
    public bool fasterenergy;


    //if out of energy creatures goes home
    public bool outofenergy = false;


    //ratio for speed, used so creatures consume more energy at higher speds
    public float speedmultiplier;



    
    public bool hunting;


    bool foodcor;


    //limits speed, creatures that are too fast make no sense
    bool toofast = false;


    // Start is called before the first frame update
    void Start()
    {

        //get navmesh for creature
        navMeshAgent = GetComponent<NavMeshAgent>();

        //sphere collider (detection radius for food)
        SphereCollider collider = this.GetComponent<SphereCollider>();

        //time taken to randomly decide new path
        timerForNewPath = 3.5f;

        //starting position
        lastPosition = transform.position;

        //energy = 90;

    }


    //Gets a random new position for creature to go to 
    Vector3 getNewRandomPosition()
    {
        float x = Random.Range(-15, 4);
        float z = Random.Range(-22, 21);

        Vector3 pos = new Vector3(x, 0, z);
        return pos;
    }

    //Vector3 for starting position
    Vector3 goHome()
    {
        Vector3 pos = new Vector3(13, 0, 0);
        return pos;
    }

   
    

    //couritne for time taken to decide new path (waits every 3.5 seconds before running)
    IEnumerator Coroutine()
    {
        inCoroutine = true;
        yield return new WaitForSeconds(timerForNewPath);
        getNewPath();
        inCoroutine = false;
    }


    //couroutine for game logic (checks logic every second)
    IEnumerator Couroutine2()
    {
        testCoroutine = true;
        
        //Debug.Log("COURTINE WORKING");
        yield return new WaitForSeconds(1);
        Logic();
        testCoroutine = false;
        
    }

    //used to help fix eating bugs
    IEnumerator foodCouroutine()
    {
        foodcor = true;
        yield return new WaitForSeconds(2);
        hunting = false;
        foodcor = false;

    }


    //eating of food
    void foodcheck()
    {
        //once food and creature in same location eat food
        if ((Math.Abs(navMeshAgent.transform.position.x - eatenfood.transform.position.x) <= 0.2) && (Math.Abs(navMeshAgent.transform.position.z - eatenfood.transform.position.z) <=0.2))
        {
            
            Destroy(eatenfood);
            foodcount++;
            fooddetected = false;
            hunting = false;

            
        }
    }

   
    void Logic()
    {
        //references to other scripts for time/spawning of food etc
        GameObject Manage = GameObject.Find("_Manager");
        TimerScript timerscript = Manage.GetComponent<TimerScript>();
        SpawnBlob spawnfood = Manage.GetComponent<SpawnBlob>();
        float time = timerscript.Timer;

      
        //if creature still has time and detects food go for food
        if (time < 50 && fooddetected == true && outofenergy == false)
        {

            navMeshAgent.SetDestination(foodposition);
            fooddetected = false;
            
        }

        // ran out of energy
        if (energy - distanceTravelled <=30)
        {
            outofenergy = true;
        }

        if (outofenergy == true)
        {
            navMeshAgent.SetDestination(goHome());
        }


        //go home before time up
        if (time >= 50)
        {
            navMeshAgent.SetDestination(goHome());
            //Debug.Log("Home time");
        }

    }

    //gets new random position for creature to go to
    void getNewPath()
    {
        //get current time
        GameObject Manage = GameObject.Find("_Manager");
        TimerScript timerscript = Manage.GetComponent<TimerScript>();
        float time = timerscript.Timer;

        //continue looking for food
        if (time < 50 && fooddetected == false && outofenergy == false && hunting == false)
        {
            navMeshAgent.SetDestination(getNewRandomPosition());
        }


      

    }

   

    // Update is called once per frame
    void Update()
    {

        //references to other scripts for time/spawning of food etc
        GameObject Manage = GameObject.Find("_Manager");
        TimerScript timerscript = Manage.GetComponent<TimerScript>();
        SpawnBlob spawnfood = Manage.GetComponent<SpawnBlob>();
        float timetest = timerscript.Timer;

        //Coroutines
        if (!inCoroutine)
        {
            StartCoroutine(Coroutine());
        }

        if(!testCoroutine)
        {
            StartCoroutine(Couroutine2());
        }


        //Ratio for speed (higher speed = more energy consumption
        speedmultiplier = navMeshAgent.speed / 3.5f;

        
        //if creature has a higher speed, make them consume more energy else run as normal
        if(fasterspeed == true)
        {
            distanceTravelled += (Vector3.Distance(transform.position, lastPosition)) * speedmultiplier ;
        }
        else
        {
            distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        }
        
        lastPosition = transform.position;



        
        
       
       //Try for mutation at end of day, and Reset values
        if (timetest == 60)
        {
            distanceTravelled = 0;
            foodcount = 0;
            outofenergy = false;
            fooddetected = false;
            if(mutated == false)
            {
                Mutate();
                mutated = true;
            }
      
        }

        //if time is up and creature has 2 pieces of food duplicate it
        if (timetest == 58 && foodcount >= 2 && reproduced == false)
        {
            foodcount = 0;
            Instantiate(Blob, new Vector3(15, 1, 0), transform.rotation);
            //Debug.Log("i have reproduced");
            
            reproduced = true;
        }



        //destroy remaining food and spawn new ones
        if (timetest ==60 && fooddeleted == false)
        {
            GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Food");
            foreach (GameObject obj in allObjects)
            {
                Destroy(obj);
            }
            spawnfood.SpawnFood();
            fooddeleted = true;
        }

        //reset temp bool
        if(timetest== 1)
        {
            fooddeleted = false;
            mutated = false;
            reproduced = false;
            recordadded = false;
            hunting = false;
        }

        //no food = death
        if (timetest == 57 && foodcount == 0)
        {
            Destroy(Blob);
            //Debug.Log("I have died");

        }


        //add record to excel spreadsheet
        if(timetest ==55)
        {
            if(recordadded == false)
            {
                addtest(navMeshAgent.speed, energy);
                recordadded = true;
            }
        }
        

        //creature is too fast, no more mutations
        if(navMeshAgent.speed > 10)
        {
            toofast = true;
        }
      


        //check if creature and food are in same place(to eat it)
        foodcheck();

      



    }

    //  1/10 chance at end of day to mutate giving a slightly higher speed
    //  chance at end of day to mutate giving more energy 
    void Mutate()
    {
        //Random value between 0 and 1
        double rand = Random.value;
        Debug.Log(rand);



        //5% Chance of Mutation
        if(rand<0.05)
        {
            Debug.Log("MUTATION HAS OCCURED");


            //   50/50 for either speed or energy mutation
            float rand2 = Random.value;
            

            //speed mutation
            if(toofast == false)
            {
                if (rand2 < 0.5)
                {
                    if (!fasterenergy)
                    {
                        float speedmultiplier = UnityEngine.Random.Range(1.1f, 1.3f);
                        //Debug.Log("this is random mutation for speed" + rand2);
                        navMeshAgent.speed = navMeshAgent.speed * speedmultiplier;
                        Debug.Log("Mutation for speed has occured");
                        fasterspeed = true;
                        Blob.GetComponent<Renderer>().material = yellow;
                    }

                }
            }
            
            
           //energy mutation
            if (rand2 > 0.5)
            {
                if(!fasterspeed)
                {
                    energy = energy * 1.4f;
                    Debug.Log("Mutation for energy has occured");
                    Blob.GetComponent<Renderer>().material = purple;
                    fasterenergy = true;
                }                
            }

        }


    }



    private void OnTriggerEnter(Collider other)
    {
        if(!hunting)
        {
            if (other.gameObject.tag == "Food")
            {
                //eatenfood = food detected by creature
                //stored as seperate gameObject to reference 
                eatenfood = (other.gameObject);


                foodposition = other.transform.position;
                // Debug.Log("food hit");
                //Debug.Log(fooddetected);
                // Destroy(other);
                fooddetected = true;
                hunting = true;

                if (!foodcor)
                {
                    StartCoroutine(foodCouroutine());
                }

            }

        }
    }

    
    
    public static void addtest(float speed, float energy)
    {
        string data = (speed + "," + energy).ToString();
        GameObject Manage = GameObject.Find("_Manager");
        Path path = Manage.GetComponent<Path>();
        string filepath = path.path;
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filepath, true))
        {
            file.WriteLine(data);
           
        }
    }



    
    
}
