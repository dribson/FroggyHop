using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.Tween;
using UnityEngine.UI;

public class Froggy : MonoBehaviour
{

    Rigidbody2D RB;
    Collider2D col;
    Vector2 HopForce;
    bool airborne = false, checkForCol = true;
    [SerializeField] Transform CameraTransform, JumpTo;
    System.Action<ITween<Vector2>> TweenHopTo;
    Vector3 rightPos, leftPos;
    [SerializeField] Text HeightText, ScoreText;
    float maxHeight, score, savedHeight;
    bool isPlaying = false;
    [SerializeField] GameController GC;
    Touch tap;
    [SerializeField] [Range(20, 80)] float JumpToScreenPct = 50f;
    SpriteRenderer SR, hatSR;
    [SerializeField] ParticleSystem Trail;


    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        HopForce = new Vector2(50, 550);
        col = GetComponent<Collider2D>();
        SR = GetComponent<SpriteRenderer>();
        hatSR = transform.Find("HatSprite").GetComponent<SpriteRenderer>();
        //Trail.Stop();
    }

    public void StartGame()
    {
        StopAllCoroutines();
        ResetPositions();
        isPlaying = true;
        score = maxHeight = savedHeight = 0;
        HeightText.text = "Height: 0 ft";
        ScoreText.text = "Points: 0";
        JumpTo.transform.position = transform.position + new Vector3(0.5f, 1, -1);
        JumpTo.GetComponent<SpriteRenderer>().enabled = false;
        Trail.Clear();
        Trail.Play();
        //StartCoroutine(RepeatTween());
    }

    private void Update()
    {
        if (isPlaying)
        {
            //JumpTo.position = new Vector2(transform.position.x, transform.position.y + 1);
            
            if (!airborne)
            {
                #region EditorControls
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    JumpTo.position = new Vector2(transform.position.x, transform.position.y + 1);
                    UpdateJumpToLoc(tap.position);
                    BeginEndHop(true);
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    Hop();
                    BeginEndHop(false);
                }
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    UpdateJumpToLoc(Input.mousePosition);
                }
#endif
                #endregion
                if (Input.touchCount > 0)
                {
                    tap = Input.GetTouch(0);
                    switch (tap.phase)
                    {
                        case (TouchPhase.Began):
                            UpdateJumpToLoc(tap.position);
                            BeginEndHop(true);
                            break;
                        case (TouchPhase.Moved):
                            UpdateJumpToLoc(tap.position);
                            break;
                        case (TouchPhase.Ended):
                            Hop();
                            BeginEndHop(false);
                            break;
                    }
                }
            }

            if (RB.velocity.y < 0 && airborne)
            {
                // TODO replace this check with some raycasts down to reenable collision only when above a platform we *should* land on
                col.isTrigger = false;
                checkForCol = true;
            }

            HeightText.text = "Height: " + transform.position.y.ToString("F1") + " ft";
            if (transform.position.y > maxHeight)
            {
                maxHeight = transform.position.y;
            }

        }
    }

    private void LateUpdate()
    {
        if (isPlaying)
        {
            
            CameraTransform.position = new Vector3(transform.position.x, CameraTransform.position.y, CameraTransform.position.z);

            if (transform.position.y > CameraTransform.position.y)
            {
                CameraTransform.position = new Vector3(CameraTransform.position.x, transform.position.y, CameraTransform.position.z);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (checkForCol && isPlaying)
        {
            score += 3*(transform.position.y - savedHeight);
            savedHeight = transform.position.y;
            ScoreText.text = "Points: " + score.ToString("F0");
            airborne = false;
            JumpTo.transform.position = transform.position + new Vector3(0.5f, 1, -1);
            GC.CheckGenNewSectors();
            //StartCoroutine(RepeatTween());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable" && isPlaying)
        {
            score += collision.GetComponent<Collectable>().GainPoints();
            ScoreText.text = "Points: " + score.ToString("F0");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "KILL")
        {
            JumpTo.GetComponent<SpriteRenderer>().enabled = false;
            isPlaying = false;
            col.enabled = false;
            GC.EndGame(score);
            Trail.Clear();
            Trail.Stop();
        }
    }

    public void Hop()
    {
        if (!airborne && isPlaying)
        {
            score += 4;
            ScoreText.text = "Points: " + score.ToString("F0");
            HopForce = new Vector2(300 * (JumpTo.position.x - transform.position.x), 535);
            checkForCol = false;
            RB.AddForce(HopForce);
            col.isTrigger = true;
            airborne = true;
            JumpTo.GetComponent<SpriteRenderer>().enabled = false;
            //StopAllCoroutines();
            //TweenFactory.Clear();
            if (Trail.isStopped)
                Trail.Play();
        }
    }

    
    void TweenHopFunc()
    {
        TweenHopTo = (t) =>
        {
            JumpTo.position = t.CurrentValue;
        };
        rightPos = JumpTo.position;
        leftPos = rightPos - new Vector3(1, 0, -1);
        JumpTo.gameObject.Tween("TweenHop", rightPos, leftPos, 1.5f, TweenScaleFunctions.CubicEaseInOut, TweenHopTo)
            .ContinueWith(new Vector2Tween().Setup(leftPos, rightPos, 1.5f, TweenScaleFunctions.CubicEaseInOut, TweenHopTo));
    }

    IEnumerator RepeatTween()
    {
        TweenHopFunc();
        yield return new WaitForSeconds(3f);
        StartCoroutine(RepeatTween());
    }
    

    public void ResetPositions()
    {
        RB.velocity = Vector3.zero;
        col.enabled = true;
        transform.position = new Vector3(0, 0);
        CameraTransform.position = new Vector3(0, 3, -10);
    }

    void BeginEndHop(bool began)
    {
        JumpTo.GetComponent<SpriteRenderer>().enabled = began;
    }

    void UpdateJumpToLoc(Vector2 touchPos)
    {
        float screenTapPosPct = (touchPos.x / Screen.width);
        float jumpToPosX = ((transform.position.x - 0.5f) + screenTapPosPct);
        JumpTo.position = new Vector2(jumpToPosX, transform.position.y + 1);
        JumpTo.GetComponent<SpriteRenderer>().enabled = true;
        speen(screenTapPosPct > 0.5f);
    }

    void speen(bool speen)
    {
        SR.flipX = speen;
        hatSR.flipX = speen;
    }

    
    
}
