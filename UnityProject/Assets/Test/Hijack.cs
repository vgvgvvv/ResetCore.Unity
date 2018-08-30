using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class Hijack : MonoBehaviour
{

    //This will hold the counting up coroutine
    IEnumerator _countUp;
    //This will hold the counting down coroutine
    IEnumerator _countDown;
    //This is the coroutine we are currently
    //hijacking
    IEnumerator _current;

    //A value that will be updated by the coroutine
    //that is currently running
    int value = 0;

    void Start()
    {
        //Create our count up coroutine
        _countUp = CountUp();
        //Create our count down coroutine
        _countDown = CountDown();
        //Start our own coroutine for the hijack
        StartCoroutine(DoHijack());
    }

    void Update()
    {
        //Show the current value on the screen
        Debug.unityLogger.Log(value.ToString());
    }

    void OnGUI()
    {
        //Switch between the different functions
        if (GUILayout.Button("Switch functions"))
        {
            if (_current == _countUp)
                _current = _countDown;
            else
                _current = _countUp;
        }
    }

    IEnumerator DoHijack()
    {
        while (true)
        {
            //Check if we have a current coroutine and MoveNext on it if we do
            if (_current != null && _current.MoveNext())
            {
                //Return whatever the coroutine yielded, so we will yield the
                //same thing
                yield return _current.Current;
            }
            else
                //Otherwise wait for the next frame
                yield return null;
        }
    }

    IEnumerator CountUp()
    {
        //We have a local increment so the routines
        //get independently faster depending on how
        //long they have been active
        float increment = 0;
        while (true)
        {
            //Exit if the Q button is pressed
            if (Input.GetKey(KeyCode.Q))
                break;
            increment += Time.deltaTime;
            value += Mathf.RoundToInt(increment);
            yield return null;
        }
    }

    IEnumerator CountDown()
    {
        float increment = 0f;
        while (true)
        {
            if (Input.GetKey(KeyCode.Q))
                break;
            increment += Time.deltaTime;
            value -= Mathf.RoundToInt(increment);
            //This coroutine returns a yield instruction
            yield return new WaitForSeconds(0.1f);
        }
    }

}