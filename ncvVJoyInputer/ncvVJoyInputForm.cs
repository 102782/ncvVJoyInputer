using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using JoyInputer;
using System.Reflection;

namespace ncvVJoyInputer
{
    public partial class ncvVJoyInputForm : Form
    {
        private Plugin.IPluginHost host;
        private string installDirectry;
        private SimpleLogger logger;
        private VJoyInputController vjoy;
        private VJoyInputerConfig config;
        private bool enableVJoy;
        private int targetLevel;
        private int targetIndex;
        private string targetLabel;

        private readonly string[] AXIS_LABELS = new string[] 
        { 
            "左スティック上", "左スティック右", "左スティック下", "左スティック左", 
            "右スティック上", "右スティック右", "右スティック下", "右スティック左" 
        };
        private readonly string[] POV_LABELS = new string[] 
        { 
            "十字キー上", "十字キー右", "十字キー下", "十字キー左"
        };


        public ncvVJoyInputForm(Plugin.IPluginHost host) 
        {
            InitializeComponent();
            this.host = host;
            initialize();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void ncvVJoyInputForm_Load(object sender, EventArgs e) 
        {
            formShowLoad();
        }

        private void ncvVJoyInputForm_FormClosing(object sender, FormClosingEventArgs e) 
        {
            if(e.CloseReason == CloseReason.UserClosing) 
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        /// <summary>
        /// フォーム作成時の処理（NiconamaCommentViwer起動直後）
        /// </summary>
        private void initialize() 
        {
            this.installDirectry = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.logger = new SimpleLogger();
            this.vjoy = new VJoyInputController(this.logger);
            this.config = new VJoyInputerConfig(this.host.DirectoryPathAppSetting);
            this.config.Load();
            this.enableVJoy = false;

            this.targetLevel = 0;
            this.targetIndex = 0;
            this.targetLabel = "";

            SetButtonText();
            if (!this.host.IsConnected)
            {
                this.button1.Enabled = false;
            }

            this.treeView1.Nodes.Add(new TreeNode("ボタン設定", this.config.current.buttons.Select((c, i) => { return new TreeNode(string.Format("ボタン{0}", i + 1)); }).ToArray()));
            this.treeView1.Nodes.Add(new TreeNode("スティック設定", this.config.current.axis.Select((c, i) => { return new TreeNode(this.AXIS_LABELS[i]); }).ToArray()));
            this.treeView1.Nodes.Add(new TreeNode("十字キー設定", this.config.current.pov.Select((c, i) => { return new TreeNode(this.POV_LABELS[i]); }).ToArray()));

            this.toolTip2.SetToolTip(this.button4, "デフォルト設定を読み込む");
            this.toolTip3.SetToolTip(this.button3, "設定を反映させて保存する");
        }

        /// <summary>
        /// フォーム表示時の処理
        /// </summary>
        private void formShowLoad() 
        {
            this.config.Load();
            this.numericUpDown1.Value = this.config.temporary.span;
            if (!this.host.IsConnected)
            {
                this.button1.Enabled = false;
            }
            else
            {
                this.button1.Enabled = true;
            }

            SetButtonText();
        }

        /// <summary>
        /// 接続時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Host_BroadcastConnected(object sender, EventArgs e) 
        {
            this.host.ReceivedComment += new Plugin.ReceivedCommentEventHandler(this.Host_ReceivedComment);
            this.logger.Add(SimpleLogger.TAG.INFO, "Connected");

            this.vjoy.Initialize(this.installDirectry, this.config.current.span, this.config.current.buttons, this.config.current.axis, this.config.current.pov);
            this.enableVJoy = this.vjoy.isInitializedVJoy;

            SetButtonText();
            this.button1.Enabled = true;
        }

        /// <summary>
        /// 切断時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Host_BroadcastDisConnected(object sender, EventArgs e) 
        {
            if (this.enableVJoy)
            {
                this.vjoy.Relinquish();
                this.enableVJoy = this.vjoy.isInitializedVJoy;
            }

            SetButtonText();

            this.host.ReceivedComment -= new Plugin.ReceivedCommentEventHandler(this.Host_ReceivedComment);
            this.logger.Add(SimpleLogger.TAG.INFO, "DisConnected");

            this.button1.Enabled = false;

            //this.logger.Save(this.installDirectry, "ncvVJoyInputer_Log.txt");
        }

        /// <summary>
        /// コメント受信時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="e.CommentDataList">コメントデータリスト</param>
        public void Host_ReceivedComment(object sender, Plugin.ReceivedCommentEventArgs e) 
        {
            if (this.enableVJoy)
            {
                this.logger.Add(SimpleLogger.TAG.INFO, "Recived Count: " + e.CommentDataList.Count);
                if (e.CommentDataList.Any())
                {
                    e.CommentDataList.Select((c) =>
                    {
                        this.logger.Add(SimpleLogger.TAG.INFO, "Recived: " + c.Comment);
                        this.vjoy.SetSource(c.Comment);
                        return c;
                    }).ToArray();
                }
            }
        }

        /// <summary>
        /// 有効/無効切り替えボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.host.IsConnected) return;

            this.button1.Enabled = false;

            if (this.enableVJoy)
            {
                this.enableVJoy = false;
                this.vjoy.Relinquish();
            }
            else
            {
                this.vjoy.Initialize(this.installDirectry, this.config.current.span, this.config.current.buttons, this.config.current.axis, this.config.current.pov);
                this.enableVJoy = this.vjoy.isInitializedVJoy;
            }

            SetButtonText();
            this.button1.Enabled = true;
        }

        /// <summary>
        /// 有効/無効切り替えボタンの表示テキスト設定
        /// </summary>
        private void SetButtonText()
        {
            if (this.enableVJoy)
            {
                this.button1.Text = "無効化";
                this.toolTip1.SetToolTip(this.button1, "vJoyへの送信を無効に切り替える");
            }
            else
            {
                this.button1.Text = "有効化";
                this.toolTip1.SetToolTip(this.button1, "vJoyへの送信を有効に切り替える");
            }
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 設定適用ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.config.Save();

            if (this.host.IsConnected && this.enableVJoy)
            {
                this.button1.Enabled = false;
                this.enableVJoy = false;
                this.vjoy.Relinquish();
                this.vjoy.Initialize(this.installDirectry, this.config.current.span, this.config.current.buttons, this.config.current.axis, this.config.current.pov);
                this.enableVJoy = this.vjoy.isInitializedVJoy;
                SetButtonText();
                this.button1.Enabled = true;
            }

            this.Close();
        }

        /// <summary>
        /// 正規表現設定ツリー項目選択時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            TreeViewHitTestInfo ht = treeView1.HitTest(e.Location);

            if (ht.Location == TreeViewHitTestLocations.Label)
            {
                this.targetLabel = "";
                this.targetLevel = ht.Node.Level;
                this.targetIndex = ht.Node.Index;
                if (this.targetLevel == 1)
                {
                    this.targetLabel = ht.Node.Parent.Text;
                }

                SetTargetRegex(this.targetLabel, this.targetIndex);
            }
        }

