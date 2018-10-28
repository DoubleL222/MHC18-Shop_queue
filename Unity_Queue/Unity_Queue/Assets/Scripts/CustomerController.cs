using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState
{
    Shopping,
    GoingToLine,
    InLine,
    Leaving
}

public class CustomerController : MonoBehaviour {

    [HideInInspector]
    private float shoppingTime = 60;

    static readonly float shoppingDelay = 8.0f;
    float nextShoppingPosition = float.MinValue;

    NavMeshAgent myAgent;

    Cashier myCashier;

    bool arrivedAtLine = false;

    private CustomerState myState;

	// Use this for initialization
	void Start () {
        myState = CustomerState.Shopping;
        myAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        if (myState == CustomerState.Shopping)
        {

            if (nextShoppingPosition <= Time.time)
            {
                myAgent.SetDestination(StoreManager.instance.GetRandomShoppingPosition());
                nextShoppingPosition = Time.time + shoppingDelay;
            }
            shoppingTime -= Time.deltaTime;
            if (shoppingTime <= 0)
            {
                myState = CustomerState.GoingToLine;
                myCashier = StoreManager.instance.GetBestLinePosition();
                myAgent.SetDestination(myCashier.GetPositionInLine());
                myCashier.AddCustomerToLine(this);
                arrivedAtLine = false;
            }
        }
        else if (myState == CustomerState.GoingToLine)
        {
            float dist = myAgent.remainingDistance;

            if (dist != Mathf.Infinity && myAgent.pathStatus == NavMeshPathStatus.PathComplete && myAgent.remainingDistance == 0)
            {
                Debug.Log("Reached destination");
                arrivedAtLine = true;
                myState = CustomerState.InLine;
            }
        }
        else if (myState == CustomerState.InLine)
        {

        }
        else if (myState == CustomerState.Leaving)
        {

        }
	}

    public void SetNewDestinationInLine(Vector3 _newDestinationInLine)
    {
        myAgent.SetDestination(_newDestinationInLine);
    }

    public bool HasReachedDestination()
    {
        float dist = myAgent.remainingDistance;

        if (dist != Mathf.Infinity && myAgent.pathStatus == NavMeshPathStatus.PathComplete && myAgent.remainingDistance == 0)
        {
            return true;
        }
        return false;
    }

    public CustomerState getState()
    {
        return myState;
    }

    public void SetShoppingTime(float _st)
    {
        shoppingTime = _st;
    }

    public void ChangeCashier(Cashier newCashier)
    {
        myCashier = newCashier;
        myAgent.SetDestination(myCashier.GetPositionInLine());
        arrivedAtLine = false;
        myState = CustomerState.GoingToLine;
    }

    public void Leave()
    {
        myAgent.SetDestination(StoreManager.instance.GetExit());
        Destroy(this.gameObject, 2.0f);
    }
}
