﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3074
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cfix.LicAdmin {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cfix.LicAdmin.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are using a licensed copy of Visual Assert. You may use this form 
        ///to enter a different license key..
        /// </summary>
        internal static string InfoChangeKey {
            get {
                return ResourceManager.GetString("InfoChangeKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The evaluation period has expired. To continue using Visual Assert, 
        ///please enter a valid license key..
        /// </summary>
        internal static string InfoExpired {
            get {
                return ResourceManager.GetString("InfoExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To activate Visual Assert, please enter a valid license key..
        /// </summary>
        internal static string InfoLicense {
            get {
                return ResourceManager.GetString("InfoLicense", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installing the license key failed: {0}.
        /// </summary>
        internal static string InstallFailed {
            get {
                return ResourceManager.GetString("InstallFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installation failed.
        /// </summary>
        internal static string InstallFailedCaption {
            get {
                return ResourceManager.GetString("InstallFailedCaption", resourceCulture);
            }
        }
    }
}
