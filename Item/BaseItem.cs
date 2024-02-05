using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(Image))]
public class BaseItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image frame;
    public Image itemIcon;
    public Image nofify;
    public TextMeshProUGUI itemCount;

	public struct SelectedItem
    {
		static public BaseItem selectItemInfo;
		static public UserItem ItemInfo;
	}
	// 인벤에 있는 하나의 아이템 정보
	public UserItem baseItemInfo;

    // Start is called before the first frame update

    internal void Init(MySQL.ItemDB itemInfo, int userItemCount)
    {
        itemCount = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        frame = transform.Find("GradeFrame").GetComponent<Image>();
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        nofify = transform.Find("Nofify").GetComponent<Image>();

		baseItemInfo.userItemInfo = itemInfo; // 하나하나 아이템 정보가 들어가짐
		baseItemInfo.userItemCount = userItemCount;

		// 포션이 아니면 카운트 안보임
        if (itemInfo.itemType != MySQL.ItemType.Potion)
        {
			itemCount.gameObject.SetActive(false);
        }

		itemCount.text = userItemCount.ToString();

		itemIcon.sprite = Resources.Load<Sprite>(itemInfo.iconName.ToString());
    }

	public bool dragOnSurfaces = true;

	private GameObject m_DraggingIcon;
	private RectTransform m_DraggingPlane;

	public void OnBeginDrag(PointerEventData eventData)
	{
		var canvas = FindInParents<Canvas>(gameObject);

		if (canvas == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcon = new GameObject("icon");

		m_DraggingIcon.transform.SetParent(canvas.transform, false);
		m_DraggingIcon.transform.SetAsLastSibling();

		var image = m_DraggingIcon.AddComponent<Image>();

		image.sprite = itemIcon.sprite;
        image.SetNativeSize();

        if (dragOnSurfaces)
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;

		SetDraggedPosition(eventData);

		// 베이스 아이템에서 선택된 녀석의 정보를 저장
		BaseItem.SelectedItem.ItemInfo = baseItemInfo;
		BaseItem.SelectedItem.selectItemInfo = eventData.pointerEnter.gameObject.GetComponent<BaseItem>();
		//BaseItem.SelectedItem.selectItemInfo = transform.GetComponent<BaseItem>();

		m_DraggingIcon.GetComponent<Image>().raycastTarget = false;
	}

    internal void ItemCountUpdate(int user_item_count)
    {
		itemCount.text = user_item_count.ToString();
    }

    public void OnDrag(PointerEventData data)
	{
		if (m_DraggingIcon != null)
			SetDraggedPosition(data);
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
			m_DraggingPlane = data.pointerEnter.transform as RectTransform;

		var rt = m_DraggingIcon.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlane.rotation;
		}
	}

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyDragItem();
    }

    private void DestroyDragItem()
    {
        if (m_DraggingIcon != null)
            Destroy(m_DraggingIcon);
    }

    static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;

		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}

    public void OnDrop(PointerEventData eventData)
    {
		Debug.Log("baseitemdrop");
    }

	public PointerEventData.InputButton mouseRight = PointerEventData.InputButton.Right;
	public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == mouseRight)
        {
			// 인벤에서 우클릭 했을 때 포션이라면
			if(baseItemInfo.userItemInfo.itemType == MySQL.ItemType.Potion)
			{
				// 만약 hp가 만땅이라면 먹지 않기
				if (Player.Instance.HP >= Player.Instance.MaxHP)
					return;
				Player.Instance.HP += baseItemInfo.userItemInfo.itemAmount;
				UserInfoUI.Instance.ChangeHP(Player.Instance.HP);

				var ItemsOwnedByTheUser = (from value in UserDB.instance.userInventoryItems
										   where value.userItemInfo.itemID == baseItemInfo.userItemInfo.itemID
										   select value).First();
				ItemsOwnedByTheUser.userItemCount--;
				itemCount.text = ItemsOwnedByTheUser.userItemCount.ToString();


				MySQL.instance.UserItemUpdate(baseItemInfo.userItemInfo.itemID, false);
			}
            else
			{
				// 장비라면 장착 실행
				EquipItemBase selectedItem = EquipSlotItems.instance.ItemSelect(baseItemInfo);
				 //!= null && selectedItem.equipItemInfo.user_item_info.itemID == baseItemInfo.user_item_info.itemID
				if (selectedItem.equipItemInfo.userItemInfo.itemID != 0)
					return;

                EquipSlotItems.instance.equipMounting(baseItemInfo);
				UserDB.instance.EquipMounting(baseItemInfo);
				Destroy(gameObject);
                UserItemPanel.instance.baseItemList.Remove(this); // 이건 아마도 없어도 될듯
                int index = UserDB.instance.userInventoryItems.FindIndex(x => x.userItemInfo.itemID == baseItemInfo.userItemInfo.itemID);
				UserDB.instance.userInventoryItems.RemoveAt(index);
			}
        }
    }
}
//public class MouseInputHandler : MonoBehaviour, IPointerClickHandler
//{

//	public PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
//	public PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;

//	public void OnPointerClick(PointerEventData eventData)
//	{
//		if (eventData.button == btn1)
//		{
//			// DO SOMETHING HERE
//		}
//		else if (eventData.button == btn2)
//		{
//			// DO SOMETHING HERE
//		}
//	}
//}