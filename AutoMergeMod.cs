using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using Il2CppPussyIsland.Screens.MergeBoard;
using Il2CppPussyIsland.Items;

[assembly: MelonInfo(typeof(AutoMergePlugin.AutoMergeMod), "Pussy Island AutoMerge", "1.3.0", "Custom")]
[assembly: MelonGame("Bulkagames", "PussyIsland")]

namespace AutoMergePlugin
{
    public class AutoMergeMod : MelonMod
    {
        private bool autoMergeEnabled = false;
        private bool autoCollectEnabled = false;
        private float nextActionTime = 0f;
        private BoardManager cachedBoard = null;

        public override void OnUpdate()
        {
            Keyboard current = Keyboard.current;
            if (current != null)
            {
                if (current.f5Key.wasPressedThisFrame)
                {
                    autoMergeEnabled = !autoMergeEnabled;
                    MelonLogger.Msg($"[AutoMerge] Auto-Merge (F5): {(autoMergeEnabled ? "ON" : "OFF")}");
                }

                if (current.f6Key.wasPressedThisFrame)
                {
                    autoCollectEnabled = !autoCollectEnabled;
                    MelonLogger.Msg($"[AutoMerge] Auto-Collect (F6): {(autoCollectEnabled ? "ON" : "OFF")}");
                }
            }

            // сли игрок кликает мышкой (достаёт из инвентаря или тапает генератор), даём анимациям 0.6 сек завершиться
            Mouse mouse = Mouse.current;
            if (mouse != null && (mouse.leftButton.isPressed || mouse.leftButton.wasReleasedThisFrame))
            {
                nextActionTime = Math.Max(nextActionTime, Time.unscaledTime + 0.6f);
                return;
            }

            if ((!autoMergeEnabled && !autoCollectEnabled) || Time.unscaledTime < nextActionTime)
                return;

            nextActionTime = Time.unscaledTime + 0.45f;

            if (!EnsureBoard()) return;

            if (autoMergeEnabled && TryMergeOnePair())
                return;

            if (autoCollectEnabled)
                TryCollectOneResource();
        }

        private bool EnsureBoard()
        {
            if (cachedBoard != null && cachedBoard.isActiveAndEnabled)
                return true;

            var boards = UnityEngine.Object.FindObjectsOfType<BoardManager>();
            if (boards == null || boards.Length == 0) return false;

            foreach (var b in boards)
            {
                if (b != null && b.isActiveAndEnabled)
                {
                    cachedBoard = b;
                    return true;
                }
            }
            return false;
        }

        private bool IsValidBoardItem(GridItemView view)
        {
            if (view == null || !view.isActiveAndEnabled || !view.gameObject.activeInHierarchy)
                return false;

            if (view.isReleaseProcess || view.isDragging)
                return false;

            // дём завершения полёта предмета из инвентаря или генератора
            if (view.moveTween != null && view.moveTween.active)
                return false;

            if (view.Slot == null || view.Slot.ItemView != view)
                return false;

            if (view.Item == null || view.Item.IsWrappedBoxOrWeb)
                return false;

            return true;
        }

        private bool TryMergeOnePair()
        {
            try
            {
                var mergeableDict = cachedBoard.mergeableItems;
                if (mergeableDict == null || mergeableDict.Count == 0) return false;

                foreach (var entry in mergeableDict)
                {
                    var itemsList = entry.Value;
                    if (itemsList != null && itemsList.Count >= 2)
                    {
                        GridItemView item1 = itemsList[0];
                        GridItemView item2 = itemsList[1];

                        if (item1 != item2 && IsValidBoardItem(item1) && IsValidBoardItem(item2))
                        {
                            cachedBoard.HandleMove(item1, item2.Slot);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[AutoMerge] Merge error: {ex.Message}");
            }
            return false;
        }

        private void TryCollectOneResource()
        {
            try
            {
                var items = cachedBoard.availableItems;
                if (items == null || items.Count == 0) return;

                for (int i = 0; i < items.Count; i++)
                {
                    GridItemView view = items[i];
                    if (!IsValidBoardItem(view)) continue;

                    Item item = view.Item;
                    if (item.IsMaxLevel && item.ItemType.ToString().IndexOf("Resource", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cachedBoard.CollectResource(view);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[AutoMerge] Collect error: {ex.Message}");
            }
        }
    }
}
