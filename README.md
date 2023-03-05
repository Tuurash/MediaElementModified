# MediaElementModified
Focusing on .net Maui Media Element Android fullscreen Video Issue Which is not currently supported by .net Maui [MediaElement](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/mediaelement).

<h4 align="center">Demonstration</h2>   

<table width="100%" style="padding:10px">
  <tr>
    <td width="50%" align="center" valign="center"><h5>Protrait</h5>
    </td>
    <td width="50%" align="center" valign="center"><h5>Full Screen</h5>
    </td>
  </tr>


  <tr>
    <td width="50%" align="center" valign="center"><img align="center" 
        src="https://github.com/Tuurash/MediaElementModified/blob/main/Snaps/protrait.jpg" width="250"/>
    </td>
    <td width="50%" align="center" valign="center"><img align="center" 
        src="https://github.com/Tuurash/MediaElementModified/blob/main/Snaps/fullscreen.jpg" />
    </td>
  </tr>
</table>
  
  
## Overview
A custom [Device Orientation Service](https://github.com/Tuurash/MediaElementModified/blob/main/Services/DeviceOrientationService.cs) was created in order to override and manupulate orientation on the go.

```csharp
namespace MElemetModified.Services;

public partial class DeviceOrientationService{
    public partial void SetDeviceOrientation(DisplayOrientation displayOrientation);
}
```  
and this method needs to inherited into All platforms.
##### Android specific

```csharp
namespace MElemetModified.Services;

public partial class DeviceOrientationService{
    private static readonly IReadOnlyDictionary<DisplayOrientation, ScreenOrientation> _androidDisplayOrientationMap =
        new Dictionary<DisplayOrientation, ScreenOrientation>{
            [DisplayOrientation.Landscape] = ScreenOrientation.Landscape,
            [DisplayOrientation.Portrait] = ScreenOrientation.Portrait,
        };

    public partial void SetDeviceOrientation(DisplayOrientation displayOrientation){
        var currentActivity = ActivityStateManager.Default.GetCurrentActivity();
        if (currentActivity is not null)
            if (_androidDisplayOrientationMap.TryGetValue(displayOrientation, out ScreenOrientation screenOrientation))
                currentActivity.RequestedOrientation = screenOrientation;
    }
}
```  
##### Every other platform specific codes(iOS, Windows, Mac)
```csharp
namespace MElemetModified.Services;

public partial class DeviceOrientationService{
    public partial void SetDeviceOrientation(DisplayOrientation displayOrientation){}
}
```  
##### NOTICE all the namespaces. Make sure it is implemented accordingly.
As it can be seen In other platform specific codes the method can be kept empty.  

  
  
Finally, for in the media visualization page it is necessary to define platform wise condtions as iOS has no issue regarding video fullscreen.
```csharp
#if ANDROID
        // Make Fullscreen Button visible
#elif IOS
        // keep Fullscreen Button hidden
#endif
```

