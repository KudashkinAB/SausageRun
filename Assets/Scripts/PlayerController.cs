using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float mass = 5.5f;
    [SerializeField]
    public float speed = 20f;
    [SerializeField]
    Rigidbody rb;
    public Transform positionBone;
    [SerializeField]
    float deathDelayTime = 1.5f;
    [SerializeField]
    Predictor predictor;
    [SerializeField]
    float maxSpeedForPush = 0.1f;
    [SerializeField]
    float minDeltaTouch = 0.0025f;
    [SerializeField]
    float minPullDistance = 0.05f;
    [SerializeField]
    float minPositionDelta = 0.05f;

    bool isDead = false;
    bool pulled = false;
    bool mousePulled = false;

    Vector3 playerScreenPosition;
    Vector3 lastPredictionPullPosition;
    Vector3 playerLastPosition;
    Vector3 mousePosition;


    void Start()
    {
    }

    void Update()
    {
        playerScreenPosition = Camera.main.WorldToScreenPoint(positionBone.transform.position) / new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        mousePosition = Input.mousePosition / new Vector2(Screen.width, Screen.height);
        #region Touch
        if (Input.touchCount == 1)
        {
            Vector3 touchPosition = (Vector3)Input.GetTouch(0).position / new Vector2(Screen.width, Screen.height);

            if (!pulled && (touchPosition - playerScreenPosition).magnitude <= minPullDistance)
            {
                pulled = true;
            }

            if (pulled && rb.velocity.magnitude <= maxSpeedForPush && (touchPosition - playerScreenPosition).magnitude >= minPullDistance)
            {
                if ((touchPosition - lastPredictionPullPosition).magnitude >= minDeltaTouch
                    || (playerLastPosition - positionBone.transform.position).magnitude >= minPositionDelta)
                {
                    Trajectory((Input.GetTouch(0).rawPosition - Input.GetTouch(0).position) / new Vector2(Screen.width, Screen.height));
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    pulled = false;
                    Push((playerScreenPosition - touchPosition) * speed);
                }
            }
            else
            {
                pulled = false;
            }
        }
        else
        {
            pulled = false;
        }

        #endregion

        #region Mouse
        if (!mousePulled && Input.GetMouseButtonDown(0) && rb.velocity.magnitude <= maxSpeedForPush && (mousePosition - playerScreenPosition).magnitude <= minPullDistance)
        {
            mousePulled = true;
            lastPredictionPullPosition = mousePosition;
            playerLastPosition = positionBone.transform.position;
        }

        if (mousePulled && rb.velocity.magnitude <= maxSpeedForPush)
        {
            if (Input.GetMouseButton(0))
            {
                if ((mousePosition - playerScreenPosition).magnitude >= minPullDistance
                    && (mousePosition - lastPredictionPullPosition).magnitude >= minDeltaTouch
                        || (playerLastPosition - positionBone.transform.position).magnitude >= minPositionDelta)
                {
                    lastPredictionPullPosition = mousePosition;
                    playerLastPosition = positionBone.transform.position;
                    Trajectory((playerScreenPosition - mousePosition) * speed);

                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                mousePulled = false;
                Push((playerScreenPosition - mousePosition) * speed);
            }
        }
        else
        {
            mousePulled = false;
        }
        #endregion

        if (!pulled && !mousePulled)
        {
            predictor.Stop();
        }

        if (positionBone.position.y <= 0 && !isDead)
        {
            Death();
        }
    }

    public void Trajectory(Vector3 startSpeed)
    {
        if (UIController.simulation)
            predictor.Simulate(this, transform.position, startSpeed);
        if (UIController.parabola)
            predictor.Parabola(positionBone.transform.position, 9.8f, startSpeed);
    }

    public void Push(Vector3 startSpeed)
    {
        rb.AddForce(startSpeed * 5.5f, ForceMode.Impulse);
    }

    public Vector3 GetPlayerVelocity()
    {
        return rb.velocity;
    }

    public void Death()
    {
        isDead = true;
        Camera.main.GetComponent<CameraController>().enabled = false;
        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(deathDelayTime);
        UIController.ui.GameOver();
    }
}
