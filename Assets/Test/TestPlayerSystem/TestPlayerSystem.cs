using System.Collections;
using System.Collections.Generic;
using ResetCore.GameSystem;
using UnityEngine;

public class TestPlayerSystem : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    TestTimePlayer player1 = new TestTimePlayer("A");
	    player1.Init(new TimePlayerData(0.5f, 0, 1));

	    TestTimePlayer player2 = new TestTimePlayer("B");
	    player2.Init(new TimePlayerData(1, 0, 5));

        SkillPlayer skillPlayer = new SkillPlayer();
	    skillPlayer.Init(new BasePlayerData(1, 0), new BasePlayer[]
	    {
	        player1, player2
        });

        //skillPlayer.Start();
        Debug.Log(skillPlayer.ToJson());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
