﻿using UnityEngine;

public class DifferentialDriveControl : MonoBehaviour
{
    public float R=0;
    public float L = 0.7f;

    public Rigidbody m_Rigidbody;
  
    public float V_L = 0;
    public float V_R = 0;

    // x, y, and θ is the pose of the wheelchair
    private float x, y;
    private float theta;

    private float drivingTime;

    private void Start()
    {
        x = gameObject.transform.position.x;
        y = gameObject.transform.position.y;
        theta = Mathf.PI / 2;

        drivingTime = 1f;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            V_L = 0.2f;
            V_R = 0.1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            V_L = 0.1f;
            V_R = 0.2f;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            V_L = 0.2f;
            V_R = 0.2f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            V_L = -0.2f;
            V_R = -0.2f;
        }
    }

    private void FixedUpdate()
    {

        (x, y, theta) = diffdrive(V_L, V_R, drivingTime, L);

        Debug.Log($"X: {x}, Y: {y} Theta: {theta}");

        Move();
        Turn();

        V_L = 0;
        V_R = 0;

    }

    private void Move()
    {
        Vector3 movement = new Vector3(x,gameObject.transform.position.y, y);
        m_Rigidbody.MovePosition(movement);
    }

    private void Turn()
    {
        float theta_deg = -Mathf.Rad2Deg * theta + 90f;
        Quaternion rotate = Quaternion.Euler(0f, theta_deg, 0f);
        m_Rigidbody.MoveRotation(rotate);
        
    }

    /**
     *  vl and vr are the speed of the left and right wheel
     *  t is the driving time
     *  l is the distance between the wheels of the robot. 
     *  The output of the function is the new pose of the robot xn, yn, and θn.
     *  reference: http://ais.informatik.uni-freiburg.de/teaching/ss17/robotics/exercises/solutions/03/sheet03sol.pdf
     */
    (float, float, float) diffdrive(float v_l, float v_r, float t, float l)
    {
        float x_n, y_n;
        float theta_n;

        // Straight line
        if (v_l == v_r)
        {
            theta_n = theta;
            x_n = x + v_l * t * Mathf.Cos(theta);
            y_n = y + v_l * t * Mathf.Sin(theta);
           
        }
        else  // Circular motion
        {
            //  Calculate the radius
            R = l / 2.0f * ((v_l + v_r) / (v_r - v_l));

            // Computing center of curvature
            float ICC_x = x - R * Mathf.Sin(theta);
            float ICC_y = y + R * Mathf.Cos(theta);

            // Compute the angular velocity
            float omega = (v_r - v_l) / l;

            // Computing angle change
            float dtheta = omega * t;

            // Forward kinematics for differential drive
            x_n = Mathf.Cos(dtheta) * (x - ICC_x) - Mathf.Sin(dtheta) * (y - ICC_y) + ICC_x;
            y_n = Mathf.Sin(dtheta) * (x - ICC_x) + Mathf.Cos(dtheta) * (y - ICC_y) + ICC_y;
            theta_n = theta + dtheta;

        }
        return (x_n, y_n, theta_n);
    }
}