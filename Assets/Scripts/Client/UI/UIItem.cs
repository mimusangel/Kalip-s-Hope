using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    public int slot;
    private Image _spriteImage;
    private UIItem _selectedItem;
    private Text _number;

    private void Awake()
    {
        _selectedItem = GameObject.Find("SelectedItem").GetComponent<UIItem>();
        _spriteImage = GetComponent<Image>();
        _number = GetComponentInChildren<Text>();
        UpdateItem(null);
    }

    public void UpdateItem(Item item)
    {
        this.item = item;
        if (this.item != null)
        {
            _spriteImage.color = Color.white;
            _spriteImage.sprite = Resources.Load<Sprite>("Sprites/UI/Items/" + item.template);
            if (item.number > 1)
                _number.text = item.number.ToString();
            else
                _number.text = "";
        }
        else
        {
            _spriteImage.color = Color.clear;
            _number.text = "";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_selectedItem.item == null && this.item != null)
        {
            _selectedItem.UpdateItem(this.item);
            UpdateItem(null);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_selectedItem.item != null)
        {
            UIItem pointed = UIInventory.instance.PointedSlot();
            if (pointed != null && pointed.slot != slot)
            {
                ClientSocketScript css = GameManager.instance.GetComponent<ClientSocketScript>();
                css.Send(
                    PacketHandler.newPacket(PacketHandler.PacketID_MoveItem,
                        _selectedItem.item.slot,
                        pointed.slot
                    )
                );
                UIInventory.instance.inventory.SwapItem(_selectedItem.item.slot, pointed.slot);
                UpdateItem(pointed.item);
                pointed.UpdateItem(_selectedItem.item);
            }
            else
                UpdateItem(_selectedItem.item);
            _selectedItem.UpdateItem(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIInventory.instance.pointedSlot = slot;
        if (this.item != null)
        {
            ItemTooltip.instance.GenerateTooltip(this.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIInventory.instance.pointedSlot = -1;
        ItemTooltip.instance.gameObject.SetActive(false);
    }
}
