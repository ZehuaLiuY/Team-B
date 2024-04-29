using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

public class RPCManager : MonoBehaviourPunCallbacks
{
    private bool isRPCExecuting = false;

    public void ExecuteRPC(string methodName, RpcTarget target, params object[] parameters)
    {
        if (!isRPCExecuting)
        {
            isRPCExecuting = true;
            photonView.RPC(methodName, target, parameters);
        }
        else
        {
            StartCoroutine(WaitForRPC(methodName, target, parameters));
        }
    }

    IEnumerator WaitForRPC(string methodName, RpcTarget target, object[] parameters)
    {
        while (isRPCExecuting)
        {
            yield return null; // 等待一帧
        }

        // RPC 执行完成后再次尝试调用
        ExecuteRPC(methodName, target, parameters);
    }

    public void ExecuteTargetRPC(string methodName, PhotonView targetPhotonView, params object[] parameters)
    {
        if (!isRPCExecuting)
        {
            isRPCExecuting = true;
            targetPhotonView.RPC(methodName, targetPhotonView.Owner, parameters);
        }
        else
        {
            StartCoroutine(WaitForTargetRPC(methodName, targetPhotonView, parameters));
        }
    }

    IEnumerator WaitForTargetRPC(string methodName, PhotonView targetPhotonView, object[] parameters)
    {
        while (isRPCExecuting)
        {
            yield return null; // 等待一帧
        }

        // RPC 执行完成后再次尝试调用
        ExecuteTargetRPC(methodName, targetPhotonView, parameters);
    }

    public void rpcFinished()
    {
        photonView.RPC("OnRPCExecutionFinished",RpcTarget.All);
    }

    [PunRPC]
    private void OnRPCExecutionFinished()
    {
        if (photonView.IsMine)
        {
            isRPCExecuting = false;
        }
    }
}
