using DefaultNamespace;
using events;
using events.game;
using game;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] 
    private PlayerConfig config;

    private StateContext context;
    private CSStates.AbstractState currentState;
    private bool isHitWinCollider;
    
    private void Start()
    {
        context = new StateContext()
        {
            animator = GetComponent<Animator>(),
            controller = GetComponent<CharacterController>(),
            cachedTransform = transform,
            config = config,
            gravity = Physics.gravity
        };

        CSStates.statesRegistry.Clear();
        CSStates.statesRegistry.Add(typeof(CSStates.MovingState), new CSStates.MovingState(context));
        CSStates.statesRegistry.Add(typeof(CSStates.JumpingState), new CSStates.JumpingState(context));
        CSStates.statesRegistry.Add(typeof(CSStates.FallingState), new CSStates.FallingState(context));
        CSStates.statesRegistry.Add(typeof(CSStates.RestoringState), new CSStates.RestoringState(context));

        currentState = CSStates.statesRegistry[typeof(CSStates.MovingState)];
        currentState.EnterState();
    }

    public void OnEndOfFallRolling()
    {
        context.isRestoreCompleted = true;
    }

    public void OnEndOfHardLanding()
    {
        context.isRestoreCompleted = true;
    }

    private void Update()
    {
        var nextState = currentState.GetNextState();
        if (nextState != null)
        {
            currentState = nextState;
            currentState.EnterState();
            Debug.Log($"Switching state to {currentState.GetType().Name}");
        }
        currentState.HandleUpdate();
    }

    private void FixedUpdate()
    {
        currentState.HandleFixedUpdate();
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isHitWinCollider && hit.gameObject.CompareTag("WinCollider"))
        {
            isHitWinCollider = true;
            EventBus.Publish(new PlayerWinEvent());
        }

        if (hit.gameObject.CompareTag("ScoreCollider"))
        {
            hit.gameObject.tag = "Untagged";
            EventBus.Publish(new HitScorePointEvent());
        }
    }
}