        /// <summary>
        /// 正規表現設定の反映
        /// </summary>
        /// <param name="label"></param>
        /// <param name="index"></param>
        private void SetTargetRegex(string label, int index)
        {
            switch (label)
            {
                case ("ボタン設定"):
                    this.label2.Text = string.Format("ボタン{0}", index + 1);
                    this.textBox1.Text = this.config.temporary.buttons[index];
                    this.label3.Text = string.Format("コメントが指定した正規表現にマッチした時、\nボタン{0}の入力を送信します", this.targetIndex + 1);
                    break;
                case ("スティック設定"):
                    this.label2.Text = this.AXIS_LABELS[index];
                    this.textBox1.Text = this.config.temporary.axis[index];
                    this.label3.Text = string.Format("コメントが指定した正規表現にマッチした時、\n{0}の入力を送信します", this.AXIS_LABELS[this.targetIndex]);
                    break;
                case ("十字キー設定"):
                    this.label2.Text = this.POV_LABELS[index];
                    this.textBox1.Text = this.config.temporary.pov[index];
                    this.label3.Text = string.Format("コメントが指定した正規表現にマッチした時、\n{0}の入力を送信します", this.POV_LABELS[this.targetIndex]);
                    break;
                default:
                    this.label2.Text = "-";
                    this.textBox1.Text = "";
                    this.label3.Text = "-";
                    break;
            }
        }

        /// <summary>
        /// 正規表現設定変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.targetLevel == 0) return;

            switch (this.targetLabel)
            {
                case ("ボタン設定"):
                    this.config.temporary.buttons[this.targetIndex] = this.textBox1.Text;
                    break;
                case ("スティック設定"):
                    this.config.temporary.axis[this.targetIndex] = this.textBox1.Text;
                    break;
                case ("十字キー設定"):
                    this.config.temporary.pov[this.targetIndex] = this.textBox1.Text;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 押し込み時間設定変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.config.temporary.span = (int)this.numericUpDown1.Value;
        }

        /// <summary>
        /// デフォルト設定読み込みボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.label2.Text = "-";
            this.textBox1.Text = "";
            this.label3.Text = "-";
            this.config.SetDefaultToTemporary();

            if (this.targetLevel == 1)
            {
                SetTargetRegex(this.targetLabel, this.targetIndex);
            }
        }
    }
}

