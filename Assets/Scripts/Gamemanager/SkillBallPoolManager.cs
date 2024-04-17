using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class SkillBallPoolManager : MonoBehaviourPunCallbacks
{
    public static SkillBallPoolManager Instance { get; private set; }

    public GameObject[] skillBallPrefabs;
    private List<Transform> skillPoints;
    public int poolSize = 3;
    public Transform skillPointTf;
    private Dictionary<string, Queue<PhotonView>> pools = new Dictionary<string, Queue<PhotonView>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            skillPoints = new List<Transform>();
            for (int i = 0; i < skillPointTf.childCount; i++)
            {
                skillPoints.Add(skillPointTf.GetChild(i));
            }

            InitializePools();
        }
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ActivateSkillBallsRoutine());
        }
    }

    private void InitializePools()
    {
        // Only the master client should initialize the pools to avoid duplicate instantiations
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var prefab in skillBallPrefabs)
            {
                Queue<PhotonView> newPool = new Queue<PhotonView>();
                for (int i = 0; i < poolSize; i++)
                {
                    Transform skillPoint = skillPoints[Random.Range(0, skillPoints.Count)];
                    GameObject obj = PhotonNetwork.Instantiate(prefab.name, skillPoint.position, Quaternion.identity);
                    obj.SetActive(false);
                    newPool.Enqueue(obj.GetComponent<PhotonView>());
                }
                pools.Add(prefab.name, newPool);
            }
        }
    }

    private IEnumerator ActivateSkillBallsRoutine()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var prefab in skillBallPrefabs)
                {
                    string skillName = prefab.name;
                    bool isSkillBallActive = pools[skillName].Any(pv => pv.gameObject.activeInHierarchy);

                    if (!isSkillBallActive && pools[skillName].Count > 0)
                    {
                        var skillBall = pools[skillName].Dequeue();
                        skillBall.gameObject.SetActive(true);
                        pools[skillName].Enqueue(skillBall);
                    }
                }
            }
            yield return new WaitForSeconds(60); 
        }
    }

    public GameObject GetSkillBall(string skillType)
    {
        if (pools.ContainsKey(skillType) && pools[skillType].Count > 0)
        {
            PhotonView photonView = pools[skillType].Dequeue();
            photonView.gameObject.SetActive(true);
            return photonView.gameObject;
        }
        else
        {
            // Optionally instantiate a new object if the pool is empty
            GameObject obj = PhotonNetwork.Instantiate(skillType, Vector3.zero, Quaternion.identity);
            return obj;
        }
    }

    public void ReturnSkillBall(string skillType, PhotonView photonView)
    {
        string cleanedSkillType = skillType.Replace("(Clone)", "").Trim();

        if (pools.ContainsKey(cleanedSkillType))
        {
            photonView.gameObject.SetActive(false);
            pools[cleanedSkillType].Enqueue(photonView);
        }
        else
        {
            Debug.LogError("Unrecognized skill type: " + cleanedSkillType);
        }
    }
}

