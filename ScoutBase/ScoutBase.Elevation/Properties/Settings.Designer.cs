﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScoutBase.Elevation.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Database_InMemory {
            get {
                return ((bool)(this["Database_InMemory"]));
            }
            set {
                this["Database_InMemory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("GLOBE")]
        public string Elevation_GLOBE_DataPath {
            get {
                return ((string)(this["Elevation_GLOBE_DataPath"]));
            }
            set {
                this["Elevation_GLOBE_DataPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.airscout.eu/downloads/ScoutBase/1/ElevationData/GLOBE")]
        public string Elevation_GLOBE_UpdateURL {
            get {
                return ((string)(this["Elevation_GLOBE_UpdateURL"]));
            }
            set {
                this["Elevation_GLOBE_UpdateURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("globe.json")]
        public string Elevation_GLOBE_JSONFile {
            get {
                return ((string)(this["Elevation_GLOBE_JSONFile"]));
            }
            set {
                this["Elevation_GLOBE_JSONFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SRTM3")]
        public string Elevation_SRTM3_DataPath {
            get {
                return ((string)(this["Elevation_SRTM3_DataPath"]));
            }
            set {
                this["Elevation_SRTM3_DataPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.airscout.eu/downloads/ScoutBase/1/ElevationData/SRTM3")]
        public string Elevation_SRTM3_UpdateURL {
            get {
                return ((string)(this["Elevation_SRTM3_UpdateURL"]));
            }
            set {
                this["Elevation_SRTM3_UpdateURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("srtm3.json")]
        public string Elevation_SRTM3_JSONFile {
            get {
                return ((string)(this["Elevation_SRTM3_JSONFile"]));
            }
            set {
                this["Elevation_SRTM3_JSONFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SRTM1")]
        public string Elevation_SRTM1_DataPath {
            get {
                return ((string)(this["Elevation_SRTM1_DataPath"]));
            }
            set {
                this["Elevation_SRTM1_DataPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.airscout.eu/downloads/ScoutBase/1/ElevationData/SRTM1")]
        public string Elevation_SRTM1_UpdateURL {
            get {
                return ((string)(this["Elevation_SRTM1_UpdateURL"]));
            }
            set {
                this["Elevation_SRTM1_UpdateURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("srtm1.json")]
        public string Elevation_SRTM1_JSONFile {
            get {
                return ((string)(this["Elevation_SRTM1_JSONFile"]));
            }
            set {
                this["Elevation_SRTM1_JSONFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int Datatbase_BackgroundUpdate_Period {
            get {
                return ((int)(this["Datatbase_BackgroundUpdate_Period"]));
            }
            set {
                this["Datatbase_BackgroundUpdate_Period"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int Database_BackgroundUpdate_ThreadWait {
            get {
                return ((int)(this["Database_BackgroundUpdate_ThreadWait"]));
            }
            set {
                this["Database_BackgroundUpdate_ThreadWait"] = value;
            }
        }
    }
}
