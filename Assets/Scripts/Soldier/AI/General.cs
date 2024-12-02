using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
public class General : MonoBehaviour
{
    public Transform[] squadLeaders;
    public Transform retreatPoint;
    // References to other components
    private Health healthComponent;
    private SoldierAttributes attributes;
    public enum FormationType { ZigZag, VShape, Square }
    public FormationType currentFormation = FormationType.Square;
    public Transform generalTransform;
    public float formationSpacing = 15f;
    public float followDistance = 5f;
    private FSM leaderFSM;
    private FSM.State[] previousStates;
    private FSM.State currentState;
    private GameObject leaderObject;
    private Vector3 offset;
    public Vector3[] squareFormation = new Vector3[]
    {
        new Vector3(0, 0, 5),    // Middle squad 
        new Vector3(0, 0, 20),    // Squad in front
        new Vector3(0, 0, -10),   // Squad behind
        new Vector3(15, 0, 5),    // Squad to the right
        new Vector3(-15, 0, 5)    // Squad to the left
    };
    public Vector3[] vFormation = new Vector3[]
    {
        new Vector3(0, 0, 5),    // Middle squad 
        new Vector3(-20, 0, -5),    // further behind and to the left
        new Vector3(-10, 0, -15),   // Second left Squad further behind
        new Vector3(10, 0, -15),    // Second Right Squad further behind
        new Vector3(20, 0, -5)    // Right Squad behind
    };
    public Vector3[] zigZagFormation = new Vector3[]
    {
        new Vector3(0, 0, 5),    // Middle squad 
        new Vector3(-5, 0, -5),    // Squad to the left and back
        new Vector3(5, 0, -5),   //Squad to the right and forward
        new Vector3(-10, 0, -5),    //Squad to the left and back
        new Vector3(10, 0, 5)    //Squad to the right and forward
    };
    void Start()
    {
        squadLeaders = GetGeneralChildren();
        UpdateSquadFormation(FSM.State.Idle);
    }
    void Update()
    {
        for (int i = 0; i < squadLeaders.Length; i++)
        {
            
                FSM leader = squadLeaders[i].GetComponent<FSM>();
            
                FSM.State currentState = leader.GetCurrentState();
                // Check if the state has changed
                if (previousStates[i] != currentState)
                {
                    // Update the stored state
                    previousStates[i] = currentState;
                    // Take actions based on the new state
                    UpdateSquadFormation(currentState);
                }
        }
    }
        // This method updates the formation of squads
        public void UpdateSquadFormation(FSM.State newState)
        {
            switch (newState)
            {
                case FSM.State.Idle:
                    setSquareFormation();
                    break;
                case FSM.State.Attack:
                    setZigZagFormation();
                    break;
                case FSM.State.Wandering:
                    setSquareFormation();
                    break;
                case FSM.State.Chase:
                    setVFormation();
                    break;
                case FSM.State.Retreat:
                    setSquareFormation();
                    break;
                default:
                    break;
            }
        }
        void setSquareFormation()
        {
            for (int i = 0; i < squadLeaders.Length; i++)
            {
                SquadLeader squadLeader = squadLeaders[i].GetComponent<SquadLeader>();
                squadLeader.setPosition(squareFormation[i], generalTransform);
            }
        }
        void setVFormation()
        {
            for (int i = 0; i < squadLeaders.Length; i++)
            {
                SquadLeader squadLeader = squadLeaders[i].GetComponent<SquadLeader>();
                squadLeader.setPosition(vFormation[i], generalTransform);
            }
        }
        void setZigZagFormation()
        {
            for (int i = 0; i < squadLeaders.Length; i++)
            {
                SquadLeader squadLeader = squadLeaders[i].GetComponent<SquadLeader>();
                squadLeader.setPosition(zigZagFormation[i], generalTransform);
            }
        }
        public FSM.State[] getCurrentStates()
        {
            List<FSM.State> states = new List<FSM.State>();
            for (int i = 0; i < squadLeaders.Length; i++)
            {
                leaderFSM = squadLeaders[i].GetComponent<FSM>();
                states.Add(leaderFSM.GetCurrentState());
            }
            return states.ToArray();
        }
        public Transform[] GetGeneralChildren()
        {
            List<Transform> leaders = new List<Transform>();
            for (int i = 0; i < generalTransform.childCount; i++)
            {
                leaders.Add(generalTransform.GetChild(i));
            }
            return leaders.ToArray();
        }
    } 