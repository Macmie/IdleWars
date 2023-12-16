using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask _walkableAreaMask;
    [SerializeField] private Camera _mainCamera;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            var mousePosition = Input.mousePosition;

            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _walkableAreaMask))
            {
                Debug.Log($"Hit: {hit.point}");
                _agent.SetDestination(hit.point);
            }
        }
    }
}
