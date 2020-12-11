// // MIT License
// // Original author: Mohammed Iqubal Hussain (Polyandcode.com)

// using System;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.UI;

// namespace BAStudio.MultiLayoutScroller
// {
//     public class UnityGroupedMultiLayoutScroller : MultiLayoutScroller
//     {
//         protected override void Awake ()
//         {
//             base.Awake();
//         }
//         protected override void Start ()
//         {
//             base.Start();
//         }
//         // When using Unity's LayoutGroup, these elements will change size to stabilize the scroll progress
//         RectTransform ghostPaddingL, ghostPaddingR, ghostPaddingT, ghostPaddingB, innerContent;
//         HorizontalLayoutGroup viewHorizontalLayoutGroup;
//         protected override void InitView(ViewInstance view, Action onInitialized)
//         {
//             //Setting up container and bounds
//             content = view.RectTransform;
//             content.SetParent(viewport, false);
//             // Make active view match viewport
//             content.anchorMin = Vector2.zero;
//             content.anchorMax = Vector2.one; 
//             content.pivot = Vector2.up;
//             content.sizeDelta = Vector2.zero;
//             yield return null;
//             SetRecyclingBounds();

//             minActiveLayoutIndex = 0;
//             maxActiveLayoutIndex = activeSchema.views[activeViewIndex].layouts.Count;

//             activeViewInstance = view;
//             float contentXSize = 0, contentYSize = 0;
//             switch (activeViewSchema.viewLayoutType)
//             {
//                 case ViewLayoutType.UnityHorizontalLayout:
//                 {
//                     // if (ghostPaddingB == null) ghostPaddingB = new GameObject("PADDING_B", typeof(RectTransform)).GetComponent<RectTransform>();
//                     // if (ghostPaddingT == null) ghostPaddingT = new GameObject("PADDING_T", typeof(RectTransform)).GetComponent<RectTransform>();
//                     // if (ghostPaddingL == null) ghostPaddingL = new GameObject("PADDING_L", typeof(RectTransform)).GetComponent<RectTransform>();
//                     // if (ghostPaddingR == null) ghostPaddingR = new GameObject("PADDING_R", typeof(RectTransform)).GetComponent<RectTransform>();
//                     // if (innerContent == null) innerContent = new GameObject("INNER_CONTENT", typeof(RectTransform)).GetComponent<RectTransform>();
//                     // ghostPaddingB.SetParent(view.transform, false);
//                     // ghostPaddingT.SetParent(view.transform, false);
//                     // ghostPaddingL.SetParent(view.transform, false);
//                     // ghostPaddingR.SetParent(view.transform, false);
//                     // innerContent.SetParent(view.transform, false);
//                     // innerContent.sizeDelta = new Vector2(0, 0);
//                     // if (!activeViewSchema.useUnityPrefabLayout)
//                     // {
//                     //     HorizontalLayoutGroup hlg = innerContent.gameObject.AddComponent<HorizontalLayoutGroup>();
//                     //     Assert.IsNotNull(hlg);
//                     //     ContentSizeFitter csf = innerContent.gameObject.AddComponent<ContentSizeFitter>();
//                     //     Assert.IsNotNull(csf);
//                     //     csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
//                     //     hlg.padding = activeViewSchema.autoLayoutPadding;
//                     //     hlg.spacing = activeViewSchema.autoLayoutSpacing;
//                     // }
//                     if (!activeViewSchema.useUnityPrefabLayout)
//                     {
//                         HorizontalLayoutGroup hlg = activeViewInstance.gameObject.AddComponent<HorizontalLayoutGroup>();
//                         Assert.IsNotNull(hlg);
//                         ContentSizeFitter csf = activeViewInstance.gameObject.AddComponent<ContentSizeFitter>();
//                         Assert.IsNotNull(csf);
//                         csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
//                         hlg.padding = activeViewSchema.autoLayoutPadding;
//                         hlg.spacing = activeViewSchema.autoLayoutSpacing;
//                     }
//                     contentYSize = content.sizeDelta.y + activeViewSchema.autoLayoutPadding.top + activeViewSchema.autoLayoutPadding.bottom;
//                     for (var i = 0; i < maxActiveLayoutIndex; i++)
//                     {
//                         contentXSize += layoutTypeMeta[activeSchema.views[activeViewIndex].layouts[i].typeID].layoutWidth;
//                     }
//                     contentXSize += activeViewSchema.autoLayoutSpacing * (maxActiveLayoutIndex - 1); // Spacing
//                     contentXSize += activeViewSchema.autoLayoutPadding.left + activeViewSchema.autoLayoutPadding.right; // Padding
//                     content.sizeDelta = new Vector2(contentXSize, contentYSize);

