using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cashier : MonoBehaviour {

    private static readonly float servingTime = 2.0f;

    public enum CashierState
    {
        checking,
        serving
    }

    [HideInInspector]
    public List<CustomerController> CustomersInLine = new List<CustomerController>();

    private bool isManed;

    private float serving = servingTime;

    private MeshRenderer myRenderer;

    Material mannedCashier, cashier;

    CashierState myState = CashierState.checking;

    public bool IsManed
    {
        get
        {
            return isManed;
        }

        set
        {
            isManed = value;
            if (isManed)
            {
                myRenderer.material = mannedCashier;
                myState = CashierState.checking;
            }
            else
            {
                myRenderer.material = cashier;
            }
        }
    }

    // Use this for initialization
    void Start () {
        myRenderer = GetComponent<MeshRenderer>();
        mannedCashier = Resources.Load("MannedCashier") as Material;
        cashier = Resources.Load("Cashier") as Material;
        IsManed = false;
	}

    public Vector3 GetPositionInLine()
    {
        Vector3 pos = transform.position;
        pos.x -= 1;
        pos.z += (CustomersInLine.Count) * 1.5f;

        return pos;
    }

    public int GetLine()
    {
        return CustomersInLine.Count;
    }

    public bool AllReached()
    {
        bool reached = true;
        foreach (CustomerController c in CustomersInLine)
        {
            if (c.getState() != CustomerState.InLine)
            {
                reached = false;
            }
        }
        return reached;
    }

    public void AddCustomerToLine(CustomerController cc)
    {
        CustomersInLine.Add(cc);
    }

    public void PopFromLine(CustomerController cc)
    {
        CustomersInLine.Remove(cc);
    }

    public void BreakLine(Cashier nextCashier, int lineBreak)
    {
        for (int i = lineBreak; i < CustomersInLine.Count; i++)
        {
            CustomersInLine[i].ChangeCashier(nextCashier);
            nextCashier.AddCustomerToLine(CustomersInLine[i]);
        }
        CustomersInLine.RemoveRange(lineBreak, CustomersInLine.Count - lineBreak);
    }

    public Vector3 GetPositionInLineById(int _id)
    {
        Vector3 pos = transform.position;
        pos.x -= 1;
        pos.z += (_id) * 1.5f;
        return pos;
    }

	// Update is called once per frame
	void Update () {
        if (IsManed)
        {
            if (myState == CashierState.checking)
            {
                if (CustomersInLine.Count > 0)
                {
                    if (CustomersInLine[0].HasReachedDestination())
                    {
                        myState = CashierState.serving;
                        serving = servingTime;
                    }
                }
            }
            else if (myState == CashierState.serving)
            {
                serving -= Time.deltaTime;
                if (serving <= 0)
                {
                    myState = CashierState.checking;
                    CustomersInLine[0].Leave();
                    CustomersInLine.RemoveAt(0);
                    for (int i = 0; i < CustomersInLine.Count; i++)
                    {
                        CustomersInLine[i].SetNewDestinationInLine(GetPositionInLineById(i));
                    }
                }
            }
        }
	}
}
