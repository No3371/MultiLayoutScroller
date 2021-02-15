# MultiLayoutScroller
A fully de-coupled, data-based infinitely looping scroller for Unity.

## View, Layout, Item
A MultiLayoutScroller instance may loaded with multiple views. A View is a root page that may contains any amount of layouts, and a layout may contains any amount of items.

## Schema, Instance
A schema is a data object that define a View/Layout/Item. For example, view schema defines the scrolling direction of a View (Horizontal/Vertical), layout schema defines what items it have, and a item schema is a pair of (type, itemID).

On the other hand, a instance is a runtime MonoBehaviour that use Unity UGUI to display stuff by how schema defines it. Users could simply extend the base instance classes to provide custom behaviour. Usually, a instance Monobehaviour is packed in a prefab visually designed to work with the instance class behaviour. 

## DataSource
As you may have noticed, the item schema only contains item ID, not actual data to be displayed, the system loads what to be displayed everytime a item comes in the view, the content data is fully separated, the data could be anything that implement `IDataSource` interface and will be injected into the MultiLayoutScroller at runtime.

Because everything is based on data, **we can even dynamically generate/modify contents on the fly, or store the data in ways**, jsons, scriptable objects, online resources, you name it.

## Layout vs Item
To correctly use the system, it's important to remember that it's layout that gets scrolled, not item. Layout act as "item group", whether it's a single item group or a multi-item group, it get scrolled as a whole. Item instances are placed relative to its host (layout instance), the enables multiple layout to be easily managed in single scroller.
