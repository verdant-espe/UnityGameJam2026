Original Unity-ready D100 with 100 planar faces numbered 1 through 100.
Files: D100_Numbered.obj, D100_Numbered.mtl, D100_Numbered_Texture.png
In Unity, drag all three files into Assets (e.g. Assets/Dice). Drag the OBJ model into the scene.
For URP, if the texture does not show, create a URP/Lit material, assign the PNG as Base Map, and apply the material to the model.
Use the existing D100RollAnimation component on a parent GameObject.
The visible face numbering is decoration. The existing LuckyNumberManager generates the actual fair 1-100 game roll.
Original generated geometry and artwork. Freely use, modify, and share in this game project.
