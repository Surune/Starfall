using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Networking
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class NetworkCoopPlayer : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SyncVar(hook = nameof(OnDisplayNameChanged))]
        private string displayName;

        public string DisplayName => displayName;

        private Rigidbody2D rigidBody;
        private Vector2 requestedDirection;

        public override void OnStartLocalPlayer()
        {
            CmdSetDisplayName(NetworkSessionManager.Instance.LocalPlayerName);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
        }

        private void Update()
        {
            if (!isOwned)
            {
                return;
            }

            var keyboard = Keyboard.current;
            var direction = new Vector2(
                (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
            CmdSetMove(direction.normalized);
        }

        [Command]
        private void CmdSetMove(Vector2 direction)
        {
            requestedDirection = direction;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            rigidBody.MovePosition(rigidBody.position + requestedDirection * (moveSpeed * Time.fixedDeltaTime));
        }

        [Command]
        private void CmdSetDisplayName(string value)
        {
            displayName = value;
            RpcSetDisplayName(value);
        }

        [ClientRpc]
        private void RpcSetDisplayName(string value)
        {
            ApplyDisplayName(value);
        }

        private void OnDisplayNameChanged(string _, string value)
        {
            ApplyDisplayName(value);
        }

        private void ApplyDisplayName(string value)
        {
            displayName = value;
            gameObject.name = value;
            spriteRenderer.color = isOwned ? new Color(0.35f, 0.9f, 1f) : new Color(1f, 0.55f, 0.85f);
        }

        private void OnGUI()
        {
            var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(screenPosition.x - 60f, Screen.height - screenPosition.y - 42f, 120f, 24f), displayName);
        }
    }
}
