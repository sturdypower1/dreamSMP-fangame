using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ItemInventoryAuthoring : MonoBehaviour
{
    public ItemInfo[] itemInfos;
}
public struct ItemData : IBufferElementData{
    public Item item;
}
public class ItemConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ItemInventoryAuthoring itemInventory) => {
            DstEntityManager.AddBuffer<ItemData>(entity);
            DynamicBuffer<ItemData> ItemInventory = DstEntityManager.GetBuffer<ItemData>(entity);

            foreach(ItemInfo itemInfo in itemInventory.itemInfos){
                Item item = new Item{
                    itemType = itemInfo.itemType,
                    name = itemInfo.name,
                    description = itemInfo.description,
                    useTime = itemInfo.useTime
                };
                ItemInventory.Add(new ItemData{item = item});
            }
        });
    }
}
public enum ItemType{
    none,
    healing,
    statsboost,
    damage

}
[System.Serializable]
public struct ItemInfo{
    public ItemType itemType;
    public string name;
    public string description;
    public float useTime;
}
[System.Serializable]
public struct Item{
    public ItemType itemType;
    public FixedString32 name;
    public FixedString128 description;
    public float useTime;
}