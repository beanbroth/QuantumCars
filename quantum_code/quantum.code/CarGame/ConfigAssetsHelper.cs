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
            return f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig.Id);
        }
    }
}