# Setup Guide

## Requirements
- Windows
- .NET Framework


## Steps
1. Download WebControlsAI project zip file
2. Extract the zip file
3. Create a new Empty ASP.NET .NET Framework 4.5.2 web application
4. Install the latest Newtonsoft.Json NuGet package to the project
5. Add  DragList.vb, and MessageControl.vb to the project from the above extracted zip data
6. Create the CSS, Images, and Scripts folders
7. Add existing files for each of those folders from above extracted zip data
8. Set the BuildAction property for each of the above files to "Embedded Resource"
9. Edit the AssemblyInfo.vb file:

--- After the <Assembly: AssemblyTradmark(...) line, add  <Assembly: TagPrefix("[Root Namespace]", "WC")>
--- After the <Assembly: AssemblyFileVersion(...) line add the following lines:

<Assembly: System.Web.UI.WebResource("[Root Namespace].DragList.js", "text/javascript")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].DragList.css", "text/css")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].MessageControl.js", "text/javascript")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].MessageControl.css", "text/css")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].picError.Image.png", "image/png")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].picExclaim.Image.png", "image/png")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].picInfo.Image.png", "image/png")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].picQuestion.Image.gif", "image/gif")>
<Assembly: System.Web.UI.WebResource("[Root Namespace].Stop.gif", "image/gif")>

10. Build the solution. The [Assembly Name].dll file is created in the bin directory in the [configuration] folder. 
11. Add a reference to this file in your web project to use these controls

