### Simple Asset Manager
This class scans folders and prefabs in **Resources** folder and then generates a C# script that contains enums based on it.

So you can create prefab like this with a single drag'n drop:
```Unity
Instantiate(AssetManager.GetPrefab(Prefab.UI.SpawnButton_1));
```
This is designed specifically for my side scroller project.
- Compared to **Asset Bundle** or **Addressable**, this is more suitable for personal learning projects.
- I made this in order to streamline the process of registering and creating prefabs.
- You can see the whole code from my [RTS project](https://github.com/DevSongOfficial/Real-Time-Strategy).

#
### How To Use
1. Place any prefab you want to access at runtime through AssetManager.cs into the **Resources** folder.
2. Open the editor window. (Window -> Custom Asset Window)
3. Based on your folder structure, After clicking **Save** Button in the editor window, a corresponding **Prefab.cs** file will be automatically generated/edited.
4. Now you can create your prefabs by calling `AssetManager.GetPrefab(prefabName);`
#
