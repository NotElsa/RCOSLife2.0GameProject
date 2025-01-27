using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


/* This abstract class represent the slots in the player's inventory */
/* Codes provided by: Dan Pos - Game Dev Tutorials! */

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;    

    protected InventorySystem inventorySystem; //inventory that we are trying to display on the UI canvas
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary; //pair up UI slot with system slot

    //getter
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem invToDisplay, int offset); //to be implement in child classes

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        // Look through every slot in the dictionary
        // key: InventorySlot_UI
        // value: InventorySlot
        foreach (var slot in slotDictionary)
        {
            // if the value of the entry is equal to the slot that we pass in
            if(slot.Value == updatedSlot)
            {
                // update the UI correspond to the inventory
                // Slot key - UI representation of the value
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    //This function will incharge of picking up and placing item in the Hotbar
    public void SlotClicked(InventorySlot_UI clickedUISlot)
    {
        /* Change so that the user don't have to just press Shift to split but let
        user customize the split key in the new Unity input system. (This is hardcoding :( ) */
        bool playerPressedShift = Keyboard.current.leftShiftKey.isPressed;
        
        
        //         If clicked slot have item and                             mouse is not holding an item
        if(clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            //If player is holding shift key? Split the Stack from inventory slot and 
            //put it into mouse inventory
            if (playerPressedShift && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot)) //split stacc
            {
                mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                return;
            }
            else //pick up item in the clicked slot
            {
                mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                return;
            }
        }

        // If clicked slot don't have item and mouse is holding an item, then place down the item
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }
        // If clicked slot has an item and mouse is holding an item, decide what to do?
        //Are both item the same?
        //Yes: Combine the item
        // If slot total item + mouse total item <= stackSize, combine those 2
        // If slot total item + mouse total item > stackSize, combine unil = to stackSize of the item on the slot and leave the rest on the mouse
        //No: Swap item of the slot and the mouse;

        //both  mouse inventory and the clicked slot have item
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool sameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            if (sameItem && clickedUISlot.AssignedInventorySlot.IsEnoughSpaceInStack(mouseInventoryItem.AssignedInventorySlot.StackSize)) //combine items it does not reach the max stack
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            else if(sameItem && 
                !clickedUISlot.AssignedInventorySlot.IsEnoughSpaceInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
            {
                //same item, not room left in the stack
                if(leftInStack < 1)
                {
                    //stack full so swap item
                    SwapSlots(clickedUISlot);
                }
                else
                {
                    //slot not at max, take item from mous inventory until max stack
                    int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }
            //both mouse inventory and clicked slot does not have the same item
            if (!sameItem)
            {
                SwapSlots(clickedUISlot);
            } 
        }

    }

    private void SwapSlots(InventorySlot_UI clickedUISlot)
    {

        //clone the item on the mouse and store it, then remove the item in the mouse inventory
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        //update the mouseInventory with the desired swapped item
        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        //empty the spot that the player clicks
        clickedUISlot.ClearSlot();

        //then put back the item that we store (in the temp var) from the mouse at the beginning to the clicked slot.
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }
}
