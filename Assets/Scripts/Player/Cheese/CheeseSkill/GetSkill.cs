using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
public class GetSkill : MonoBehaviour
{
    private bool hasSkill = false;

    private string Invisible = "Invisible Skill";
    private string Clone = "Clone Skill";
    private string Detector = "Detector Skill";
    private string Sprint = "Sprint Skill";
    private string Jump = "Jump Skill";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Invisible) && !hasSkill)
        {
            other.gameObject.SetActive(false);
            ActivateSkill(Invisible);
            hasSkill = true;
        } else if (other.CompareTag(Clone) && !hasSkill)
        {
            other.gameObject.SetActive(false);
            ActivateSkill(Clone);
            hasSkill = true;
        } else if (other.CompareTag(Detector) && !hasSkill)
        {
            other.gameObject.SetActive(false);
            ActivateSkill(Detector);
            hasSkill = true;
        } else if (other.CompareTag(Sprint) && !hasSkill)
        {
            other.gameObject.SetActive(false);
            ActivateSkill(Sprint);
            hasSkill = true; 
        } else if (other.CompareTag(Jump) && !hasSkill)
        {
            other.gameObject.SetActive(false);
            ActivateSkill(Jump);
            hasSkill = true; 
        }
    }
    
    public void ActivateSkill(string skills)
    {
        if (skills == Invisible)
        {
            InvisibilityEffect invisibilityEffect = GetComponent<InvisibilityEffect>();
            if (invisibilityEffect != null && CheeseFightUI.Instance != null)
            {
                invisibilityEffect.enabled = true;
                CheeseFightUI.Instance.ShowSkillUI(true, Invisible); 
            }
        } else if (skills == Clone)
        {
            Clone_Skill cloneSkill = GetComponent<Clone_Skill>();
            if (cloneSkill != null && CheeseFightUI.Instance != null)
            {
                cloneSkill.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, Clone);
            }
        } else if (skills == Detector)
        {
            EnemyDetector enemyDetector = GetComponentInChildren<EnemyDetector>();
            if (enemyDetector != null && CheeseFightUI.Instance != null)
            {
                enemyDetector.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, Detector); 
            }
        } else if (skills == Sprint)
        {
            Sprint_Skill sprintSkill = GetComponent<Sprint_Skill>();
            if (sprintSkill != null && CheeseFightUI.Instance != null)
            {
                sprintSkill.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, Sprint); 
            }
        } else if (skills == Jump)
        {
            Jump_Skill jumpSkill = GetComponent<Jump_Skill>();
            if (jumpSkill != null && CheeseFightUI.Instance != null)
            {
                jumpSkill.enabled = true; 
                CheeseFightUI.Instance.ShowSkillUI(true, Jump); 
            }
        }
    }

    public void DeactivateSkill(string skills)
    {
        if (skills == Invisible)
        {
            hasSkill = false; 
            InvisibilityEffect invisibilityEffect = GetComponent<InvisibilityEffect>();
            if (invisibilityEffect != null)
            {
                invisibilityEffect.enabled = false;
                CheeseFightUI.Instance.ShowSkillUI(false, Invisible);
            }
        } else if (skills == Clone)
        {
            hasSkill = false; 
            Clone_Skill cloneSkill = GetComponent<Clone_Skill>();
            if (cloneSkill != null)
            {
                cloneSkill.enabled = false; 
                CheeseFightUI.Instance.ShowSkillUI(false, Clone); 
            }
        } else if (skills == Detector)
        {
            hasSkill = false;
            EnemyDetector enemyDetector = GetComponentInChildren<EnemyDetector>();
            if (enemyDetector != null)
            {
                enemyDetector.enabled = false; 
                CheeseFightUI.Instance.ShowSkillUI(false, Detector); 
            }
        } else if (skills == Sprint)
        {
            hasSkill = false; 
            Sprint_Skill sprintSkill = GetComponent<Sprint_Skill>();
            if (sprintSkill != null)
            {
                sprintSkill.enabled = false;
                CheeseFightUI.Instance.ShowSkillUI(false, Sprint);
            }
        } else if (skills == Jump)
        {
            hasSkill = false; 
            Jump_Skill jumpSkill = GetComponent<Jump_Skill>();
            if (jumpSkill != null)
            {
                jumpSkill.enabled = false;
                CheeseFightUI.Instance.ShowSkillUI(false, Jump);
            }
        }
    }
}
