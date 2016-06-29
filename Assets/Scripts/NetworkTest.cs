namespace Test
{
    using UnityEngine;

    public class NetworkTest : MonoBehaviour
    {
        public void DisconnectPhoton()
        {
            PhotonNetwork.Disconnect();
        }

        public void LockRoom()
        {
            PhotonNetwork.room.open = false;
        }
    }
}

