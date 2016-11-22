using VolumeWatcher.Enumrate;

namespace VolumeWatcher.Properties {
    
    
    // このクラスでは設定クラスでの特定のイベントを処理することができます:
    //  SettingChanging イベントは、設定値が変更される前に発生します。
    //  PropertyChanged イベントは、設定値が変更された後に発生します。
    //  SettingsLoaded イベントは、設定値が読み込まれた後に発生します。
    //  SettingsSaving イベントは、設定値が保存される前に発生します。
    internal sealed partial class Settings {

        public Settings() {
            // // 設定の保存と変更のイベント ハンドラーを追加するには、以下の行のコメントを解除します:
            //
            this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // SettingChangingEvent イベントを処理するコードをここに追加してください。
            this.Save();
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // SettingsSaving イベントを処理するコードをここに追加してください。
        }

        //[global::System.Configuration.ApplicationScopedSetting()] // UserScopedSettingAttribute()
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        public EWindowPosition window_position2
        {
            get
            {
                return (EWindowPosition)(this["window_position"]);
            }
            set
            {
                this["window_position"] = (int)value;
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        public System.Version Version
        {
            get
            {
                return new System.Version((string)this["AssemblyVersion"]);
            }
            set
            {
                this["AssemblyVersion"] = value.ToString();
            }
        }

        /*
        public void SaveVolumeWindowPosition(int index = 0)
        {
            const string key = "window_potision";
            //　Open　App.Config　of　executable
            System.Configuration.Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, index.ToString());
            config.Save(ConfigurationSaveMode.Modified);
            //　Force　a　reload　of　a　changed　section.
            ConfigurationManager.RefreshSection("appSettings");
        }
        */
    }
}
