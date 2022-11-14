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

    void Awake()
    {
        player = GetComponent<Rigidbody2D>();              
        anim = GetComponent<Animator>();                    
    }

    private void Start()
    {
        inputReader.OnMovement += e => movement = e;         
    }
   
    void FixedUpdate()
    {
        
        player.velocity = movement * speed;   
        
    }
}
