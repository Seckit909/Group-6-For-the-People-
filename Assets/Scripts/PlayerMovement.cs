using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D player;
    private Animator anim;

    [SerializeField] Vector2 movement;                                                                
    [SerializeField] float speed = 5f;
    [SerializeField] InputReader inputReader;
    [SerializeField] PlayerData playerData;

    void Awake()
    {
        player = GetComponent<Rigidbody2D>();              
        anim = GetComponent<Animator>();                    
    }

    private void Start()
    {
        playerData.ResetPlayerPosition();
        inputReader.OnMovement += e => movement = e;         
    }

    private void Update()
    {
        playerData.PlayerPosition = player.position;
    }

    void FixedUpdate()
    {
        
        player.velocity = movement * speed;   
        
    }
}
