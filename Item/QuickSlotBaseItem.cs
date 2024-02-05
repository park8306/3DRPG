using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(Image))]
public class QuickSlotBaseItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	public Image frame;
	public Image quickSlotItemIcon;
	public Image nofify;
	public TextMeshProUGUI quickSlotItemCount;
	public TextMeshProUGUI quickSlotItemNumber;

	static public int KeyNumber = 0;
	public KeyCode keyNumber = KeyCode.Alpha0;

	BaseItem dropedItem;
	public UserItem dropItemInfo;

	Sprite firstSprite;
	Color firstColor;
	internal void Init()
	{
		++KeyNumber;
		keyNumber = keyNumber + KeyNumber;
		quickSlotItemCount = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
		quickSlotItemNumber = transform.Find("ItemNumber").GetComponent<TextMeshProUGUI>();
		frame = transform.Find("GradeFrame").GetComponent<Image>();
		quickSlotItemIcon = transform.Find("ItemIcon").GetComponent<Image>();
		nofify = transform.Find("Nofify").GetComponent<Image>();

		firstSprite = quickSlotItemIcon.sprite;
		firstColor = quickSlotItemIcon.color;

		dropItemInfo = null;
	}

    private void Update()
    {
		// 유저의 hp가 최대 hp보다 크거나 같고 드롭된 아이템이 없거나 아이템 타입이 포션이 아니면 실행 안함
		if (dropItemInfo == null || 
			dropItemInfo.userItemInfo.itemType != MySQL.ItemType.Potion)
		{
			return;
		}
		if (Input.GetKeyDown(keyNumber))
        {
			if (Player.Instance.HP >= Player.Instance.MaxHP || ItemsOwnedByTheUser.userItemCount <= 0)
				return;
            Debug.Log($"{keyNumber}퀵슬롯 사용");

            // 플레이어 hp회복
            Player.Instance.HP += dropItemInfo.userItemInfo.itemAmount;
			if (Player.Instance.HP > Player.Instance.MaxHP)
				Player.Instance.HP = Player.Instance.MaxHP;
			UserInfoUI.Instance.ChangeHP(Player.Instance.HP);
			//퀵슬롯, 인벤토리, 데이터베이스에 포션 수 감소
			dropItemInfo.userItemCount--; // 포션 수 감소해주고
			ItemsOwnedByTheUser.userItemCount--;

			// 위의 방법이 안되니까 함수로 만들어서 해결해보자
			dropedItem.ItemCountUpdate(ItemsOwnedByTheUser.userItemCount);

            //quickSlotItemCount.text = ItemsOwnedByTheUser.user_item_count.ToString(); // 얜 퀵슬롯

			MySQL.instance.UserItemUpdate(dropItemInfo.userItemInfo.itemID, false); // 데이터베이스에 수 줄여줌
		}
		quickSlotItemCount.text = ItemsOwnedByTheUser.userItemCount.ToString();
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

		image.sprite = quickSlotItemIcon.sprite;
		image.SetNativeSize();

		if (dragOnSurfaces)
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;

		SetDraggedPosition(eventData);
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
	public UserItem ItemsOwnedByTheUser;
	public void OnDrop(PointerEventData eventData)
	{
		if ((BaseItem.SelectedItem.ItemInfo.userItemInfo.itemType & MySQL.ItemType.Potion) != MySQL.ItemType.Potion)
			return;

		QuickSlotBaseItem QuickSlotItemSame = new QuickSlotBaseItem();

		for (int i = 0; i < QuickSlotItems.instance.baseItems.Length; i++)
        {
			if (QuickSlotItems.instance.baseItems[i].dropItemInfo == null)
				continue;
			QuickSlotItemSame = QuickSlotItems.instance.baseItems[i];
			QuickSlotItemSame.dropedItem = null;
			QuickSlotItemSame.dropItemInfo = null;
			QuickSlotItemSame.quickSlotItemIcon.sprite = firstSprite;
			QuickSlotItemSame.quickSlotItemIcon.color = firstColor;
			QuickSlotItemSame.quickSlotItemCount.text = "0";
			ItemsOwnedByTheUser = null;
		}

		dropedItem = BaseItem.SelectedItem.selectItemInfo;
		dropItemInfo = BaseItem.SelectedItem.ItemInfo;
		quickSlotItemIcon.sprite = dropedItem.itemIcon.sprite;
		quickSlotItemIcon.color = Color.white;
		quickSlotItemCount.text = BaseItem.SelectedItem.selectItemInfo.baseItemInfo.userItemCount.ToString();

		ItemsOwnedByTheUser = (from value in UserDB.instance.userInventoryItems
								   where value.userItemInfo.itemID == dropItemInfo.userItemInfo.itemID
								   select value).First();
	}
}
