using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Starfall.Constants;

namespace Starfall.Manager {
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance {
            get { return instance; }
        }
        
        [SerializeField] private Toggle soundToggleButton; // 배경음 음소거 토글 버튼
        private bool isMuted;
        private AudioSource[] audioSources;
        private string mutePlayerPrefs => ConstantStore.soundMute;

        private static SoundManager instance;

        void Start() 
        {
            if (instance != null && instance != this) {
                Destroy(gameObject);
                return;
            }

            instance = this;

            isMuted = PlayerPrefs.GetInt(mutePlayerPrefs) == 1;
            ApplyMute();
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDestroy() 
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        public void ApplyMute()
        {
            FindToggle();
            audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources) {
                audioSource.mute = PlayerPrefs.GetInt("soundMuted") == 1;
            }
        }

        private void OnSceneChanged(Scene currentScene, Scene nextScene) 
        {
            // Scene이 전환될 때마다 Toggle 버튼을 찾음
            ApplyMute();
        }

        private void FindToggle() {
            // 현재 Scene에서 BGM Toggle 버튼을 찾음
            var obj = GameObject.Find("SoundToggle");
            if (obj != null) {
                soundToggleButton = obj.GetComponent<Toggle>();
                soundToggleButton.isOn = PlayerPrefs.GetInt("soundMuted") == 1;
                soundToggleButton.onValueChanged.AddListener(SetMute);
            }
        }

        // 음소거 설정
        public void SetMute(bool _isMuted) {
            foreach (AudioSource audioSource in audioSources) {
                audioSource.mute = _isMuted;
            }
            PlayerPrefs.SetInt("soundMuted", _isMuted ? 1 : 0);
        }
    }
}