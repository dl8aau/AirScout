﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirScoutPlaneServer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Windows_Startup_Minimized {
            get {
                return ((bool)(this["Windows_Startup_Minimized"]));
            }
            set {
                this["Windows_Startup_Minimized"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Windows_Startup_Systray {
            get {
                return ((bool)(this["Windows_Startup_Systray"]));
            }
            set {
                this["Windows_Startup_Systray"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Windows_Startup_Autorun {
            get {
                return ((bool)(this["Windows_Startup_Autorun"]));
            }
            set {
                this["Windows_Startup_Autorun"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Webserver_Active {
            get {
                return ((bool)(this["Webserver_Active"]));
            }
            set {
                this["Webserver_Active"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9872")]
        public decimal Webserver_Port {
            get {
                return ((decimal)(this["Webserver_Port"]));
            }
            set {
                this["Webserver_Port"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<HTML><BODY> Welcome to AirScout PlaneServer</BODY></HTML>")]
        public string Webserver_Welcome {
            get {
                return ((string)(this["Webserver_Welcome"]));
            }
            set {
                this["Webserver_Welcome"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Log_Active {
            get {
                return ((bool)(this["Log_Active"]));
            }
            set {
                this["Log_Active"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%DataDir%\\Log")]
        public string Log_Directory {
            get {
                return ((string)(this["Log_Directory"]));
            }
            set {
                this["Log_Directory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%DataDir%\\Database")]
        public string Database_Directory {
            get {
                return ((string)(this["Database_Directory"]));
            }
            set {
                this["Database_Directory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Application_FirstRun {
            get {
                return ((bool)(this["Application_FirstRun"]));
            }
            set {
                this["Application_FirstRun"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%AppDir%")]
        public string Icon_Directory {
            get {
                return ((string)(this["Icon_Directory"]));
            }
            set {
                this["Icon_Directory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PlaneServer.ico")]
        public string Icon_Filename {
            get {
                return ((string)(this["Icon_Filename"]));
            }
            set {
                this["Icon_Filename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public decimal Log_Verbosity {
            get {
                return ((decimal)(this["Log_Verbosity"]));
            }
            set {
                this["Log_Verbosity"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-15")]
        public decimal Planes_MinLon {
            get {
                return ((decimal)(this["Planes_MinLon"]));
            }
            set {
                this["Planes_MinLon"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public decimal Planes_MaxLon {
            get {
                return ((decimal)(this["Planes_MaxLon"]));
            }
            set {
                this["Planes_MaxLon"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("35")]
        public decimal Planes_MinLat {
            get {
                return ((decimal)(this["Planes_MinLat"]));
            }
            set {
                this["Planes_MinLat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public decimal Planes_MaxLat {
            get {
                return ((decimal)(this["Planes_MaxLat"]));
            }
            set {
                this["Planes_MaxLat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("OpenStreetMap")]
        public string Planes_MapProvider {
            get {
                return ((string)(this["Planes_MapProvider"]));
            }
            set {
                this["Planes_MapProvider"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public decimal Planes_MinAlt {
            get {
                return ((decimal)(this["Planes_MinAlt"]));
            }
            set {
                this["Planes_MinAlt"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ADSBFeed_Active {
            get {
                return ((bool)(this["ADSBFeed_Active"]));
            }
            set {
                this["ADSBFeed_Active"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::AirScout.PlaneFeeds.PlanefeedSettings_WF WebFeed1_Settings {
            get {
                return ((global::AirScout.PlaneFeeds.PlanefeedSettings_WF)(this["WebFeed1_Settings"]));
            }
            set {
                this["WebFeed1_Settings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20000")]
        public decimal Planes_MaxAlt {
            get {
                return ((decimal)(this["Planes_MaxAlt"]));
            }
            set {
                this["Planes_MaxAlt"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public decimal Planes_Lifetime {
            get {
                return ((decimal)(this["Planes_Lifetime"]));
            }
            set {
                this["Planes_Lifetime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal Setting {
            get {
                return ((decimal)(this["Setting"]));
            }
            set {
                this["Setting"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%DataDir%\\Tmp")]
        public string Tmp_Directory {
            get {
                return ((string)(this["Tmp_Directory"]));
            }
            set {
                this["Tmp_Directory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.airscout.de/UpdateURL.txt")]
        public string Database_UpdateURL {
            get {
                return ((string)(this["Database_UpdateURL"]));
            }
            set {
                this["Database_UpdateURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public decimal Database_UpdateCycle {
            get {
                return ((decimal)(this["Database_UpdateCycle"]));
            }
            set {
                this["Database_UpdateCycle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::AirScout.PlaneFeeds.PlanefeedSettings_WF WebFeed2_Settings {
            get {
                return ((global::AirScout.PlaneFeeds.PlanefeedSettings_WF)(this["WebFeed2_Settings"]));
            }
            set {
                this["WebFeed2_Settings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::AirScout.PlaneFeeds.PlanefeedSettings_WF WebFeed3_Settings {
            get {
                return ((global::AirScout.PlaneFeeds.PlanefeedSettings_WF)(this["WebFeed3_Settings"]));
            }
            set {
                this["WebFeed3_Settings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[none]")]
        public string Planes_WebFeed1 {
            get {
                return ((string)(this["Planes_WebFeed1"]));
            }
            set {
                this["Planes_WebFeed1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[none]")]
        public string Planes_WebFeed2 {
            get {
                return ((string)(this["Planes_WebFeed2"]));
            }
            set {
                this["Planes_WebFeed2"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[none]")]
        public string Planes_WebFeed3 {
            get {
                return ((string)(this["Planes_WebFeed3"]));
            }
            set {
                this["Planes_WebFeed3"] = value;
            }
        }
    }
}