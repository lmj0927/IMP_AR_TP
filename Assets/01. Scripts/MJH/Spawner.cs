using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.UIElements;
using System;
using UnityEngine.PlayerLoop;
public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float distance = 3.0f;
    public float hight = -0.3f;
    
    private ARAnchorManager anchormanager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anchormanager = GetComponent<ARAnchorManager>();
         SpawnEnemyInFront();

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpawnEnemyInFront()
    {
        //1. 카메라의 현재 위치와 방향을 잡는다.
        Transform camTran = Camera.main.transform;
        //2. 카메라 앞쪽 정해진 거리를 계산한다.
        UnityEngine.Vector3 Spawnpos = camTran.position + (camTran.forward * distance);
        Spawnpos.y += hight;
        //3. 해당위치에 pose 생성
        GameObject ghost = Instantiate(enemyPrefab,Spawnpos,UnityEngine.Quaternion.LookRotation(camTran.position-Spawnpos));
        //4. 해당 위치에 anchor 생성
        ARAnchor anchor = ghost.AddComponent<ARAnchor>();
        //5. 적 생성 및 귀속
    }
}