//                     if (horizontalScrollbar != null)
//                     {
//                         horizontalScrollbar.value = 0;
//                         horizontalScrollbar.size = viewport.rect.width / contentXSize;
//                     }
//                     break;
//                 }
//                 case ViewLayoutType.UnityVerticalLayout:
//                 {
//                     if (ghostPaddingB == null) ghostPaddingB = new GameObject("PADDING_B", typeof(RectTransform)).GetComponent<RectTransform>();
//                     if (ghostPaddingT == null) ghostPaddingT = new GameObject("PADDING_T", typeof(RectTransform)).GetComponent<RectTransform>();
//                     if (ghostPaddingL == null) ghostPaddingL = new GameObject("PADDING_L", typeof(RectTransform)).GetComponent<RectTransform>();
//                     if (ghostPaddingR == null) ghostPaddingR = new GameObject("PADDING_R", typeof(RectTransform)).GetComponent<RectTransform>();
//                     if (innerContent == null) innerContent = new GameObject("INNER_CONTENT", typeof(RectTransform)).GetComponent<RectTransform>();
//                     ghostPaddingB.SetParent(view.transform, false);
//                     ghostPaddingT.SetParent(view.transform, false);
//                     ghostPaddingL.SetParent(view.transform, false);
//                     ghostPaddingR.SetParent(view.transform, false);
//                     innerContent.SetParent(view.transform, false);
//                     innerContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (view.transform as RectTransform).rect.width);
//                     if (!activeViewSchema.useUnityPrefabLayout)
//                     {
//                         VerticalLayoutGroup hlg = view.GetComponent<VerticalLayoutGroup>();
//                         Assert.IsNotNull(hlg);
//                         ContentSizeFitter csf = view.GetComponent<ContentSizeFitter>();
//                         Assert.IsNotNull(csf);
//                         csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
//                         hlg.padding = activeViewSchema.autoLayoutPadding;
//                         hlg.spacing = activeViewSchema.autoLayoutSpacing;
//                     }
//                     contentXSize = content.sizeDelta.x + activeViewSchema.autoLayoutPadding.left + activeViewSchema.autoLayoutPadding.right;
//                     for (var i = 0; i < maxActiveLayoutIndex; i++)
//                     {
//                         contentYSize += layoutTypeMeta[activeSchema.views[activeViewIndex].layouts[i].typeID].layoutHeight;
//                     }
//                     contentYSize += activeViewSchema.autoLayoutSpacing * (maxActiveLayoutIndex - 1); // Spacing
//                     contentYSize += activeViewSchema.autoLayoutPadding.top + activeViewSchema.autoLayoutPadding.bottom; // Padding
//                     content.sizeDelta = new Vector2(contentXSize, contentYSize);

//                     if (verticalScrollbar != null)
//                     {
//                         verticalScrollbar.value = 0;
//                         verticalScrollbar.size = viewport.rect.width / contentXSize;
//                     }
//                     break;
//                 }
//                 default:
//                 {
//                     throw new System.NotImplementedException();
//                 }
//             }
//             if (onInitialized != null) onInitialized();
//         }

