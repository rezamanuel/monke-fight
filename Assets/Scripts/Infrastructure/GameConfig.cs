using UnityEngine;

namespace Monke.Infrastructure
{
    [CreateAssetMenu(menuName = "Monke/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public bool skipCardSelect;
    }
}