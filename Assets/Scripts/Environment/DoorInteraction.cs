using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
using UnityEngine.InputSystem; // 确保导入了Input System命名空间
using UnityEngine.UI;
using StarterAssets;
using Photon.Pun;
using UnityEngine.VFX;
using UnityEngine.InputSystem;


public class DoorInteraction : MonoBehaviour
{
    public GameObject text; 
    public GameObject vfxSmell;
    private Animator _characterAnimator;
    public AudioClip _doorOpenSound;
    
    
// #if ENABLE_INPUT_SYSTEM
//     private PlayerInput _playerInput;
// #endif
    private StarterAssetsInputs _input;
    private AudioSource _doorAudioSource; 
    private Animator _doorAnimator;
    private bool _isPlayerNear;
    private bool _doorIsOpen;
    private PhotonView _photonView;
    private GameObject _currentVFXInstance; 
    private PlayerIK _playerIK;
    private Transform _targetChild;
    private PhotonView _playerPhotonView;

    private bool _cheeseInSide = false;
    private CheckCheeseInside _checkCheeseInside;

    void Awake()
    {
    }

    private void Start()
    {
        //_vfxSmell.SetActive(false);
        GameObject detector = GameObject.FindWithTag("Detector");
        _checkCheeseInside = detector.GetComponent<CheckCheeseInside>();
        text.SetActive(false); 
        _photonView = transform.GetComponent<PhotonView>();
        _doorAnimator = GetComponent<Animator>();
        _playerIK = GetComponent<PlayerIK>();
        _doorAudioSource = GetComponent<AudioSource>();
        _input = GetComponent<StarterAssetsInputs>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Target") ) // 确保是玩家触发了这个区域
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                _isPlayerNear = true;
                text.SetActive(true);
                
                if(other != null)
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
        checkCheese();
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
            
            //Debug.Log("_cheeseInSide: " + _cheeseInSide);
            if (_doorAnimator.GetBool("IsOpen") && _cheeseInSide)
            {
                _photonView.RPC("PlayVFX", RpcTarget.All);
            }
            else
            {
                _photonView.RPC("StopVFX", RpcTarget.All);
            }
        }
        
        if (!_cheeseInSide && _currentVFXInstance != null)
        {
            _photonView.RPC("StopVFX", RpcTarget.All);
        }
        else if(_cheeseInSide && _currentVFXInstance == null && _doorAnimator.GetBool("IsOpen"))
        {
            _photonView.RPC("PlayVFX", RpcTarget.All);
        }
        //else if(_cheeseInSide && currentVFXInstance == null)
        //{
        //    photonView.RPC("PlayVFX", RpcTarget.All);
        //}
    }
    
    private IEnumerator ResetAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_playerPhotonView != null) {
            _playerPhotonView.RPC("SetPlayerIK_FlameThrower", RpcTarget.All, true);
        }
    }

    private void checkCheese()
    {
        if (_checkCheeseInside != null) {
            _cheeseInSide = _checkCheeseInside.isCheeseInside;
        }
    }

    [PunRPC]
    void ToggleDoor() {

        _doorAnimator.SetBool("IsOpen", !_doorAnimator.GetBool("IsOpen")); // Toggle door state
    }

    [PunRPC]
    void PlayVFX()
    {
        // instantiate visual effect
        _currentVFXInstance = PhotonNetwork.Instantiate("VFXSmell", transform.position, Quaternion.identity);

        // let the visual effect play
        //currentVFXInstance.GetComponent<VisualEffect>().Play();
    }

    [PunRPC]
    void StopVFX()
    {
        if (_currentVFXInstance != null)
        {
            // if the visual effect is playing, stop
            //currentVFXInstance.GetComponent<VisualEffect>().Stop();
            PhotonNetwork.Destroy(_currentVFXInstance);
            _currentVFXInstance = null;
        }
    }
}
