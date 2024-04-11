using System.Diagnostics;
using System.Linq;
using Malee;
using UnityEngine;
using UnityEngine.Serialization;

namespace JellyGarden.Scripts.Targets
{
    [CreateAssetMenu(fileName = "TargetLevel", menuName = "TargetLevel", order = 1)]
    public class TargetLevel : ScriptableObject
    {
        [Reorderable]
        public TargetList targets;
    }
    
    [System.Serializable]
    public class TargetList : ReorderableArray<TargetObject> {
    }

    [System.Serializable]
    public class SpriteList : ReorderableArray<Sprite>
    {
    }

    [System.Serializable]
    public class TargetObject
    {
        public Target type;
        public Sprite icon;
        public int color;
        [FormerlySerializedAs("count")]public int targetCount;
        [HideInInspector] public int countCollected;
        [HideInInspector] public TargetIcon guiObj;
        [Reorderable,Header("If 0 elements, it considers only Block prefab as target")]
        public SpriteList blockSprites;
        public TargetObject DeepCopy()
        {
            TargetObject other = (TargetObject) MemberwiseClone();
            return other;
        }

        public bool Done() => GetCount() <= 0;
        public int GetCount() => targetCount - countCollected;

        public bool SetOff(GameObject item, int c = 1)
        {
            if (type == Target.COLLECT)
            {
                if(item.GetComponent<Item>()?.color == color)
                {
                    countCollected+=c;
                    return true;
                }
            }
            else if (type == Target.INGREDIENT)
            {
                if (item.GetComponent<Item>()?.sprRenderer.sprite.name == icon.name)
                {
                    countCollected+=c;
                    return true;
                }
            }
            else if (type == Target.BLOCKS)
            {
 
                if (item.GetComponent<Square>() != null && blockSprites.Any(i => i.name == item.GetComponent<SpriteRenderer>().sprite.name))
                {
                    countCollected+=c;
                    LevelManager.THIS.TargetBlocks--;

                    return true;
                }
                else if (item.GetComponent<Square>() != null)
                {
                    countCollected+=c;
                    LevelManager.THIS.TargetBlocks--;

                    return true;
                }


            }
            else if (type == Target.SCORE)
            {
                countCollected = LevelManager.Score;
            }
            return false;
        }
    }
}