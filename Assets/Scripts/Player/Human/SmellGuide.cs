
using Photon.Pun;
using UnityEngine;


public class SmellGuide : MonoBehaviourPun
{
    private GameObject _currentSmellEffect;

    private GameObject _leftSmell;
    private GameObject _rightSmell;
    private GameObject _bottomSmell;


    private GameObject UICamera;

    private void Start()
    {
        UICamera = GameObject.Find("UICamera");
        
        _leftSmell = UICamera.transform.Find("LeftSmell").gameObject;
        _rightSmell = UICamera.transform.Find("RightSmell").gameObject;
        _bottomSmell = UICamera.transform.Find("BottomSmell").gameObject;

        _currentSmellEffect = UICamera.transform.Find(transform.name).gameObject;
        

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Target"))
        {
            if (photonView.IsMine && !_leftSmell.activeSelf && !_rightSmell.activeSelf && !_bottomSmell.activeSelf)
            {
                _currentSmellEffect.SetActive(true);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            if(photonView.IsMine)
            {
                _currentSmellEffect.SetActive(false);
            }
        }

    }
}
