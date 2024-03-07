using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    unsafe public class ConfigAssetsHelper
    {
        public static GameConfig GetGameConfig(Frame f)
        {
            // AssetGuid defaultGameConfigID = f.GetSingleton<GameSessionManager>().fallbackGameConfig.Id;
            // if (f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig.Id) == null)
            // {
            //     Log.Warn("GameConfig not found, using default");
            //     return f.FindAsset<GameConfig>(defaultGameConfigID);
            // }
            // else
            // {
            //     Log.Warn("GameConfig found");
            return f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig.Id);
            // }
        }
        
        public static MapCustomData GetMapCustomData(Frame f)
        {
            return f.FindAsset<MapCustomData>(f.Map.UserAsset.Id);
        }
        
        public static Graph GetAINavigationGraph(Frame f)
        {
            return f.FindAsset<Graph>(GetMapCustomData(f).AINavigationGraph.Id);
        }
    }
}