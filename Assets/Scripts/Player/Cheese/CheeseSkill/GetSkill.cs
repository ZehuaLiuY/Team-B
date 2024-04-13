using System.Collections;
using System.Collections.Generic;
using CheeseController;
using Photon.Pun;
using UnityEngine;
public class GetSkill : MonoBehaviourPunCallbacks
{
    public AudioClip _skillAcquiredSound;

    private bool hasSkill = false;
    private string _invisible = "Invisible Skill";
    private string _clone = "Clone Skill";
    private string _detector = "Detector Skill";
    private string _sprint = "Sprint Skill";
    private string _jump = "Jump Skill";
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (photonView.IsMine)
    //     {
    //         if (other.CompareTag(Invisible) && !hasSkill)
    //         {
    //             other.gameObject.SetActive(false);
    //             ActivateSkill(Invisible);
    //             hasSkill = true;
    //         } else if (other.CompareTag(Clone) && !hasSkill)
    //         {
    //             other.gameObject.SetActive(false);
    //             ActivateSkill(Clone);
    //             hasSkill = true;
    //         } else if (other.CompareTag(Detector) && !hasSkill)
    //         {
    //             other.gameObject.SetActive(false);
    //             ActivateSkill(Detector);
    //             hasSkill = true;
    //         } else if (other.CompareTag(Sprint) && !hasSkill)
    //         {
    //             other.gameObject.SetActive(false);
    //             ActivateSkill(Sprint);
    //             hasSkill = true; 
    //         } else if (other.CompareTag(Jump) && !hasSkill)
    //         {
    //             other.gameObject.SetActive(false);
    //             ActivateSkill(Jump);
    //             hasSkill = true; 
    //         }
    //     }
    // }
    
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            string skillTag = null;

            if (other.CompareTag(_invisible) && !hasSkill) skillTag = _invisible;
            else if (other.CompareTag(_clone) && !hasSkill) skillTag = _clone;
            else if (other.CompareTag(_detector) && !hasSkill) skillTag = _detector;
            else if (other.CompareTag(_sprint) && !hasSkill) skillTag = _sprint;
            else if (other.CompareTag(_jump) && !hasSkill) skillTag = _jump;

            if (skillTag != null)
            {
                // call RPC to hide the skill ball
                photonView.RPC("HideSkillBall", RpcTarget.All, other.gameObject.GetPhotonView().ViewID);
                ActivateSkill(skillTag);
                hasSkill = true;
                
                if (_audioSource && _skillAcquiredSound)
                {
                    _audioSource.PlayOneShot(_skillAcquiredSound);
                }
            }
        }
    }

    [PunRPC]
    void HideSkillBall(int ballViewID)
    {
        PhotonView ballPhotonView = PhotonView.Find(ballViewID);
        if (ballPhotonView != null)
        {
            ballPhotonView.gameObject.SetActive(false);
        }
    }
    
    
    
    public void ActivateSkill(string skills)
    {
        if (skills == _invisible)
        {
            InvisibilityEffect invisibilityEffect = GetComponent<InvisibilityEffect>();
            if (invisibilityEffect != null && CheeseFightUI.Instance != null)
            {
                invisibilityEffect.enabled = true;
                CheeseFightUI.Instance.ShowSkillUI(true, _invisible);
            }
        } else if (skills == _clone)
        {
            Clone_Skill cloneSkill = GetComponent<Clone_Skill>();
            if (cloneSkill != null && CheeseFightUI.Instance != null)
            {
                cloneSkill.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, _clone);
            }
        } else if (skills == _detector)
        {
            EnemyDetector enemyDetector = GetComponentInChildren<EnemyDetector>();
            if (enemyDetector != null && CheeseFightUI.Instance != null)
            {
                enemyDetector.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, _detector);
            }
        } else if (skills == _sprint)
        {
            Sprint_Skill sprintSkill = GetComponent<Sprint_Skill>();
            if (sprintSkill != null && CheeseFightUI.Instance != null)
            {
                sprintSkill.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, _sprint);
            }
        } else if (skills == _jump)
        {
            Jump_Skill jumpSkill = GetComponent<Jump_Skill>();
            if (jumpSkill != null && CheeseFightUI.Instance != null)
            {
                jumpSkill.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, _jump);
            }
        }
    }

    public void DeactivateSkill(string skills)
    {
        if (skills == _invisible)
        {
            hasSkill = false; 
            InvisibilityEffect invisibilityEffect = GetComponent<InvisibilityEffect>();
            if (invisibilityEffect != null)
            {
                invisibilityEffect.enabled = false;
                CheeseFightUI.Instance.ShowSkillUI(false, _invisible);
            }
        } else if (skills == _clone)
        {
            hasSkill = false; 
            Clone_Skill cloneSkill = GetComponent<Clone_Skill>();
            if (cloneSkill != null)
            {
                cloneSkill.enabled = false; 
                CheeseFightUI.Instance.ShowSkillUI(false, _clone);
            }
        } else if (skills == _detector)
        {
            hasSkill = false;
            EnemyDetector enemyDetector = GetComponentInChildren<EnemyDetector>();
            if (enemyDetector != null)
            {
                enemyDetector.enabled = false; 
                CheeseFightUI.Instance.ShowSkillUI(false, _detector);
            }
        } else if (skills == _sprint)
        {
            hasSkill = false; 
            Sprint_Skill sprintSkill = GetComponent<Sprint_Skill>();
            if (sprintSkill != null)
            {
                sprintSkill.enabled = false;
                CheeseFightUI.Instance.ShowSkillUI(false, _sprint);
            }
        } else if (skills == _jump)
        {
            hasSkill = false; 
            Jump_Skill jumpSkill = GetComponent<Jump_Skill>();
            if (jumpSkill != null)
            {
                jumpSkill.enabled = false;
                CheeseFightUI.Instance.ShowSkillUI(false, _jump);
            }
        }
    }
}
