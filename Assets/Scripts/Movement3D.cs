using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0;
    private Vector3 moveDirection = new Vector3(0, 0.2f, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void MoveTO(Vector3 direction)
    {
        moveDirection = direction;
    }
}
