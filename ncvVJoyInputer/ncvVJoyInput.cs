using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ncvVJoyInputer
{
    public class ncvVJoyInput : Plugin.IPlugin
    {
        private Plugin.IPluginHost host = null;
        private ncvVJoyInputForm form = null;

        #region IPlugin メンバ

        /// <summary>
        /// プラグインのホスト
        /// </summary>
        public Plugin.IPluginHost Host
        {
            get
            {
                return this.host;
            }
            set
            {
                this.host = value;
                this.form = new ncvVJoyInputForm(this.host);
            }
        }

        /// <summary>
        /// プラグインの名前
        /// </summary>
        public string Name
        {
            get
            {
                return "ncvVJoyInputer";
            }
        }

        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        public string Version
        {
            get
            {
                return "1.0.0.0";
            }
        }

        /// <summary>
        /// プラグインの説明
        /// </summary>
        public string Description
        {
            get
            {
                return "プラグインの雛形です。";
            }
        }

        /// <summary>
        /// アプリケーション起動時に自動実行するかどうか
        /// </summary>
        public bool IsAutoRun
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// プラグインを実行する
        /// </summary>
        public void Run()
        {
            if (!this.form.Visible)
            {
                this.form.Show((System.Windows.Forms.IWin32Window)this.Host.MainForm.Owner);
            }
            else if (this.form.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                this.form.WindowState = System.Windows.Forms.FormWindowState.Normal;
            }
        }

        /// <summary>
        /// NiconamaCommentViwer起動時に自動実行する
        /// </summary>
        public void AutoRun()
        {
            this.Host.BroadcastConnected += new Plugin.BroadcastConnectedEventHandler(this.form.Host_BroadcastConnected);
            this.Host.BroadcastDisConnected += new Plugin.BroadcastDisConnectedEventHandler(this.form.Host_BroadcastDisConnected);
        }

        #endregion

    }
}

