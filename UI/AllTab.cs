//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using System.Linq;

//public class AllTab : MonoBehaviour , IPointerUpHandler
//{
//    public BaseItem baseItem;
//    List<BaseItem> baseItemList = new List<BaseItem>(30);
//    private void Awake()
//    {
//        baseItem = GameObject.Find("ItemBase").GetComponent<BaseItem>();
//    }

//    private void Start()
//    {
//        TabSelect("AllTab");
//    }

//    private void TabSelect(string itemType)
//    {
//        var textFocus = transform.Find("IconFocus").gameObject;
//        var tabFocus = transform.Find("TabFocus").gameObject;
//        baseItemList.ForEach(x => Destroy(x.gameObject));
//        baseItemList.Clear();

//        baseItem.gameObject.SetActive(false);
//        if (textFocus.activeSelf)
//        {

//            textFocus.SetActive(false);
//            tabFocus.SetActive(false);
//            Debug.Log("true -> false");
//        }
//        else
//        {
//            if(itemType == "AllTab")
//            {
//                for (int i = 0; i < UserDB.instance.userItems.Count; i++)
//                {
//                    var newItem = Instantiate(baseItem, baseItem.transform.parent);
//                    newItem.Init(UserDB.instance.userItems[i]);
//                    baseItemList.Add(newItem);
//                    baseItemList[i].gameObject.SetActive(true);
//                }
//            }
//            else
//            {
//                var newItemList = (from item in UserDB.instance.userItems
//                                  where item.itemType == itemType
//                                   select item).ToList();
//                for (int i = 0; i < newItemList.Count; i++)
//                {
//                    var newItem = Instantiate(baseItem, baseItem.transform.parent);
//                    newItem.Init(newItemList[i]);
//                    baseItemList.Add(newItem);
//                    baseItemList[i].gameObject.SetActive(true);
//                }

//            }
            
//            textFocus.SetActive(true);
//            tabFocus.SetActive(true);
//            Debug.Log("false -> true");
//        }
//    }

//    public void OnPointerUp(PointerEventData eventData)
//    {
//        Debug.Log("OnPointerClick");
//        if (eventData.button == PointerEventData.InputButton.Left)
//        {
//            switch (eventData.pointerClick.name)
//            {
//                case "AllTab":

//                    TabSelect(eventData.pointerClick.name);

//                    break;
//                case "EquipTab":
//                    TabSelect("Weapon");
//                    break;
//                case "PotionTab":
//                    TabSelect("Potion");
//                    break;
//                case "RingTab":
//                    TabSelect("Ring");
//                    break;
//                default:
//                    break;
//            }

//        }
//    }
//}
