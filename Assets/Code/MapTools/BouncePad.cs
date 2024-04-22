﻿using Code.Players;
using UnityEngine;

namespace Code.MapTools{
    public class BouncePad : MonoBehaviour{
        public float upForce;
        public float directionalForce;

        private void OnTriggerEnter(Collider other){
            if (!other.transform.parent) return;
            if (!other.transform.parent.TryGetComponent(out GamePlayer gamePlayer)) return;
            if (!gamePlayer.isLocalPlayer) return;

            Rigidbody rb = gamePlayer.GetComponent<Rigidbody>();
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            rb.AddForce(velocity.normalized * directionalForce + Vector3.up * upForce, ForceMode.VelocityChange);
        }
    }
}