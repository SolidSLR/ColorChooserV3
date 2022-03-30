using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public static List<Color> availableColors;

        public NetworkVariable<Color> actualColor= new NetworkVariable<Color>();

        private Renderer ren;

        public override void OnNetworkSpawn()
        {
             if (IsOwner)
            {
                Move();
                
               
            }
        }

        public void Move()
        {
        
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
            transform.position = Position.Value;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void FillAvailableColors(){

            //availableColors.Add(Color.black);
            availableColors.Add(Color.blue);
            //availableColors.Add(Color.cyan);
            availableColors.Add(Color.red);
            //availableColors.Add(Color.grey);
            //availableColors.Add(Color.white);
            //availableColors.Add(Color.magenta);
            availableColors.Add(Color.green);
            //availableColors.Add(Color.yellow);
        }
        
        public Color GetRandomColor(bool first = false){

            Color oldColor = ren.material.color;

            Color newColor = availableColors[Random.Range(0, availableColors.Count)];

            availableColors.Remove(newColor);

            if(!first) availableColors.Add(oldColor);

            return newColor;
        }

        public void ChangeColor(){

            SubmitColorRequestServerRpc();

        }

        [ServerRpc]

        void SubmitColorRequestServerRpc(bool first = false, ServerRpcParams rpcParams = default){

            Debug.Log(availableColors.Count);

            Color oldColor = actualColor.Value;

            Color newColor = availableColors[Random.Range(0, availableColors.Count)];

            availableColors.Remove(newColor);

            if(!first) {availableColors.Add(oldColor);}

            actualColor.Value = newColor;

            
        }

        void Awake(){

            availableColors = new List<Color>();

            if(availableColors.Count==0){
                FillAvailableColors();
            }
        }

        void Start(){

            ren = GetComponent<Renderer>();

            if(IsOwner) {SubmitColorRequestServerRpc(true);}
        }

        void Update(){

            transform.position = Position.Value;

            ren.material.color = actualColor.Value;
        }
    }
}