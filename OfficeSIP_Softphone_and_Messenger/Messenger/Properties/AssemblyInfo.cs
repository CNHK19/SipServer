// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle(Messenger.AssemblyInfo.AssemblyTitle)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Messenger.AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct(Messenger.AssemblyInfo.AssemblyProduct)]
[assembly: AssemblyCopyright(Messenger.AssemblyInfo.AssemblyCopyright)]
[assembly: AssemblyTrademark(Messenger.AssemblyInfo.AssemblyTrademark)]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]


[assembly: AssemblyVersion(Messenger.AssemblyInfo.AssemblyVersion + ".0")]
[assembly: AssemblyFileVersion(Messenger.AssemblyInfo.AssemblyFileVersion + ".0")]
