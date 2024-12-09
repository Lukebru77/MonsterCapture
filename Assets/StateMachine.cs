using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour
{
    public bool Aggressive;
    public bool Cowardly;

    public GameObject cage;
    public Text StateLabel;
    public enum State
    {
        Patrol,
        Investigating,
        RunAway,
        Chasing,
        Attack,
        Captured
    }
    public State state;

    Rigidbody rb;
    PlayerController player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        NextState();
    }

    void NextState()
    {
        StateLabel.text = state.ToString();
        switch (state)
        {
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Investigating:
                StartCoroutine(InvestigatingState());
                break;
            case State.RunAway:
                StartCoroutine(RunAwayState());
                break;
            case State.Chasing:
                StartCoroutine(ChasingState());
                break;
            case State.Attack:
                StartCoroutine(AttackState());
                break;
            case State.Captured:
                break;
        }
    }

    IEnumerator PatrolState()
    {
        //Setup /entry point / Start()/Awake()
        Debug.Log("Entering Patrol State");


        while (state == State.Patrol) // "Update() loop"
        {
            transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);

            //Direction from A to B
            // B - A
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();
            //Dot product parameters should be "normalised"
            float result = Vector3.Dot(transform.forward, directionToPlayer);

            if (result >= 0.95f)
            {
                if (Aggressive == true)
                {
                    state = State.Chasing;
                }
                else if (Cowardly == true)
                {
                    state = State.RunAway;
                }
            }
            yield return null; // Wait for a frame
        }

        //tear down/ exit  point  / OnDestory()
        Debug.Log("Exiting Patrol State");
        NextState();
    }


    IEnumerator InvestigatingState()
    {
        //Setup /entry point / Start()/Awake()
        Debug.Log("Entering Investigating State");
        float startTime = Time.time;
        float deltaSum = 0;

        while (state == State.Investigating) // "Update() loop"
        {
            deltaSum += Time.deltaTime;
            yield return null; // Wait for a frame
        }

        float endTime = Time.time - startTime;
        Debug.Log("DeltaSum = " + deltaSum + " | End Time = " + endTime);

        //tear down/ exit  point  / OnDestory()
        Debug.Log("Exiting Investigating State");
        NextState();
    }
    IEnumerator RunAwayState()
    {
        //Setup /entry point / Start()/Awake()
        Debug.Log("Entering RunAway State");


        while (state == State.RunAway) // "Update() loop"
        {
            float wave = Mathf.Sin(Time.time * 20f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 20f) * 0.1f + 1f;
            transform.localScale = new Vector3(wave, wave2, wave);


            float shimmy = Mathf.Cos(Time.time * 30f) * 10f + 15f;
            //choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            //directionToPlayer.Normalize();
            float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

            if (angle > 0)
            {
                transform.rotation *= Quaternion.Euler(0f, -50f * Time.deltaTime, 0f);
            }
            else
            {
                transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            }

            if (rb.velocity.magnitude < 5f)
            {
                rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);
            }

            if (directionToPlayer.magnitude > 10f)
            {
                state = State.Patrol;
            }

            yield return new WaitForFixedUpdate(); // Wait for the next fixed up
        }


        //tear down/ exit  point  / OnDestory()
        Debug.Log("Exiting Chasing State");
        NextState();
    }

    IEnumerator ChasingState()
    {
        //Setup /entry point / Start()/Awake()
        Debug.Log("Entering Chasing State");


        while (state == State.Chasing) // "Update() loop"
        {
            float wave = Mathf.Sin(Time.time * 20f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 20f) * 0.1f + 1f;
            transform.localScale = new Vector3(wave, wave2, wave);


            float shimmy = Mathf.Cos(Time.time * 30f) * 10f + 15f;
            //choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            //directionToPlayer.Normalize();
            float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

            if (angle > 0)
            {
                transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            }
            else
            {
                transform.rotation *= Quaternion.Euler(0f, -50f * Time.deltaTime, 0f);
            }

            if (rb.velocity.magnitude < 5f)
            {
                rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);
            }

            if (directionToPlayer.magnitude < 2f)
            {
                state = State.Attack;
            }
            else if (directionToPlayer.magnitude > 10f)
            {
                state = State.Patrol;
            }

            yield return new WaitForFixedUpdate(); // Wait for the next fixed up
        }


        //tear down/ exit  point  / OnDestory()
        Debug.Log("Exiting Chasing State");
        NextState();
    }
    

    IEnumerator AttackState()
    {
        //Setup /entry point / Start()/Awake()
        Debug.Log("Entering Attack State");

        while (state == State.Attack) // "Update() loop"
        {
            Vector3 scale = transform.localScale;
            scale.z = Mathf.Cos(Time.time * 20f) * 3f + 1f;
            transform.localScale = scale;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            if (directionToPlayer.magnitude > 3f)
            {
                state = State.Chasing;
            }

            yield return null; // Wait for a frame
        }

        //tear down/ exit  point  / OnDestory()
        Debug.Log("Exiting Attack State");
        NextState();
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Cage")
        {
            cage.SetActive(true);
            state = State.Captured;

            Destroy(gameObject, 7.5f);
        }
    }
}