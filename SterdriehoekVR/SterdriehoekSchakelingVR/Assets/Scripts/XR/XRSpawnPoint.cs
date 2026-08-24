using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

namespace SterdriehoekVR.XR
{
    /// <summary>
    /// Zorgt dat de speler (XR Origin) altijd op dezelfde plek en in dezelfde richting
    /// verschijnt bij het starten van de scene, ongeacht waar de speler fysiek staat
    /// in zijn kamer of hoe de headset op dat moment getrackt/gekalibreerd is.
    ///
    /// BELANGRIJK: we corrigeren enkel de horizontale (X/Z) positie en de kijkrichting.
    /// De hoogte (Y) van de camera raken we NIET aan - die wordt bepaald door de echte
    /// lichaamslengte van de speler via Floor-tracking. Zouden we de camera (ogen) op de
    /// Y van het spawnpunt zetten, dan komt het lichaam van de speler daaronder terecht
    /// (= onder de vloer spawnen).
    /// </summary>
    [DisallowMultipleComponent]
    public class XRSpawnPoint : MonoBehaviour
    {
        [Tooltip("De XR Origin rig die verplaatst moet worden.")]
        [SerializeField] private XROrigin xrOrigin;

        [Tooltip("Het punt waar de speler moet spawnen (positie X/Z + kijkrichting). De Y-waarde van dit object wordt genegeerd.")]
        [SerializeField] private Transform spawnPoint;

        [Tooltip("Aantal frames wachten voor het herpositioneren, zodat de eerste tracking-data van de headset zeker binnen is.")]
        [SerializeField] private int framesToWait = 1;

        private void Reset()
        {
            xrOrigin = FindFirstObjectByType<XROrigin>();
            spawnPoint = transform;
        }

        private void Start()
        {
            if (spawnPoint == null)
                spawnPoint = transform;

            StartCoroutine(SnapToSpawnPointNextFrame());
        }

        private IEnumerator SnapToSpawnPointNextFrame()
        {
            for (int i = 0; i < framesToWait; i++)
                yield return null;

            MoveToSpawn();
        }

        /// <summary>
        /// Draait de rig zodat de speler in de juiste richting kijkt, en verschuift de rig
        /// enkel horizontaal (X/Z) zodat de speler op de juiste plek staat. De hoogte van
        /// de camera (bepaald door Floor-tracking + echte lichaamslengte) blijft ongemoeid.
        /// </summary>
        public void MoveToSpawn()
        {
            if (xrOrigin == null || spawnPoint == null)
            {
                Debug.LogWarning("[XRSpawnPoint] XR Origin of spawnPoint ontbreekt.", this);
                return;
            }

            // 1) Kijkrichting uitlijnen (draait rond de camera, raakt hoogte niet aan).
            xrOrigin.MatchOriginUpCameraForward(spawnPoint.up, spawnPoint.forward);

            // 2) Enkel horizontaal verschuiven naar de X/Z van het spawnpunt.
            Vector3 cameraPos = xrOrigin.Camera.transform.position;
            Vector3 horizontalOffset = spawnPoint.position - cameraPos;
            horizontalOffset.y = 0f;
            xrOrigin.transform.position += horizontalOffset;
        }
    }
}