//         protected override void OnScrolledRight()
//         {
//             _recycling = true;
//             float leftSize = ghostPaddingL.rect.width, rightSize = ghostPaddingR.rect.width;
//             float spacing = activeViewInstance.GetComponent<HorizontalLayoutGroup>().spacing;
//             float contentSize = innerContent.rect.width;
//             //Recycle until cell at left is avaiable and current item count smaller than datasource
//             while (activeViewInstance.layouts[minActiveLayoutIndex].RectTransform.MaxX() < _recyclableViewBounds.min.x && minActiveLayoutIndex < activeViewSchema.layouts.Count)
//             {
//                 // pool the leftmos layout
//                 // add the layout width to left ghost padding width
//                 // add a spacing to left ghost padding width
//                 float delta = activeViewInstance.layouts[minActiveLayoutIndex].RectTransform.rect.width + spacing;
//                 leftSize += delta;
//                 contentSize -= delta;
//                 LayoutInstance li = activeViewInstance.layouts[minActiveLayoutIndex];
//                 li.gameObject.SetActive(false); // When toggling & reparenting a UI object, the order matters a lot when we are pursuing performance
//                 li.RectTransform.SetParent(hidden.transform);
//                 layoutPool[activeViewSchema.layouts[minActiveLayoutIndex].typeID].Push(li);
//                 minActiveLayoutIndex++;
//             }
//             // While there're still more layouts on the right side should be loaded
//             while (maxActiveLayoutIndex < activeViewSchema.layouts.Count - 1 && spacing + activeViewInstance.layouts[maxActiveLayoutIndex].RectTransform.MaxX() < _recyclableViewBounds.max.x)
//             {
//                 int newLayoutIndex = maxActiveLayoutIndex + 1;
//                 LayoutInstance li = layoutPool[activeViewSchema.layouts[newLayoutIndex].typeID].Pop().GetComponent<LayoutInstance>();
//                 li.transform.SetParent(innerContent, false);
//                 rightSize -= (spacing + (li.transform as RectTransform).rect.width);
//                 for (int i = 0; i < li.items.Length; i++)
//                 {
//                     li.items[i].enabled = false;
//                     li.Assign(i, li.items[i]);
//                     li.items[i].SetData(DataSource[activeViewSchema.layouts[newLayoutIndex].items[i].id]);
//                     li.items[i].enabled = true;
//                 }
//                 li.gameObject.SetActive(true); // When toggling & reparenting a UI object, the order matters a lot when we are pursuing performance
//                 activeViewInstance.layouts.Add(newLayoutIndex, li);
//                 maxActiveLayoutIndex++;
//             }

//             viewHorizontalLayoutGroup.padding.left = (int) leftSize;
//             viewHorizontalLayoutGroup.padding.right = (int) rightSize;
//             // ghostPaddingL.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftSize);
//             innerContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, leftSize, contentSize);
//             // ghostPaddingR.Sets(RectTransform.Axis.Horizontal, rightSize);
//             _recycling = false;
//             // return new Vector2(ghostPaddingL.MaxX(), 0);
//         }
//         protected override void OnScrolledLeft()
//         {
//             _recycling = true;
//             float leftSize = ghostPaddingL.rect.width, rightSize = ghostPaddingR.rect.width;
//             float spacing = activeViewInstance.GetComponent<HorizontalLayoutGroup>().spacing;
//             while (activeViewInstance.layouts[maxActiveLayoutIndex].RectTransform.MinX() > _recyclableViewBounds.max.x && maxActiveLayoutIndex > 0)
//             {
//                 // pool the leftmos layout
//                 // add the layout width to left ghost padding width
//                 // add a spacing to left ghost padding width
//                 rightSize -= (activeViewInstance.layouts[maxActiveLayoutIndex].RectTransform.rect.width + spacing);
//                 LayoutInstance li = activeViewInstance.layouts[maxActiveLayoutIndex];
//                 li.RectTransform.SetParent(hidden.transform);
//                 layoutPool[activeViewSchema.layouts[maxActiveLayoutIndex].typeID].Push(li);
//                 maxActiveLayoutIndex++;
//             }
//             // While there're still more layouts on the left side should be loaded
//             while (minActiveLayoutIndex > 0 && activeViewInstance.layouts[minActiveLayoutIndex].RectTransform.MinX() - spacing > _recyclableViewBounds.max.x)
//             {
//                 int newLayoutIndex = minActiveLayoutIndex - 1;
//                 LayoutInstance li = layoutPool[activeViewSchema.layouts[newLayoutIndex].typeID].Pop().GetComponent<LayoutInstance>();
//                 li.transform.SetParent(innerContent, true);
//                 li.transform.SetAsFirstSibling();
//                 leftSize -= (spacing + (li.transform as RectTransform).rect.width);
//                 for (int i = 0; i < li.items.Length; i++)
//                 {
//                     li.items[i].SetData(DataSource[activeViewSchema.layouts[newLayoutIndex].items[i].id]);
//                 }
//                 activeViewInstance.layouts.Add(newLayoutIndex, li);
//                 minActiveLayoutIndex--;
//             }

//             _recycling = false;
//             // return new Vector2(ghostPaddingL.MaxX(), 0);
//         }
//         protected override void OnDestroy ()
//         {
//             base.OnDestroy();
//         }
//     }

//     // LayoutName: { 0, 1, 4, 2, 11 ,155}  => The layout pulls 

//     // MultiLayoutScroller
//     // - View (Where scrolling happening)
//     //   - Layout (A set of predefined item positon&dimension)
//     //     - Item ()
    
//     // View
//     // View controls how layouts are placed, moved, culled

//     // Layouts
//     // A layout is just a set of predefined slots, items get place under the slot transforms
//     // It does not control it's own placement

//     // Item
//     // Prefab driving data

// }