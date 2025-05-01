using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UseThis : MonoBehaviour
{
    public string pleaseDontTouch = "public에 뭐 집어넣으면 안 돼!!";
    public GameObject floor;
    public Vector3[] floorCorners;

    public GameObject[] wall;

    public Boolean isGameStart = false;

    public void getInfo(){
        floor = GameObject.FindWithTag("floor");
        if(floor == null) Debug.Log("in UseThis, can't find floor");

        floorCorners = GameObject.Find("AdjustmentSystem").GetComponent<AdjustmentSystem>().floorCornersFinal;
        Debug.Log(floorCorners[0]);
        Debug.Log(floorCorners[1]);
        Debug.Log(floorCorners[2]);
        Debug.Log(floorCorners[3]);

        wall = GameObject.FindGameObjectsWithTag("wall");
        if(wall == null) Debug.Log("in UseThis, can't find wall");
    }    
}
