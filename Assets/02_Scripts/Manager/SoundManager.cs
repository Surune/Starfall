using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Starfall.Constants;

namespace Starfall.Manager
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] Toggle _soundToggleButton; // 배경음 음소거 토글 버튼
        bool isMuted;
        AudioSource[] audioSources;
        string MutePlayerPrefs => ConstantStore.SoundMute;
        static SoundManager instance;

        void Start()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            isMuted = PlayerPrefs.GetInt(MutePlayerPrefs) == 1;
            ApplyMute();
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        public void ApplyMute()
        {
            FindToggle();
            audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.mute = PlayerPrefs.GetInt("soundMuted") == 1;
            }
        }

        void OnSceneChanged(Scene currentScene, Scene nextScene)
        {
            // Scene이 전환될 때마다 Toggle 버튼을 찾음
            ApplyMute();
        }

        void FindToggle()
        {
            // 현재 Scene에서 BGM Toggle 버튼을 찾음
            var obj = GameObject.Find("SoundToggle");
            if (obj != null)
            {
                _soundToggleButton = obj.GetComponent<Toggle>();
                _soundToggleButton.isOn = PlayerPrefs.GetInt("soundMuted") == 1;
                _soundToggleButton.onValueChanged.AddListener(SetMute);
            }
        }

        // 음소거 설정
        public void SetMute(bool _isMuted)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.mute = _isMuted;
            }
            PlayerPrefs.SetInt("soundMuted", _isMuted ? 1 : 0);
        }
    }
}
