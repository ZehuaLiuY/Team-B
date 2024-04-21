using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class Clone_Skill : MonoBehaviourPunCallbacks
{
    public float skillDuration = 10f; // Clone duration
    private bool _skillUsed;
    private float _skillTimer; // Timer to track skill duration

    private void Start()
    {
        _skillUsed = false;
    }

    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ApplyCloneEffect());
        }

        // Continuously update the skill icon if the skill has been used
        if (_skillUsed)
        {
            UpdateIcon(_skillTimer / skillDuration);
        }
    }

    IEnumerator ApplyCloneEffect()
    {
        _skillUsed = true; // Mark the skill as used

        Clone(); // Create the clone

        _skillTimer = skillDuration; // Reset the skill timer

        while (_skillTimer > 0)
        {
            _skillTimer -= Time.deltaTime;
            UpdateIcon(_skillTimer / skillDuration);
            yield return null;
        }
        
        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Clone Skill"); 
        _skillUsed = false;
    }

    void Clone()
    {
         Debug.Log("Attempting to clone character.");
         // GameObject clone = PhotonNetwork.Instantiate(this.gameObject.name.Replace("(Clone)",""), transform.position, transform.rotation);
         GameObject clone = PhotonNetwork.Instantiate("Clone", transform.position, transform.rotation);
         var cloneMovement = clone.AddComponent<CloneMovement>();
         StartCoroutine(DestroyNetworkObject(clone, skillDuration));
    }

    IEnumerator DestroyNetworkObject(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(target);
    }
    private void UpdateIcon(float fillAmount)
    {
        if (CheeseFightUI.Instance != null)
        {
            CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
        }
    }
}

