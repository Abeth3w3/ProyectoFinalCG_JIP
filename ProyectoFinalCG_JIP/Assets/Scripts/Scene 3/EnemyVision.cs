using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform visionPivot;

    [Header("Vision Settings")]
    public float visionRange = 10f;
    [Range(1, 179)] public float visionAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Detection Reaction")]
    public float detectionDelay = 1.5f; // tiempo antes de reiniciar
    public bool resetPlayerOnDetection = true;
    public Transform respawnPoint; // punto donde reaparece el jugador

    [Header("Debug")]
    public bool drawGizmos = true;
    public bool verboseDebug = false;

    private bool playerDetected = false;
    private bool reacting = false;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (visionPivot == null)
            visionPivot = transform;
    }

    void Update()
    {
        if (reacting) return; // evita múltiples detecciones simultáneas

        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 eyePos = visionPivot.position + Vector3.up * 0.2f;
        Vector3 toPlayer = player.position - eyePos;
        float dist = toPlayer.magnitude;

        if (dist > visionRange)
        {
            playerDetected = false;
            return;
        }

        Vector3 dirToPlayer = toPlayer.normalized;
        Vector3 forward = visionPivot.forward;
        forward.y = 0f;
        Vector3 flatDir = new Vector3(dirToPlayer.x, 0f, dirToPlayer.z).normalized;
        float angleToPlayer = Vector3.Angle(forward.normalized, flatDir);

        if (angleToPlayer <= visionAngle / 2f)
        {
            Vector3 targetPos = player.position + Vector3.up * 1.0f;
            Vector3 rayDir = (targetPos - eyePos).normalized;
            float rayDist = Vector3.Distance(eyePos, targetPos);

            if (!Physics.Raycast(eyePos, rayDir, out RaycastHit hit, rayDist, obstacleMask))
            {
                if (!playerDetected)
                {
                    playerDetected = true;
                    Debug.Log("👁 ¡Jugador detectado por " + gameObject.name + "!");
                    StartCoroutine(OnPlayerDetected());
                }
            }
            else
            {
                playerDetected = false;
            }
        }
        else
        {
            playerDetected = false;
        }
    }

    private IEnumerator OnPlayerDetected()
    {
        reacting = true;

        // 1) Parar movimiento del jugador
        PlayerMovement3 pm = player.GetComponent<PlayerMovement3>();
        if (pm != null) pm.enabled = false;

        // 2) Fade a negro
        yield return ScreenFader.Instance.StartCoroutine(ScreenFader.Instance.FadeOut());

        // 3) Mover al jugador al respawn
        if (respawnPoint != null)
            player.position = respawnPoint.position;

        // (Opcional) Reiniciar rotación del enemigo
        GetComponent<NavMeshAgent>().ResetPath();

        // 4) Fade back (volver)
        yield return ScreenFader.Instance.StartCoroutine(ScreenFader.Instance.FadeIn());

        // 5) Reactivar controles del jugador
        if (pm != null) pm.enabled = true;

        reacting = false;
    }


    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || visionPivot == null) return;

        Transform pivot = visionPivot;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot.position, visionRange);

        Vector3 forward = pivot.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(pivot.position, rightBoundary.normalized * visionRange);
        Gizmos.DrawRay(pivot.position, leftBoundary.normalized * visionRange);

        if (player != null)
        {
            Gizmos.color = playerDetected ? Color.green : Color.white;
            Gizmos.DrawLine(pivot.position, player.position + Vector3.up * 1.0f);
        }
    }
}
