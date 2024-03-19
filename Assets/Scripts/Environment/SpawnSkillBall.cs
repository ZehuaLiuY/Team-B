using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSkillBall : MonoBehaviour
{
    public GameObject[] skillBallPrefabs;
    public Transform skillPointTf;
    public float refreshInterval = 60f; 

    void Start()
    {
        StartCoroutine(SpawnSkillBallsPeriodically());
    }

    IEnumerator SpawnSkillBallsPeriodically()
    {
        while (true)
        {
            GenerateSkillBalls();
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    void GenerateSkillBalls()
    {
       
        foreach(Transform child in skillPointTf)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<Transform> spawnPoints = new List<Transform>();

        for (int i = 0; i < skillPointTf.childCount; i++)
        {
            spawnPoints.Add(skillPointTf.GetChild(i));
        }

        int ballsToGenerate = spawnPoints.Count;

        for (int i = 0; i < ballsToGenerate; i++)
        {
            int skillType = UnityEngine.Random.Range(0, skillBallPrefabs.Length);
            Instantiate(skillBallPrefabs[skillType], spawnPoints[i].position, Quaternion.identity, skillPointTf);
        }
    }
}

