using AdventureGame.Core.Quests;
using AdventureGame.Core.Scene;
using AdventureGame.Entities.NPC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AdventureGame.Core.Managers
{
    public class QuestTrackerManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument m_TrackerUiDocument;

        private PlayerInputActions m_InputActions;
        private VisualElement m_TrackerRoot;
        private Label m_DescriptionLabel;
        private Label m_ProgressLabel;

        private bool m_IsOpen;
        private QuestData m_CompletionPending;

        private int m_LastCount = -1;
        private Quest m_LastTracked;

        private const string k_CompletionMessage = "Quest complete! Return and speak with the NPC.";

        void Awake() => m_InputActions = InputManager.Instance.Actions;

        void OnEnable() => m_InputActions.Player.Tracker.performed += OnTrackerPressed;

        void OnDisable() => m_InputActions.Player.Tracker.performed -= OnTrackerPressed;

        void Start()
        {
            VisualElement root = m_TrackerUiDocument.rootVisualElement;
            m_TrackerRoot = root.Q<VisualElement>("TrackerRoot");
            m_DescriptionLabel = root.Q<Label>("Description");
            m_ProgressLabel = root.Q<Label>("Progress");

            if (QuestManager.Instance != null)
                foreach (Quest q in QuestManager.Instance.All)
                    q.OnPhaseChanged += HandlePhaseChanged;

            NPC.OnEpilogueStarted += HandleEpilogueStarted;

            SetVisible(false);
        }

        void OnDestroy()
        {
            if (QuestManager.Instance != null)
                foreach (Quest q in QuestManager.Instance.All)
                    q.OnPhaseChanged -= HandlePhaseChanged;

            NPC.OnEpilogueStarted -= HandleEpilogueStarted;
        }

        void HandlePhaseChanged(Quest q)
        {
            if (q.Phase == QuestPhase.After)
            {
                m_CompletionPending = q.Data;

                Refresh();
            }
        }

        void HandleEpilogueStarted(QuestData data)
        {
            if (data != m_CompletionPending)
                return;

            m_CompletionPending = null;
            m_IsOpen = false;

            Refresh();
        }

        void Update()
        {
            if (
                PauseManager.IsPaused
                || (
                    SceneTransitioner.Instance != null && SceneTransitioner.Instance.IsTransitioning
                )
            )
            {
                SetVisible(false);

                return;
            }

            if (m_IsOpen || m_CompletionPending != null)
                Refresh();
        }

        void OnTrackerPressed(InputAction.CallbackContext ctx)
        {
            if (PauseManager.IsPaused)
                return;

            if (m_CompletionPending != null)
                return;

            m_IsOpen = !m_IsOpen;

            Refresh();
        }

        void Refresh()
        {
            if (m_CompletionPending != null)
            {
                if (m_DescriptionLabel != null)
                    m_DescriptionLabel.text = k_CompletionMessage;

                if (m_ProgressLabel != null)
                    m_ProgressLabel.text = string.Empty;

                SetVisible(true);

                return;
            }

            Quest tracked = FindActiveCounted();
            if (!m_IsOpen || tracked == null)
            {
                SetVisible(false);

                return;
            }

            if (m_DescriptionLabel != null && tracked != m_LastTracked)
            {
                m_DescriptionLabel.text = tracked.Data.Description;
                m_LastTracked = tracked;
            }

            if (m_ProgressLabel != null && tracked.Count != m_LastCount)
            {
                m_ProgressLabel.text = string.Concat(
                    tracked.Count.ToString(),
                    " / ",
                    tracked.Target.ToString()
                );
                m_LastCount = tracked.Count;
            }

            SetVisible(true);
        }

        Quest FindActiveCounted()
        {
            if (QuestManager.Instance == null)
                return null;

            foreach (Quest q in QuestManager.Instance.All)
                if (
                    q.Phase == QuestPhase.During
                    && q.Data != null
                    && q.Data.CompletionMode == QuestCompletionMode.Counted
                )
                    return q;

            return null;
        }

        void SetVisible(bool visible)
        {
            if (m_TrackerRoot != null)
                m_TrackerRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
