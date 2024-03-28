using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnSkillBall : MonoBehaviourPun
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
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Transform child in skillPointTf)
            {
                PhotonNetwork.Destroy(child.gameObject);
            }

            foreach (Transform spawnPoint in skillPointTf)
            {
                int skillType = UnityEngine.Random.Range(0, skillBallPrefabs.Length);
                GameObject skillBall = PhotonNetwork.Instantiate(skillBallPrefabs[skillType].name, spawnPoint.position, Quaternion.identity);
                skillBall.AddComponent<SkillBallVisibility>();
            }
        }
    }

}

