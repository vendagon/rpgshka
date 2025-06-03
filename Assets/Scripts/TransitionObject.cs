using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionObject: MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [SerializeField] private InteractionHint interactionHint;
    [SerializeField] private string targetSceneName = "InsideHouse";
    [SerializeField] private Vector3 playerSpawnPosition; // Позиция появления внутри дома


    private bool _playerInRange = false;
    void Start()
    {
        interactionHint.HideHint();
    }
    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Enter();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            if (interactionHint != null)
                interactionHint.ShowHint();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            if (interactionHint != null)
                interactionHint.HideHint();
        }
    }

    private void Enter()
    {
        SceneDataTransfer.PlayerSpawnPosition = playerSpawnPosition;
        SceneDataTransfer.PrepareTransition();
        SceneTransitionManager.Instance.TransitionToScene(targetSceneName);
    }
}