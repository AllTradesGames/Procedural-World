Welcome to CTS - the Complete Terrain Shader!
================================

The CTS terrain shader is a profile driven shader, which means that you can generate as many profiles as you like, and apply them any time you want. 

CTS profiles keep their settings as you switch between design time and runtime. This allows you to configure your profile exactly the way you want in game, and retained the settings when you exit back to editor mode.

SETUP :

1. Set your lighting to linear / deferred for best visuals.
Window -> CTS -> Set Linear Deffered

2. Create a terrain and add your textures (CTS supports up to 16 textures).

3. Add CTS to your terrain.
Select your terrain and then select Component -> CTS -> Add CTS To Terrain.

OR

3. Add CTS to all your terrains.
Window -> CTS -> Add CTS To All Terrains.

4. Create a new CTS profile and apply it to the terrain:
Window -> CTS -> Create And Apply Profile

5. Select your CTS Profile:
New profiles are created in the CTS / Profiles directory. You can a profile by selecting it and hitting the 'Apply Profile'. NOTE : Applying a profile will overwrite any textures that were previously stored in the terrain.

NOTE: There is a collection of sample CTS Profiles in CTS / Textures Library / Texture Library, and a set of profiles that use them at CTS / Textures Library / Profile Library

6. Edit your CTS Profile:
Select and apply the CTS profile. You can then edit in the inspector and changes will be reflected into the terrain in real time. When the profile turns RED then you will need to re-bake your texures for the changes to be applied to your terrain. This only need to be done when you change your textures.

FOOTSTEPS :
If you use an asset that relies on terrain splatmaps being present to control how your footsteps sound, please de-select 'Strip Textures' in your Optimisation Settings on your profile.

POST FX :
If you want to use the Post FX in the demo's please install the Unity Post Processing Stack from https://www.assetstore.unity3d.com/en/#!/content/83912

RUNTIME BUILDING (Only for non runtime generated terrains) :
Under your profiles Optimization Settings make sytem your strip your textures. Bake your profile. Save your scene. Make sure that DX9 is disabled in your build API's. Go into Build Settings, Player Settings, Other Settings. Uncheck Auto Graphics API for windows. Remove DX9 from list.

RUNTIME TERRAIN / MAP MAGIC SUPPORT :
Check out CTSRuntimeTerrainHelper.cs in the scripts directory. Details of how to use it have been added into the introduction. A proper Map Magic integration has been created by its author.

PERFORMANCE :
We have include some handy System Metrics and a FPS script in the Prefabs directory. It will tyically show less than half the framerate that Unity shows in the editor, but when we compared it against NVidia's FPS information it seemed a lot more accurate. Drag this into your scene to get a more realistic view of your runtime frame rate. In a runtime build you can also expect 2-4x better framerates that what we show in the editor.

WEATHER :
To add weather to your terrain select Window -> CTS -> Add Weather Manager. You can then use this interface to control how your terrain responds to different weather. If you are integrating via script, CTS will signal its presence via the CTS_PRESENT define.

World Manager API :
To add world manager API to your terrain select Window -> CTS -> Add World Manager API. You can control how your terrain responds to weather via the World Manager API. Learn more here : http://www.procedural-worlds.com/blog/wapi/

DEMOS : 
You can run the demo's in the Texures Library / Demo directory. To take advantage of the post effects used you will also need to install the Unity Post Processing Stack from https://www.assetstore.unity3d.com/en/#!/content/83912

HELP : 
CTS is self documenting, so to get help on any component, just hit the ? at the top of the component. Additional help and video tutorials can be found at http://www.procedural-worlds.com/cts/.

NOTE : 
Do not delete this readme file. It is used to locate where CTS has been placed in your project, and removing it will break CTS.

ABOUT : 
CTS is proudly bought to you by Bartlomiej Galas (Nature Manufacture) & Adam Goodrich (Procedural Worlds). Many thanks to the team at Amplify, Szymon Wojciak, Pawel Homenko and the CTS beta team for their help in bringing CTS to life!

Enjoy!!