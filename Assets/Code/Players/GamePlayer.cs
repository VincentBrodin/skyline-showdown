﻿using Code.Interface;
using Code.Managers;
using Code.Networking;
using Code.Tools;
using Code.Viewers;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Code.Players{
    public class GamePlayer : NetworkBehaviour{
        [SyncVar] public int connectionId;
        [SyncVar] public int playerId;
        [SyncVar] public bool isHost;
        [SyncVar] public string playerName;
        [SyncVar] public bool frozen;
        [SyncVar] public bool stun;
        [SyncVar] public bool gameActive;
        [SyncVar] public int score;
        public TextMeshPro nameTag;
        public float stunTime;
        public bool firstTimeInLobby = true;
        public Outline outline;


        [SyncVar(hook = nameof(GameModeChanged))]
        public GameMode gameMode = GameMode.None;

        [Space] [SerializeField] private int personalLayer;

        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;

        public GameSettingsManager.OverloadMetaData metaData;
        [SyncVar] public string metaDataString;

        private Rigidbody _rb;
        private Transform _transform;
        private float _nameTagUpdate;
        private Movement _movement;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            _rb = GetComponent<Rigidbody>();
            _transform = transform;
            _movement = GetComponent<Movement>();
            outline.enabled = false;

            if (isLocalPlayer){
                //Update player values
                playerName = PlayerPrefs.GetString("Name");
                Manager().localPlayer = this;

                //Set random spawn point
                _rb.position = new Vector3(Random.Range(-1.25f, 1.25f), 0, Random.Range(-1.25f, 1.25f));

                //Reset cursor
                CursorManager.Singleton.ResetHide();

                gameObject.SetLayer(personalLayer, true);

                foreach (SkinnedMeshRenderer meshRenderer in meshRenderers){
                    meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }

                nameTag.gameObject.SetActive(false);

                isHost = isServer;
                if (PlayerPrefs.HasKey("CustomGame"))
                    metaDataString = PlayerPrefs.GetString("CustomGame");
            }

            Manager().AddPlayer(this);

            DontDestroyOnLoad(gameObject);

            if (isHost && metaDataString != ""){
                GameSettingsManager.Singleton.SetCustomMeta(metaDataString);
            }
        }

        private void FixedUpdate(){
            if (_nameTagUpdate > Time.time) return;
            _nameTagUpdate = Time.time + 1f;
            nameTag.text = playerName;
        }

        private void OnDestroy(){
            Kick();
            (NetworkManager.singleton as CustomNetworkManager)?.RemovePlayer(this);
        }

        public override void OnStopClient(){
            Kick();
            Manager().RemovePlayer(this);
            base.OnStopClient();
        }


        public void Kick(){
            if (isLocalPlayer){
                if (ViewerManager.Singleton)
                    DestroyImmediate(ViewerManager.Singleton.gameObject);
            }

            if (GetComponent<CameraController>() && GetComponent<CameraController>().cameraHolder)
                DestroyImmediate(GetComponent<CameraController>().cameraHolder.gameObject);
        }

        public void Teleport(Vector3 teleportTo){
            ServerTeleport(teleportTo);
        }

        [Command(requiresAuthority = false)]
        private void ServerTeleport(Vector3 teleportTo){
            ClientTeleport(teleportTo);
        }

        [ClientRpc]
        private void ClientTeleport(Vector3 teleportTo){
            Debug.Log($"Teleporting {playerName} from {_rb.position} to {teleportTo}");
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.position = teleportTo;
            _transform.position = teleportTo;
        }

        public void Stun(){
            ServerStun();
        }

        [Command(requiresAuthority = false)]
        private void ServerStun(){
            stun = true;
            ClientStun();
        }

        [ClientRpc]
        private void ClientStun(){
            if (!isLocalPlayer) return;
            stun = true;
            CancelInvoke(nameof(UnStun));
            Invoke(nameof(UnStun), stunTime);
        }

        private void UnStun(){
            stun = false;
        }


        public void Freeze(){
            ServerFreeze();
        }

        [Command(requiresAuthority = false)]
        private void ServerFreeze(){
            frozen = true;
            ClientFreeze();
        }

        [ClientRpc]
        private void ClientFreeze(){
            if (!isLocalPlayer) return;
            frozen = true;
        }

        public void UnFreeze(){
            ServerUnFreeze();
        }

        [Command(requiresAuthority = false)]
        private void ServerUnFreeze(){
            frozen = false;
            ClientUnFreeze();
        }

        [ClientRpc]
        private void ClientUnFreeze(){
            if (!isLocalPlayer) return;
            frozen = false;
        }

        private void GameModeChanged(GameMode oldValue, GameMode newValue){
            if (!isLocalPlayer) return;
            GameModeUi.Singleton.GameModeChanged(newValue);
        }

        public void GiveScore(int scoreToGive, string prompt){
            ServerGiveScore(scoreToGive, prompt);
        }

        [Command(requiresAuthority = false)]
        private void ServerGiveScore(int scoreToGive, string prompt){
            ClientGiveScore(scoreToGive, prompt);
        }

        [ClientRpc]
        private void ClientGiveScore(int scoreToGive, string prompt){
            if (!isLocalPlayer) return;
            float newScore = scoreToGive * Manager().localPlayer.metaData.score;
            scoreToGive = (int)newScore;
            if (score + scoreToGive < 0){
                score = 0;
            }
            else
                score += scoreToGive;

            ScoreUi.Singleton.UpdateScore(scoreToGive, prompt);
        }

        public void SetScore(int scoreToSet){
            ServerSetScore(scoreToSet);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetScore(int scoreToSet){
            ClientSetScore(scoreToSet);
        }

        [ClientRpc]
        private void ClientSetScore(int scoreToSet){
            if (!isLocalPlayer) return;
            score = scoreToSet;
        }

        public Vector3 Position(){
            return _transform.position;
        }

        public void SetNameTagVisibility(bool newValue){
            ServerSetNameTagVisibility(newValue);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetNameTagVisibility(bool newValue){
            ClientSetNameTagVisibility(newValue);
        }

        [ClientRpc]
        private void ClientSetNameTagVisibility(bool newValue){
            if (isLocalPlayer) return;
            nameTag.gameObject.SetActive(newValue);
        }

        public void SetGameActive(bool newValue){
            ServerSetGameActive(newValue);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetGameActive(bool newValue){
            gameActive = newValue;
            ClientSetGameActive(newValue);
        }

        [ClientRpc]
        private void ClientSetGameActive(bool newValue){
            if (!isLocalPlayer) return;
            gameActive = newValue;
        }

        public void SetOutlineVisibility(bool newValue){
            ServerSetOutlineVisibility(newValue);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetOutlineVisibility(bool newValue){
            ClientSetOutlineVisibility(newValue);
        }

        [ClientRpc]
        private void ClientSetOutlineVisibility(bool newValue){
            if (isLocalPlayer) return;
            outline.enabled = false;
        }


        public void AddForce(Vector3 force, ForceMode forceMode){
            ServerAddForce(force, forceMode);
        }

        [Command(requiresAuthority = false)]
        private void ServerAddForce(Vector3 force, ForceMode forceMode){
            ClientAddForce(force, forceMode);
        }

        [ClientRpc]
        private void ClientAddForce(Vector3 force, ForceMode forceMode){
            if (!isLocalPlayer) return;
            _rb.AddForce(force, forceMode);
        }

        public void ResetFall(){
            ServerResetFall();
        }

        [Command(requiresAuthority = false)]
        private void ServerResetFall(){
            ClientResetFall();
        }

        [ClientRpc]
        private void ClientResetFall(){
            Vector3 velocity = _rb.velocity;
            velocity.y = 0;
            _rb.velocity = velocity;
        }

        public void SyncMetaData(float gameTime, float scoreMult, float knockBackMult, float gravityMult,
            float speedMult){
            metaData.gameTime = (int)gameTime;
            metaData.score = scoreMult;
            metaData.knockBack = knockBackMult;
            metaData.gravity = gravityMult;
            metaData.speed = speedMult;

            string jsonMetaData = JsonUtility.ToJson(metaData);
            ServerSyncMetaData(jsonMetaData);
        }

        [Command(requiresAuthority = false)]
        private void ServerSyncMetaData(string jsonMetaData){
            foreach (GamePlayer player in Manager().Players){
                player.ClientSetMetaData(jsonMetaData);
            }
        }

        [ClientRpc]
        private void ClientSetMetaData(string jsonMetaData){
            GameSettingsManager.Singleton.SetCustomMeta(jsonMetaData);
            if (isHost){
                PlayerPrefs.SetString("CustomGame", jsonMetaData);
                metaDataString = jsonMetaData;
            }
        }
    }
}