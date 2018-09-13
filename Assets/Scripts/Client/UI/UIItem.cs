using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    private Image _spriteImage;
    private UIItem _selectedItem;
    private Text _number;
    private ItemTooltip _tooltip;

    private void Awake()
    {
        _selectedItem = GameObject.Find("SelectedItem").GetComponent<UIItem>();
        _tooltip = GameObject.Find("Tooltip").GetComponent<ItemTooltip>();
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
            _spriteImage.sprite = Resources.Load<Sprite>("Sprites/UI/Items/" + item.template); //item.icon;
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
        if (this.item != null)
        {
            if (_selectedItem.item != null)
            {
                Item clone = new Item(_selectedItem.item);
                _selectedItem.UpdateItem(this.item);
                UpdateItem(clone);
            }
            else
            {
                _selectedItem.UpdateItem(this.item);
                UpdateItem(null);
            }
        }
        else if (_selectedItem.item != null)
        {
            UpdateItem(_selectedItem.item);
            _selectedItem.UpdateItem(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.item != null)
        {
            _tooltip.GenerateTooltip(this.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.gameObject.SetActive(false);
    }
}
