using Mirror;

namespace Code.Viewers{
    public class ViewerManager : NetworkBehaviour{
        public static ViewerManager Singleton{ get; private set; }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
            
            DontDestroyOnLoad(gameObject);
        }
    }
}