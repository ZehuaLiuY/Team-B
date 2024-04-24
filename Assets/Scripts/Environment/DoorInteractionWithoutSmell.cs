using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteractionWithoutSmell : MonoBehaviour
{
    public GameObject text;
    private Animator _characterAnimator;
    public AudioClip _doorOpenSound;

    private AudioSource _doorAudioSource;
    private Animator _doorAnimator;
    private bool _isPlayerNear;
    private bool _doorIsOpen;
    private PhotonView _photonView;
    private GameObject _currentVFXInstance;
    private PlayerIK _playerIK;
    private Transform _targetChild;
    private PhotonView _playerPhotonView;


    void Awake()
    {
        text.SetActive(false);
        _photonView = transform.GetComponent<PhotonView>();
        _doorAnimator = GetComponent<Animator>();
        _playerIK = GetComponent<PlayerIK>();
        _doorAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Target")) // 确保是玩家触发了这个区域
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                _isPlayerNear = true;
                text.SetActive(true);

                if (other != null)
                {
                    _playerPhotonView = other.GetComponent<PhotonView>();
                    // playerIK = other.GetComponent<PlayerIK>();
                    _characterAnimator = other.GetComponent<Animator>();
                    // targetChild = other.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/Flamethrower");
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Target"))
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                _isPlayerNear = false;
                text.SetActive(false);
                _characterAnimator = null;
            }
        }

    }

    void Update()
    {
        if (_isPlayerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (_playerPhotonView.IsMine && _playerPhotonView.CompareTag("Player"))
            {
                _playerPhotonView.RPC("SetPlayerIK_FlameThrower", RpcTarget.All, false);
                StartCoroutine(ResetAnimation(1f));
            }
            _photonView.RPC("ToggleDoor", RpcTarget.All); // Call the RPC method
            if (_characterAnimator != null)
            {
                _characterAnimator.SetTrigger("OpenDoorTrigger");
            }

            if (_doorAudioSource != null && _doorOpenSound != null)
            {
                _doorAudioSource.PlayOneShot(_doorOpenSound);
            }


        }
    }

    private IEnumerator ResetAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_playerPhotonView != null)
        {
            _playerPhotonView.RPC("SetPlayerIK_FlameThrower", RpcTarget.All, true);
        }
    }


    [PunRPC]
    void ToggleDoor()
    {
        _doorAnimator.SetBool("IsOpen", !_doorAnimator.GetBool("IsOpen")); // Toggle door state
    }

}
