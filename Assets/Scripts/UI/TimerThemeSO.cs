using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "New Timer Theme", menuName = "Timer Theme", order = 2)]
    public class TimerThemeSO : ScriptableObject
    {
        public List<Sprite> NumberSpritesList;
    }
}