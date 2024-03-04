using Quantum; // Ensure you have the necessary using directives

namespace Quantum.Game
{
    unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame frame, PlayerRef player)
        {
            var data = frame.GetPlayerData(player);

            // Resolve the reference to the avatar prototype.
            var prototype = frame.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

            // Use the SpawnSystem to create a new entity for the player based on the prototype.
            var entity = SpawnSystem.SpawnFromPrototype(frame, prototype);

            // Create a PlayerLink component. Initialize it with the player. Add the component to the player entity.
            var playerLink = new PlayerLink()
            {
                Player = player,
            };
            frame.Add(entity, playerLink);

            // The position is now set within the SpawnSystem, so this code is not needed anymore.
            // If you still need to apply additional position offset based on player ID, you can do it here.
        }
    }
}