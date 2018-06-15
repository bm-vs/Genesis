using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // Actions
    private Rotate rotate;
    private Attack[] attacks;
    private Attack selectedAttack;

    // Status
    enum State { ROTATING, BEFORE_ATTACKING, ATTACKING, AFTER_ATTACKING };
    private State state;

    // Attributes
    public float height;
    public float rotateSpeed;
    public float minRotateSpeed;
    public float maxRotateSpeed;
    public float minRotateDuration;
    public float maxRotateDuration;
    public float attackSpeed;
    public float attackCooldown;
    public float[] statesDuration;
    public float changeStateTime;


    void Start()
    {
        //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation; // disable rotation through physics

        state = State.ROTATING;

        height = gameObject.GetComponent<Renderer>().bounds.size.y;
        minRotateSpeed = 50.0f;
        maxRotateSpeed = 200.0f;
        rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        minRotateDuration = 3.0f;
        maxRotateDuration = 7.0f;

        attackSpeed = 30.0f;
        attackCooldown = 1.0f;
        statesDuration = new float[] { Random.Range(minRotateDuration, maxRotateDuration), 1.0f, 3.0f, 1.0f };
        changeStateTime = Time.time;

        rotate = new Rotate(this);
        attacks = new Attack[] { new Attack1(this), new Attack2(this), new Attack3(this) };

    }

    void Update()
    {
        if (Time.fixedTime >= changeStateTime + statesDuration[(int)state])
        {
            changeStateTime = Time.time;
            state++;
            if ((int)state == System.Enum.GetNames(typeof(State)).Length)
            {
                state = (State)0;
            }
            switch (state)
            {
                case State.ROTATING:
                    rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
                    statesDuration[(int)State.ROTATING] = Random.Range(minRotateDuration, maxRotateDuration);
                    break;
                case State.ATTACKING:
                    selectedAttack = attacks[Random.Range(0, attacks.Length)];
                    break;
            }
            if (state == State.ROTATING)
            {
            }
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.ROTATING:
                rotate.Action(Time.deltaTime);
                break;
            case State.BEFORE_ATTACKING:
                break;
            case State.ATTACKING:
                selectedAttack.Action(Time.deltaTime);
                break;
            case State.AFTER_ATTACKING:
                // deactivate all children
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    GameObject child = this.transform.GetChild(i).gameObject;
                    if (child != null && child.CompareTag("BossAttack"))
                        child.SetActive(false);
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
    }

    void OnTriggerStay(Collider other)
    {
    }

    void OnTriggerExit(Collider other)
    {
    }

    public void PlaySound(string type)
    {
        AudioSource[] sounds = gameObject.GetComponents<AudioSource>();

        switch (type)
        {
            case "Rotate":
                sounds[0].Play();
                break;
            case "Preparation":
                sounds[1].Play();
                break;
            case "Attack":
                sounds[2].Play();
                break;
        }
    }

    public void StopSound(string type)
    {
        AudioSource[] sounds = gameObject.GetComponents<AudioSource>();

        switch (type)
        {
            case "Rotate":
                sounds[0].Stop();
                break;
            case "Preparation":
                sounds[1].Stop();
                break;
            case "Attack":
                sounds[2].Stop();
                break;
        }
    }

    public bool isPlayingSound(string type)
    {
        AudioSource[] sounds = gameObject.GetComponents<AudioSource>();

        switch (type)
        {
            case "Rotate":
                return sounds[0].isPlaying;
            case "Preparation":
                return sounds[1].isPlaying;
            case "Attack":
                return sounds[2].isPlaying;
        }

        return false;
    }
}