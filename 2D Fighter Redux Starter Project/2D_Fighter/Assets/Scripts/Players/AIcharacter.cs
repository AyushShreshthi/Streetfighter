using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIcharacter : MonoBehaviour
{
    #region Variables

    StateManager states;
    public StateManager enStates;

    public float changeStateTolerance = 3;

    public float normalRate = 1;
    float nrmTimer;

    public float closeRate = 0.5f;
    float clTimer;

    public float blockingRate = 1.5f;
    float blTimer;

    public float aiStateLife = 1;
    float aiTimer;

    bool inititateAI;
    bool closeCombat;

    bool gotRandom;
    float storeRandom;

    bool checkForBlocking;
    bool blocking;
    float blockMultiplier;

    bool randomizeAttacks;
    int numberOfAttacks;
    int curNumAttacks;

    public float JumpRate = 1;
    float jRate;
    bool jump;
    float jTimer;


    #endregion

    public AttackPatterns[] attackPatterns;

    public enum AIState
    {
        closeState,
        normalState,
        resetAI
    }
    public AIState aiState;

    private void Start()
    {
        states = GetComponent<StateManager>();

        AIsnapShots.GetInstance().RequestAISnapShot(this);
    }

    private void Update()
    {
        CheckDistance();
        States();
        AIAgent();
    }
    void States()
    {
        switch (aiState)
        {
            case AIState.closeState:
                CloseState();
                break;
            case AIState.normalState:
                NormalState();
                break;
            case AIState.resetAI:
                ResetAI();
                break;
        }


        //Blocking();
        Jumping();
    }

    private void Jumping()
    {
        if (!enStates.onGround)
        {
            float ranValue = ReturnRandom();

            if (ranValue < 50)
            {
                jump = true;
            }

        }
        if (jump)    //  || !enStates.onGround ||
        {
            states.vertical = 1;
            jRate = ReturnRandom();
            jump = false;
        }
        else
        {
            states.vertical = 0;
        }

        jTimer += Time.deltaTime;

        if (jTimer > JumpRate * 10)
        {
            

            if (jRate < 50)
            {
                jump = true;
            }
            else
            {
                jump = false;
            }

            jTimer = 0;
        }
    }

    private void ResetAI()
    {
        aiTimer += Time.deltaTime;

        if (aiTimer > aiStateLife)
        {
            inititateAI = false;
            states.horizontal = 0;
            states.vertical = 0;
            aiTimer = 0;

            gotRandom = false;

            storeRandom = ReturnRandom();
            if (storeRandom < 50)
                aiState = AIState.normalState;
            else
                aiState = AIState.closeState;

            curNumAttacks = 1;
            randomizeAttacks = false;

        }
    }

    private void NormalState()
    {
        nrmTimer += Time.deltaTime;

        if (nrmTimer > normalRate)
        {
            inititateAI = true;
            nrmTimer = 0;
        }
    }

    private void CloseState()
    {
        clTimer += Time.deltaTime;

        if (clTimer > closeRate)
        {
            clTimer = 0;
            inititateAI = true;
        }
    }

    void CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, enStates.transform.position);

        if (distance < changeStateTolerance)
        {
            if (aiState != AIState.resetAI)
                aiState = AIState.closeState;

            closeCombat = true;
        }
        else
        {
            if (aiState != AIState.resetAI)
                aiState = AIState.normalState;

            if (closeCombat)
            {
                if (!gotRandom)
                {
                    storeRandom = ReturnRandom();
                    gotRandom = true;
                }

                if (storeRandom < 60)
                {
                    Movement();
                }

            }

            closeCombat = false;
        }
    }
    void AIAgent()
    {
        if (inititateAI)
        {
            aiState = AIState.resetAI;

            float multiplier = 0;

            if (!gotRandom)
            {
                storeRandom = ReturnRandom();
                gotRandom = true;
            }

            if (!closeCombat)
            {
                //we have 30% more chance of moving
                multiplier += 30;
            }
            else
            {
                // we have 30% more chance to attack
                multiplier -= 30;
            }

            if (storeRandom + multiplier < 50)
            {
                Attack();//Attack
            }
            else
            {
                Movement();
            }
        }
    }
    void Attack()
    {
        if (!gotRandom)
        {
            storeRandom = ReturnRandom();
            gotRandom = true;
        }

        //if(storeRandom<75)
        //{
            //See how many attacks he will do
            if (!randomizeAttacks)
            {
                numberOfAttacks = (int)Random.Range(1, 4);
                randomizeAttacks = true;
            }

            if (curNumAttacks < numberOfAttacks)
            {
                int attackNumber = Random.Range(0,attackPatterns.Length);

                StartCoroutine(OpenAttack(attackPatterns[attackNumber], 0));

                curNumAttacks++;

            }

        //}
        //else   special one attack
        //{
        //    if (curNumAttacks < 1)
        //    {
        //        //states.SpecialAttack = true;
        //        curNumAttacks++;
        //    }
        //}
    }
    void Movement()
    {
        if (!gotRandom)
        {
            storeRandom = ReturnRandom();
            gotRandom = true;
        }

        if (storeRandom < 90)  // 90% chances to move close to player
        {
            if (enStates.transform.position.x < transform.position.x)
            {
                states.horizontal = -1;
            }
            else
            {
                states.horizontal = 1;
            }
        }
        else  // and then move away
        {
            if (enStates.transform.position.x < transform.position.x)
            {
                states.horizontal = 1;
            }
            else
            {
                states.horizontal = -1;
            }
        }
        // can create a modifier based from health to manipulate
    }
    void Blocking()
    {
        if (states.gettingHit)
        {
            if (!gotRandom)
            {
                storeRandom = ReturnRandom();
                gotRandom = true;
            }

            if (storeRandom < 50)
            {
                blocking = true;
                states.gettingHit = false;
                //states.blocking = true;
            }
        }
        if (blocking)
        {
            blTimer += Time.deltaTime;

            if (blTimer > blockingRate)
            {
                blTimer = 0;
            }
        }
    }
    float ReturnRandom()
    {
        float retVal = Random.Range(0, 101);
        return retVal;
    }

    IEnumerator OpenAttack(AttackPatterns a, int i)
    {
        int index = i;
        float delay = a.attacks[index].delay;
        states.attack1 = a.attacks[index].attack1;
        states.attack2 = a.attacks[index].attack2;
        yield return new WaitForSeconds(delay);

        states.attack1 = false;
        states.attack2 = false;

        if (index < a.attacks.Length - 1)
        {
            index++;
            StartCoroutine(OpenAttack(a, index));
        }

    }


    [System.Serializable]
    public class AttackPatterns
    {
        public AttackBase[] attacks;
    }
    [System.Serializable]
    public class AttackBase
    {
        public bool attack1;
        public bool attack2;
        public float delay;
    }
}


