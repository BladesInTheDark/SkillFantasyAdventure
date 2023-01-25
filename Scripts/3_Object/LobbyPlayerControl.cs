using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 로비에 있는 플레이어가 랜덤한 곳으로 움직이게 하기 위한 클래스
public class LobbyPlayerControl : MonoBehaviour
{
    // x는 7~-7
    // y는 1.5~-1.1
    Vector2 startPos = new Vector2(0, -0.4f);
    [SerializeField] Vector2 startVec2 = new Vector2(-7f, -1.1f);
    [SerializeField] Vector2 endVec2 = new Vector2(7f, 1.5f);
    Quaternion leftSee;
    Quaternion rightSee;
    Vector2 goalDestination;
    float timer = 0;
    [SerializeField] float minDelay = 2f;
    [SerializeField] float maxDelay = 7f;
    float delay = 3;
    bool moving = false;
    [SerializeField] float movSpeed;

    [SerializeField] Animator ownAnim;
    [SerializeField] RectTransform nameTextTf;

    private void Start()
    {
        transform.position = startPos;
        rightSee = Quaternion.Euler(new Vector3(0, 0, 0));
        leftSee = Quaternion.Euler(new Vector3(0, 180, 0));
        ownAnim.speed = 0.7f;
    }
    private void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalDestination, movSpeed);
            if (transform.position.x == goalDestination.x && transform.position.y == goalDestination.y)
            {
                moving = false;
                ownAnim.SetBool("Move", false);
            }
            return;
        }
        timer += Time.deltaTime;
        if (timer > delay)
        {
            MoveTo();
            delay = Random.Range(minDelay, maxDelay);
            timer = 0;
        }
    }

    public void MoveTo()
    {
        goalDestination = new Vector2(Random.Range(startVec2.x, endVec2.x), Random.Range(startVec2.y, endVec2.y));
        float mag = new Vector2(goalDestination.x - transform.position.x, goalDestination.y - transform.position.y).magnitude;
        while (mag < 3f)
        {
            goalDestination = new Vector2(Random.Range(startVec2.x, endVec2.x), Random.Range(startVec2.y, endVec2.y));
            mag = new Vector2(goalDestination.x - transform.position.x, goalDestination.y - transform.position.y).magnitude;
        }
        moving = true;
        ownAnim.SetBool("Move", true);
        if (goalDestination.x - transform.position.x > 0)
        {
            transform.rotation = rightSee;
            nameTextTf.localRotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = leftSee;
            nameTextTf.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }
}
