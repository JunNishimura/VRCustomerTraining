using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAnimator : MonoBehaviour
{

    // Animator component
    private Animator animator;

    // Transition flag define
    private const string key_isRun = "isRun";
    private const string key_isJump = "isJump";

    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // Push Keyboard(DownArrow) or touch Controller(Right A)
        if (OVRInput.Get(OVRInput.RawButton.A) || Input.GetKey(KeyCode.DownArrow))
        {
            // Wait >> Run
            this.animator.SetBool(key_isRun, true);
        }
        else
        {
            this.animator.SetBool(key_isRun, false);
        }


        // Push Keyboard(Space) or touch Controller(Right B)
        if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKey(KeyCode.Space))
        {
            // Wait or Run >> Jump
            this.animator.SetBool(key_isJump, true);
        }
        else
        {
            this.animator.SetBool(key_isJump, false);
        }
    }
}