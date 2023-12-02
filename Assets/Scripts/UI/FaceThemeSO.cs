using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "New Face Theme", menuName = "Face Theme", order = 1)]
    public class FaceThemeSO : ScriptableObject
    {
        public Sprite Smiles;
        public Sprite Wow;
        public Sprite Cool;
        public Sprite Sad;
    }
}