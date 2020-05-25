using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBlob : MonoBehaviour
{
    public GameObject Blob;
    public GameObject Food;
    public float BlobNumber;
    public float FoodNumber;
    public Slider BlobSlider;
    public Slider FoodSlider;
    //new Vector3(-9, 1, 0)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    public void SpawnBlobs()
    {
        for (int i = 0; i < BlobNumber; i++)
        {
            Instantiate(Blob, new Vector3(15, 1, 0), transform.rotation);
        }
    }

    public void SpawnFood()
    {
        for (int i =0; i <FoodNumber; i++)
        {
            Instantiate(Food, new Vector3(Random.Range(3,-15), 1,(Random.Range(-21,21))), transform.rotation);
        }
    }


    public void SubmitSliderSetting()
    {
        BlobNumber = BlobSlider.value;
        FoodNumber = FoodSlider.value;
    }
}
