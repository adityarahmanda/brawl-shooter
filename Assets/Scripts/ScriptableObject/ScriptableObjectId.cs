using UnityEngine;

namespace BrawlShooter
{
    public class IdAttribute : PropertyAttribute { }

    public class ScriptableObjectId : ScriptableObject
    {
        [Id]
        public string Id;
    }
}