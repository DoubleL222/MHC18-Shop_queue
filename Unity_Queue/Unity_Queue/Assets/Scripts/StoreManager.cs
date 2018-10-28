using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreManager : MonoBehaviour {

    public enum StoreManagement
    {
        Reactive,
        Adaptive
    }

    public GameObject PeopleSpawner;
    public GameObject Exit;
    public GameObject ShopperPrefab;

    private static StoreManager _instance;
    public static StoreManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<StoreManager>();
            }
            return _instance;
        }
    }

    private int LineBreakingPoint = 5;

    private StoreManagement myManagamentMode = StoreManagement.Reactive;

    private List<GameObject> shoppingPositions;
    private List<Cashier> allCashiers;

	// Use this for initialization
	void Start () {
        _instance = this;
        allCashiers = FindObjectsOfType<Cashier>().ToList();
        allCashiers[0].IsManed = true;
        shoppingPositions = GameObject.FindGameObjectsWithTag("ShoppingLocation").ToList();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newCustomer = Instantiate(ShopperPrefab, PeopleSpawner.transform.position, Quaternion.identity) as GameObject;
            newCustomer.GetComponent<CustomerController>().SetShoppingTime(Random.Range(5, 30));
        }

        foreach (Cashier c in allCashiers)
        {
            if (myManagamentMode == StoreManagement.Reactive)
            {
                if (c.AllReached())
                {
                    if (c.GetLine() > LineBreakingPoint)
                    {
                        Cashier closed = null;
                        bool freeLine = false;
                        foreach (Cashier closedCachiers in allCashiers)
                        {
                            if(closedCachiers != c && closedCachiers.IsManed && closedCachiers.GetLine() < 5)
                            {
                                closed = closedCachiers;
                                break;
                            }

                            if (closedCachiers != c && !closedCachiers.IsManed)
                            {
                                closed = closedCachiers;
                            }
                        }
                        if (closed != null)
                        {
                            closed.IsManed = true;
                            c.BreakLine(closed, LineBreakingPoint);
                        }
                    }
                }
            }
        }
    }


    public Vector3 GetRandomShoppingPosition()
    {
        return shoppingPositions[Random.Range(0, shoppingPositions.Count - 1)].transform.position;
    }

    public Cashier GetBestLinePosition()
    {
        int minLine = int.MaxValue;
        Cashier minC = null;
        foreach (Cashier c in allCashiers)
        {
            if (c.IsManed)
            {
                if (c.GetLine() < minLine)
                {
                    minLine = c.GetLine();
                    minC = c;
                }
            }
        }
        return minC;
    }

    public Vector3 GetExit()
    {
        return Exit.transform.position;
    }
}
