using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interactions{
    public class Selections : NetworkBehaviour{
        [SyncVar(hook = nameof(UpdateMap))] public int currentSelected;
        [Space] public float animationSpeed = 5f;
        public List<Option> options = new();
        [Header("References")] [Space] public TextMeshProUGUI selectedName;
        [Space] public Interactive nextInteractive;
        public Interactive lastInteractive;
        [Space] public Image imagePrefab;
        public Transform screenParent;
        
        private int _last;
        private int _next;
        private int _current;

        private bool _ready;

        [Serializable]
        public class Option{
            public string optionName;
            public string id;
            public Sprite icon;
            [TextArea] public string metaData;
            [HideInInspector] public Image image;
            [HideInInspector] public Transform transform;
        }

        private void Start(){
            nextInteractive.OnInteraction.AddListener(Next);
            lastInteractive.OnInteraction.AddListener(Last);

            foreach (Option option in options){
                option.image = Instantiate(imagePrefab, screenParent);
                option.image.sprite = option.icon;
                option.transform = option.image.transform;
            }

            UpdateMap(currentSelected, currentSelected);


            options[_current].transform.localPosition = Vector3.zero;

            options[_current].transform.localScale = Vector3.one;

            options[_last].transform.localPosition = new Vector3(-1.5f, 0, 0);

            options[_last].transform.localScale = new Vector3(.75f, .75f, 1);

            options[_next].transform.localPosition = new Vector3(1.5f, 0, 0);

            options[_next].transform.localScale = new Vector3(.75f, .75f, 1);

            for (int i = 0; i < options.Count; i++){
                if (i == _current || i == _last || i == _next) continue;

                options[i].transform.localPosition = Vector3.zero;

                options[i].transform.localScale = new Vector3(.75f, .75f, 1);
            }

            _ready = true;
        }

        private void Next(){
            CmdNext();
        }

        [Command(requiresAuthority = false)]
        private void CmdNext(){
            int map = currentSelected + 1;
            if (map >= options.Count) map = 0;
            currentSelected = map;
        }


        private void Last(){
            CmdLast();
        }

        [Command(requiresAuthority = false)]
        private void CmdLast(){
            int map = currentSelected - 1;
            if (map < 0) map = options.Count - 1;
            currentSelected = map;
        }

        private void UpdateMap(int oldValue, int newValue){
            int last = newValue - 1;
            if (last < 0) last = options.Count - 1;
            int next = newValue + 1;
            if (next >= options.Count) next = 0;
            _next = next;
            _last = last;
            _current = newValue;

            selectedName.text = options[_current].optionName;
            options[_current].transform.SetSiblingIndex(10);
        }

        private void Update(){
            if(!_ready) return;
            float time = animationSpeed * Time.deltaTime;

            options[_current].transform.localPosition = Vector3.Lerp(options[_current].transform.localPosition,
                Vector3.zero, time);

            options[_current].transform.localScale = Vector3.Lerp(options[_current].transform.localScale,
                Vector3.one, time);

            options[_last].transform.localPosition = Vector3.Lerp(options[_last].transform.localPosition,
                new Vector3(-1.5f, 0, 0), time);

            options[_last].transform.localScale = Vector3.Lerp(options[_last].transform.localScale,
                new Vector3(.75f, .75f, 1), time);

            options[_next].transform.localPosition = Vector3.Lerp(options[_next].transform.localPosition,
                new Vector3(1.5f, 0, 0), time);

            options[_next].transform.localScale = Vector3.Lerp(options[_next].transform.localScale,
                new Vector3(.75f, .75f, 1), time);

            for (int i = 0; i < options.Count; i++){
                if (i == _current || i == _last || i == _next) continue;

                options[i].transform.localPosition = Vector3.Lerp(options[i].transform.localPosition,
                    Vector3.zero, time);

                options[i].transform.localScale = Vector3.Lerp(options[i].transform.localScale,
                    new Vector3(.75f, .75f, 1), time);
            }
        }
    }
}