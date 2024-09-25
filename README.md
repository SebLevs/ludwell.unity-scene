Scene Manager Toolkit

A Ludwell Studio tool

---

# Table of Content
- [Editor Toolkit](#editor-toolkit)
    - [Scene Manager](#scene-manager)
        - [Hotkeys](#scene-manager-hotkeys)
        - [Contextual Menu](#scene-manager-contextual-menu)
        - [Search Bar](#searchbar)
    - [Tag Manager](#tag-manager)
        - [Hotkeys](#hotkeys)
        - [Contextual Menu](#tag-manager-contextual-menu)
- [Runtime Toolkit](#runtime-toolkit)
    - [SceneAssetReference](#sceneassetreference)
    - [SceneAssetReferenceEvent](#sceneassetreferenceevent)

---

# Editor Toolkit

Open / Close the editor window
- Tools > Ludwell Studio > Scene Manager Toolkit
- CTRL + SHIFT + ALT + S

## Scene Manager

List all SceneAssets.

Every listed element offers buttons to easily manage, find, and relocate SceneAssets.

The Scene Asset can also be renamed through its associated element.

### Scene Manager Hotkeys

Delete the current selection
- CTRL + DEL

### Scene Manager Contextual Menu
SceneAsset-related actions can be executed in bulk through the right mouse button contextual menu.

### Search Bar
The Scene Manager search bar has multiple search options.

Clicking the icon located at the inner-left side of the bar will cycle through the search behaviours.

Here is the exhaustive list of search behaviours:
- Search by name
- Search by tag
- Search by scenes in Hierarchy

## Tag Manager

A [Tag](./Runtime/Scripts/Tag/Tag.cs) can be bound to any number of [SceneAssetData](./Runtime/Scripts/SceneAssetData/SceneAssetData.cs).

The list of tag for any given SceneAsset can be retrieved at runtime through [SceneAssetReference](./Runtime/Scripts/SceneAssetReference/SceneAssetReference.cs).

### Tag Manager Hotkeys

Bind selection
- CTRL + RETURN

Unbind selection
- CTRL + BACKSPACE

Delete selection
- CTRL + DEL

### Tag Manager Contextual Menu
Tag-related actions can be executed in bulk through the right mouse button contextual menu.

---

# Runtime Toolkit

## SceneAssetReference

[SceneAssetReference](./Runtime/Scripts/SceneAssetReference/SceneAssetReference.cs)

Used to reference a SceneAsset in the editor and access its information at runtime.

The information is accessed through both [SceneAssetData](./Runtime/Scripts/SceneAssetData/SceneAssetData.cs) and its related [SceneAssetDataBinder](./Runtime/Scripts/SceneAssetData/SceneAssetDataBinder.cs).

In the inspector, additional buttons are made available to easily manage the state of a SceneAsset for build purposes.

The provided information includes:
- List of tags associated with the SceneAsset in the Scene Manager Toolkit window
- Guid
- Name
- path
- Addressable id
- IsAddressable
- Build index

## SceneAssetReferenceEvent

[SceneAssetReferenceEvent](./Runtime/Scripts/SceneAssetReference/SceneAssetReferenceEvent.cs)

Use this script instead of a SceneAsset to bypass the serialization limitation of SceneAsset.

Add logic to the script's UnityEvent that uses the script's [SceneAssetReference](./Runtime/Scripts/SceneAssetReference/SceneAssetReference.cs) as parameter.

Refer to the script in a serialized field and raise the event to invoke a behaviour that uses the [SceneAssetReference](./Runtime/Scripts/SceneAssetReference/SceneAssetReference.cs).

## SceneAssetDataBinders

[SceneAssetDataBinders](./Runtime/Scripts/SceneAssetData/SceneAssetDataBinders.cs)

Contains all [SceneAssetDataBinder](./Runtime/Scripts/SceneAssetData/SceneAssetDataBinder.cs).

Singleton scriptable object accessed by `.Instance`;

Offers utility methods:
- Evaluate the validity of a [SceneAssetDataBinder](./Runtime/Scripts/SceneAssetData/SceneAssetDataBinder.cs)
- Evaluate the validity of a [SceneAssetData](./Runtime/Scripts/SceneAssetData/SceneAssetData.cs)
- Get a [SceneAssetDataBinder](./Runtime/Scripts/SceneAssetData/SceneAssetDataBinder.cs) or a [SceneAssetData](./Runtime/Scripts/SceneAssetData/SceneAssetData.cs) from values
- Get [SceneAssetData](./Runtime/Scripts/SceneAssetData/SceneAssetData.cs)s from a tag
- etc.