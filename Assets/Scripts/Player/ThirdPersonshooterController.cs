using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ThirdPersonshooterController : MonoBehaviour
{
    public CinemachineVirtualCamera aimVirtualCamera;
    public float normalSensitivity;
    public float aimSensitivity;
    public LayerMask aimColliderLayerMask;
    public Transform debugTransform;
    public Transform pfBulletProjectile;
    public Transform spawnBulletPosition;
    public Canvas aimPoint;
    // 添加射击冷却时间变量
    public float shootCooldown = 0.5f;
    
    private float shootCooldownTimer = 0f;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    
    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width/2f,Screen.height/2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray,out RaycastHit raycastHit,999f,aimColliderLayerMask)){
            // debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
    
        if (starterAssetsInputs.aim){
            aimVirtualCamera.gameObject.SetActive(true);
            aimPoint.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1,Mathf.Lerp(animator.GetLayerWeight(1),1f,Time.deltaTime * 10f));
    
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            
            if (starterAssetsInputs.shoot)
            {
                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
                Instantiate(pfBulletProjectile, spawnBulletPosition.position,Quaternion.LookRotation(aimDir,Vector3.up));
                starterAssetsInputs.shoot = false;
            } 
    
            transform.forward = Vector3.Lerp(transform.forward,aimDirection,Time.deltaTime*20f);
        }else{
            aimVirtualCamera.gameObject.SetActive(false);
            aimPoint.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1,Mathf.Lerp(animator.GetLayerWeight(1),0f,Time.deltaTime * 10f));
        }
    }
}
