using MultiSide.shared;
using Photon.Client;
using UnityEngine;

namespace mszcubemod
{
    class CubeNetworking
    {
        public static CubeNetworking? Instance { get; private set; }

        const string ChannelPlace = "zerocraft.place";
        const string ChannelDelete = "zerocraft.delete";
        const string ChannelWorldStateRequest = "zerocraft.worldstate.request";
        const string ChannelWorldStateResponse = "zerocraft.worldstate.response";

        CubeNetworking() { }

        public static void Init()
        {
            bool available = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name == "Multiside.shared");
            if (!available) return;
            Instance = new CubeNetworking();
            Instance.InitNetwork();
        }

        void InitNetwork()
        {
            if (NetworkRegistry.Provider != null)
            {
                Subscribe(NetworkRegistry.Provider);
                return;
            }
            NetworkRegistry.OnProviderRegistered += Subscribe;
        }

        void Subscribe(INetworkProvider provider)
        {
            provider.OnReceived += OnReceived;
            provider.OnRoomJoined += OnRoomJoined;
        }

        void OnReceived(int actor, string channel, object data)
        {
            switch (channel)
            {
                case ChannelPlace when data is PhotonHashtable ht:
                    HandlePlace(ht);
                    break;
                case ChannelDelete when data is PhotonHashtable ht:
                    HandleDelete(ht);
                    break;
                case ChannelWorldStateRequest:
                    HandleWorldStateRequest(actor);
                    break;
                case ChannelWorldStateResponse when data is PhotonHashtable[] blocks:
                    HandleWorldStateResponse(blocks);
                    break;
            }
        }

        void OnRoomJoined()
        {
            NetworkRegistry.Provider?.Send(ChannelWorldStateRequest, new PhotonHashtable());
        }

        void HandlePlace(PhotonHashtable ht)
        {
            if (ht["blockId"] is not string blockId) return;
            if (ht["position"] is not Vector3 position) return;
            WorldManager.Instance.PlaceBlock(blockId, position);
        }

        void HandleDelete(PhotonHashtable ht)
        {
            if (ht["position"] is not Vector3 position) return;
            WorldManager.Instance.DeleteBlock(position);
        }

        void HandleWorldStateRequest(int requesterActor)
        {
            if (NetworkRegistry.Provider is not { } provider) return;
            if (!provider.IsMasterClient) return;

            PhotonHashtable[] blocks = WorldManager.Instance.PlacedBlocks.Values
                .Select(b => new PhotonHashtable
                {
                    ["blockId"] = b.BlockId,
                    ["position"] = b.Position
                })
                .ToArray();

            provider.SendTo(requesterActor, ChannelWorldStateResponse, blocks);
        }

        void HandleWorldStateResponse(PhotonHashtable[] blocks)
        {
            foreach (PhotonHashtable ht in blocks)
            {
                if (ht["blockId"] is not string blockId) continue;
                if (ht["position"] is not Vector3 position) continue;
                WorldManager.Instance.PlaceBlock(blockId, position);
            }
        }

        public void SendPlace(string blockId, Vector3 position)
        {
            NetworkRegistry.Provider?.Send(ChannelPlace, new PhotonHashtable
            {
                ["blockId"] = blockId,
                ["position"] = position
            });
        }

        public void SendDelete(Vector3 position)
        {
            NetworkRegistry.Provider?.Send(ChannelDelete, new PhotonHashtable
            {
                ["position"] = position
            });
        }
    }
}