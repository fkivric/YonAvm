using SIPVoipSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Alerter;
using YonAvm.Class;

namespace YonAvm
{

    using ConnectionInfo = KeyValuePair<int, string>;
    using ConnectionsTbl = Dictionary<int, string>;

    public partial class PhoneForm : DevExpress.XtraEditors.XtraForm
    {
        public static string number;
        public class Call_Queru
        {
            public int CURID { get; set; }
            public string Extension { get; set; }
            public string Destination { get; set; }
            public string Status { get; set; }
        }

        public class ConnListBoxItem
        {
            public int handle;
            public string connection;

            public ConnListBoxItem(int _handle, string _connection)
            {
                handle = _handle;
                connection = _connection;
            }

            public override string ToString()
            {
                return this.connection;
            }
        }
        public class LineInfo
        {
            public LineInfo(int id)
            {
                m_id = id;
                m_conn = new ConnectionsTbl();
                m_bCalling = false;
                m_bCallEstablished = false;
                m_bCallHeld = false;
                m_bCallPlayStarted = false;
                m_usrInputStr = "";
            }

            public ConnectionsTbl m_conn;
            public int m_id;
            public int m_lastConnId;
            public bool m_bCalling;
            public bool m_bCallEstablished;
            public bool m_bCallHeld;
            public bool m_bCallPlayStarted;
            public string m_usrInputStr;
            public System.Windows.Forms.Timer m_callDurationTimer;
            public TimeSpan m_callTime;
            public string m_callTimeStr;
        }

        private CAbtoPhone AbtoPhone = new CAbtoPhone();

        public CConfig phoneCfg;

        private string cfgFileName = "phoneCfg.ini";

        public const int LineCount = 6;
        private int m_curLineId = 1;

        ArrayList m_lineConnections = new ArrayList();

        private int m_lineIdWhereRecStarted = 0;

        private bool m_MP3RecordingEnabled = false;
        private bool m_AutoAnswerEnabled = false;
        int Zaman = 0;
        List<Call_Queru> Queue = new List<Call_Queru>();
        private readonly HttpClient httpClient;
        public static async void MobilListAsync()
        {
            try
            {
                var sorgu = new DidEXTLIST
                {
                    apikey = "HRjzrdskUzrksvBMn3ni2GFBVPwFaywH",// Properties.Settings.Default.SIPToken,
                    centraluuid = Properties.Settings.Default.DidCentraluuid,
                    centralcode = Properties.Settings.Default.DidCentralcode,
                    module = "ENTEGREF",
                    function = "EXTLIST"
                };
                var json = JsonConvert.SerializeObject(sorgu);
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // İstek oluştur
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Properties.Settings.Default.DidApiUrl);
                        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                        // İsteği gönder ve yanıtı al
                        HttpResponseMessage response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        // Yanıtı oku
                        string responseContent = await response.Content.ReadAsStringAsync();
                        DidEXTLISTreturn myDeserializedClass = JsonConvert.DeserializeObject<DidEXTLISTreturn>(responseContent);
                        if (myDeserializedClass.result == "SUCCESS")
                        {
                            ConfigForm.HatListesi.Clear();
                            foreach (var item in myDeserializedClass.data)
                            {
                                if (item.santraldahili_durum == "2")
                                {
                                    if (item.santraldahili_numara90e164 == "902129682046")
                                    {
                                        ConfigForm.Domain = item.santraldahili_domain + ":4040";
                                        ConfigForm.User = item.santraldahili_kullaniciadi;
                                        ConfigForm.Pass = item.santraldahili_sifre;
                                        ConfigForm.authId = item.santraldahili_kullaniciadi;
                                        ConfigForm.CallerId = item.santraldahili_numara90e164.ToString();
                                        ConfigForm.UserAgent = item.santraldahili_ekranadi;

                                    }
                                    else
                                    {
                                        Console.WriteLine(item.santraldahili_numara90e164);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Hata durumunda gerekli işlemleri yapabilirsiniz
                        Console.WriteLine("Hata: " + ex.Message);
                    }
                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.Message);
            }
        }


        public PhoneForm(string _number)
        {
            InitializeComponent();
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://entegref.com.tr/");
            number = _number;
            timer1.Enabled = true;
        }
        private LineInfo GetCurLine()
        {
            return (LineInfo)m_lineConnections[m_curLineId - 1];
        }
        private LineInfo GetLine(int lineId)
        {
            return (LineInfo)m_lineConnections[lineId - 1];
        }
        private void DisplayConnectionsAll(LineInfo lnInfo)
        {
            activeConnListbox.Items.Clear();
            foreach (ConnectionInfo it in lnInfo.m_conn) DisplayConnection(it);
        }
        private void DisplayConnection(ConnectionInfo ci)
        {
            int itemIndex = activeConnListbox.Items.Add(new ConnListBoxItem(ci.Key, ci.Value));
            activeConnListbox.SelectedIndex = itemIndex;
        }
        private void RemoveConnection(int connectionId)
        {
            foreach (ConnListBoxItem t in activeConnListbox.Items)
            {
                if (t.handle == connectionId) { activeConnListbox.Items.Remove(t); break; }
            }

            int count = activeConnListbox.Items.Count;
            if (count >= 1) activeConnListbox.SelectedIndex = count - 1;
        }
        private bool GetSelectedConnection(out int connectionId)
        {
            //Check connections count
            connectionId = 0;
            int count = activeConnListbox.Items.Count;
            if (count == 0) return false;

            //Get sel connection
            int selectedIndex = activeConnListbox.SelectedIndex;
            if (selectedIndex == -1) selectedIndex = count - 1;

            //Get conn id
            connectionId = ((ConnListBoxItem)activeConnListbox.Items[selectedIndex]).handle;
            return true;
        }
        private void ChageControlsState(LineInfo li)
        {
            ChageLineCaption(li);

            buttonStartHangupCall.Text = li.m_bCallEstablished || li.m_bCalling ? "Kapat" : "Arama Başlat";

            buttonHoldRetrieve.Visible = li.m_bCallEstablished;
            buttonHoldRetrieve.Text = li.m_bCallHeld ? "Devamet" : "Tut";

            buttonTransfer.Visible = li.m_bCallEstablished;
            buttonJoin.Visible = li.m_bCallEstablished;

            callDurationLabel.Visible = li.m_bCallEstablished;
            callDurationLabel.Text = li.m_callTimeStr;

            AddressBox.Enabled = li.m_bCallEstablished || li.m_bCalling ? false : true;

            buttonPlayStartStop.Text = li.m_bCallPlayStarted ? "Durdur" : "Oynatma Dosyası";
            buttonRecordStartStop.Text = (m_lineIdWhereRecStarted != 0) ? "Kaydı Durdur" : "Kaydı Başlat";

            UInputLabel.Text = li.m_usrInputStr;
        }
        private void ChageLineCaption(LineInfo li)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Line");
            sb.Append(li.m_id);
            if (li.m_bCallEstablished) sb.Insert(0, "[x]");

            getLineButton(li.m_id).Text = sb.ToString();

        }
        private Button getLineButton(int lineId)
        {
            switch (lineId)
            {
                case 1: return buttonLine1;
                case 2: return buttonLine2;
                case 3: return buttonLine3;
                case 4: return buttonLine4;
                case 5: return buttonLine5;
                case 6: return buttonLine6;
            }
            return buttonLine1;
        }
        private void HighlightCurLine(int prevCurLine, int newCurLine)
        {
            Button prevBut = getLineButton(prevCurLine);
            Button newBut = getLineButton(newCurLine);

            prevBut.Font = new Font(prevBut.Font.FontFamily, prevBut.Font.Size, prevBut.Font.Style ^ FontStyle.Bold);
            newBut.Font = new Font(newBut.Font.FontFamily, newBut.Font.Size, newBut.Font.Style | FontStyle.Bold);
        }
        private void displayNotifyMsg(string msg)
        {
            notificationsListBox.Items.Add(msg);
            notificationsListBox.TopIndex = notificationsListBox.Items.Count - 1;//scrol down
        }
        private void PhoneForm_Load(object sender, EventArgs e)
        {
            MobilListAsync();
            lblversion2.Text = "";
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                lblversion2.Text = "Version : " + ad.CurrentVersion.Major + "." + ad.CurrentVersion.Minor + "." + ad.CurrentVersion.Build + "." + ad.CurrentVersion.Revision;
            }
            if (lblversion2.Text == "")
            {
                //lblversion2.Text = Application.ProductVersion;
                lblversion2.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            //Displ initial status
            displayNotifyMsg("Baglanıyor...");

            //Configure phone
            bool bRes = ConfigurePhone();
            if (!bRes)
            {
                PictureErr.Top = 0;
                PictureErr.Left = CallPanel.Left;
                PictureErr.Width = CallPanel.Width;
                PictureErr.Height = CallPanel.Top + CallPanel.Height;
                PictureErr.BorderStyle = 0;
                PictureErr.Visible = true;
                return;
            }


            //Make lines
            for (int i = 1; i <= LineCount; ++i) m_lineConnections.Add(new LineInfo(i));
            buttonLine1.Tag = 1; buttonLine2.Tag = 2; buttonLine3.Tag = 3;
            buttonLine4.Tag = 4; buttonLine5.Tag = 5; buttonLine6.Tag = 6;

            //Configure playback controls
            spkVolume.SetRange(0, 100);
            spkVolume.Value = AbtoPhone.PlaybackVolume;
            spkVolume.TickFrequency = 10;

            spkVolumeBar.Minimum = 0;
            spkVolumeBar.Maximum = Int16.MaxValue;

            //Configure record controls
            micVolume.SetRange(0, 100);
            micVolume.Value = AbtoPhone.RecordVolume;
            micVolume.TickFrequency = 10;

            micVolumeBar.Minimum = 0;
            micVolumeBar.Maximum = Int16.MaxValue;


            //Mute Speaker/Microphone
            muteSpeakerFlag.Checked = true;
            muteMicrophoneFlag.Checked = true;

            //Get state
            m_MP3RecordingEnabled = AbtoPhone.Config.MP3RecordingEnabled != 0;
            m_AutoAnswerEnabled = AbtoPhone.Config.AutoAnswerEnabled != 0;

            AcceptButton = buttonStartHangupCall;      
            timer1.Start();
        }
        

        protected bool ConfigurePhone()
        {
            //Assign event handlers
            this.AbtoPhone.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(this.AbtoPhone_OnInitialized);
            this.AbtoPhone.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(this.AbtoPhone_OnLineSwiched);
            this.AbtoPhone.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(this.AbtoPhone_OnEstablishedCall);
            this.AbtoPhone.OnIncomingCall += new _IAbtoPhoneEvents_OnIncomingCallEventHandler(this.AbtoPhone_OnIncomingCall);
            this.AbtoPhone.OnIncomingCall2 += new _IAbtoPhoneEvents_OnIncomingCall2EventHandler(this.AbtoPhone_OnIncomingCall2);
            this.AbtoPhone.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(this.AbtoPhone_OnClearedCall);
            this.AbtoPhone.OnVolumeUpdated += new _IAbtoPhoneEvents_OnVolumeUpdatedEventHandler(this.AbtoPhone_OnVolumeUpdated);
            this.AbtoPhone.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(this.AbtoPhone_OnRegistered);
            this.AbtoPhone.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(this.AbtoPhone_OnPlayFinished);
            this.AbtoPhone.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(this.AbtoPhone_OnEstablishedConnection);
            this.AbtoPhone.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(this.AbtoPhone_OnClearedConnection);
            this.AbtoPhone.OnToneReceived += new _IAbtoPhoneEvents_OnToneReceivedEventHandler(this.AbtoPhone_OnToneReceived);
            this.AbtoPhone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(this.AbtoPhone_OnTextMessageReceived);
            this.AbtoPhone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(AbtoPhone_OnTextMessageSentStatus);
            this.AbtoPhone.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(this.AbtoPhone_OnPhoneNotify);
            this.AbtoPhone.OnRemoteAlerting2 += new _IAbtoPhoneEvents_OnRemoteAlerting2EventHandler(AbtoPhone_OnRemoteAlerting2);
            this.AbtoPhone.OnSubscribeStatus += new _IAbtoPhoneEvents_OnSubscribeStatusEventHandler(AbtoPhone_OnSubscribeStatus);
            this.AbtoPhone.OnSubscriptionNotify += new _IAbtoPhoneEvents_OnSubscriptionNotifyEventHandler(AbtoPhone_OnSubscriptionNotify);
            //this.AbtoPhone.OnDetectedAnswerTime+= new _IAbtoPhoneEvents_OnDetectedAnswerTimeEventHandler(this.AbtoPhone_OnDetectedAnswerTime);
            //this.AbtoPhone.OnRecordFinished +=new _IAbtoPhoneEvents_OnRecordFinishedEventHandler(AbtoPhone_OnRecordFinished);
            //this.AbtoPhone.OnReceivedRequestInfo += new _IAbtoPhoneEvents_OnReceivedRequestInfoEventHandler(AbtoPhone_OnReceivedRequestInfo);
            AbtoPhone.OnToneDetected += new _IAbtoPhoneEvents_OnToneDetectedEventHandler(AbtoPhone_OnToneDetected);
            //this.AbtoPhone.OnPlayFinished2 += new _IAbtoPhoneEvents_OnPlayFinished2EventHandler(AbtoPhone_OnPlayFinished2);
            //AbtoPhone.OnHoldCall2 += new _IAbtoPhoneEvents_OnHoldCall2EventHandler(AbtoPhone_OnHoldCall2);
            //AbtoPhone.OnReceivedRequestUpdate += new _IAbtoPhoneEvents_OnReceivedRequestUpdateEventHandler(AbtoPhone_OnReceivedRequestUpdate);

            //Get config
            CConfig phoneCfg = AbtoPhone.Config;

            //Load config values from file
            phoneCfg.Load(cfgFileName);

            //phoneCfg.ExSipAccount_Add("192.168.0.150", "216", "216", "", "", 300, 1, 0);
            //phoneCfg.ExSipAccount_Add("192.168.0.150", "217", "217", "", "", 300, 1, 0);


                //Manual set needed values
                //phoneCfg.ListenPort = 5060;
                //phoneCfg.StunServer = "stun.l.google.com:19302";			


                //phoneCfg.RegDomain = "...";//your domain
                //phoneCfg.RegUser = "..";//your user name
                //phoneCfg.RegPass = "..";//your password
                //phoneCfg.RegExpire = 300;
                //phoneCfg.TlsVerifyServerEnabled = false;//Disable server certificate verification (always trust)
                //phoneCfg.SignallingTransport = (int)TransportType.eTransportTLSv1;//Set TLS transport
                //phoneCfg.EncryptedCallEnabled = 1;//Enable SRTP

                //Specify License key
                //phoneCfg.LicenseUserId = "{Trial1f95-8887-147A-4C015A39...}";
                //phoneCfg.LicenseKey = "{w6uQzP8gZos82bmoIQ4zxMEt+ecv4bj+...}";		

                //Log level
                //phoneCfg.LogLevel= (LogLevelType)11;// LogLevelType.eLogDebug;//eLogError//(LogLevelType)11;
                //phoneCfg.LogPath = "C:\\temp\\logs";

                //Set AdditionalDnsServer as google dns
                //phoneCfg.AdditionalDnsServer = "8.8.8.8";

                //Specify network interface
                //phoneCfg.ActiveNetworkInterface = "...";
                //phoneCfg.TonesTypesToDetect = (int)ToneType.eToneMF + (int)ToneType.eToneDtmf;
                //phoneCfg.SignallingTransport = (int)TransportType.eTransportTLSv1;		
                //phoneCfg.SignallingTransport = (int)TransportType.eTransportUDP;
                //phoneCfg.SignallingTransport = (int)TransportType.eTransportTCP;
                //phoneCfg.TlsVerifyServerEnabled = false;
                //phoneCfg.EncryptedCallEnabled = 1;


                //phoneCfg.RegDomain = "x.x.x.x:5061";//typical port number for TLS transport on server side is 5061
                //phoneCfg.TonesTypesToDetect = (int)ToneType.eToneDtmf;


                //Set video windows
                //phoneCfg.RemoteVideoWindow = pictureReceivedVideo.Handle.ToInt32();
                //phoneCfg.LocalVideoWindow = pictureLocalVideo.Handle.ToInt32();
                //phoneCfg.RemoteVideoWindow_Add(pictureReceivedVideo.Handle.ToInt32());

                //phoneCfg->put_AudioQosDscpVal(40);
                //phoneCfg->put_VideoQosDscpVal(50);
                //phoneCfg.TonesTypesToDetect = (int)ToneType.eToneDtmf;
                //phoneCfg.DtmfAsSipInfoEnabled = 1;

                //... other initializations	
                //phoneCfg.MixerFilePlayerEnabled = 0;//allows play few files simultaneously


                try
            {
                //Apply modified config
                AbtoPhone.ApplyConfig();


                AbtoPhone.Initialize();
            }
            catch (Exception e)
            {
                displayNotifyMsg(e.Message);
                return false;
            }

            return true;
        }

        private void AbtoPhone_OnToneDetected(ToneType tType, string ToneStr, int ConnectionId, int LineId)
        {
            throw new NotImplementedException();
        }

        int gEstablishedConnectionId = 0;

        private void AbtoPhone_OnIncomingCall2(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            if (gEstablishedConnectionId == 0)
            {
                gEstablishedConnectionId = ConnectionId;
                AbtoPhone.AnswerCallLine(LineId);
            }
            else
            {
                AbtoPhone.RejectCallLine(LineId);
            }
        }
        private void callStartHangupBtn_Click(object sender, EventArgs e)
        {
            LineInfo lnInfo = GetCurLine();

            if (lnInfo.m_bCallEstablished || lnInfo.m_bCalling)
            {
                //HangUp
                int connectionId;
                if (GetSelectedConnection(out connectionId) == true) AbtoPhone.HangUp(connectionId);
                else AbtoPhone.HangUpLastCall();                
            }
            else
            {
                //Get addr
                string address = AddressBox.Text;
                if (address.Length == 0) return;

                //Append addr to combo
                int idx = AddressBox.FindString(address, -1);
                if (idx == -1) AddressBox.Items.Add(address);

                //Set status
                displayNotifyMsg("Aranıyor......");

                //Set flag, Cange controls state
                lnInfo.m_bCalling = true;
                ChageControlsState(lnInfo);

                //Start call without video
                //AbtoPhone.Config.VideoCallEnabled = 0;
                //AbtoPhone.ApplyConfig();

                //Start call and get assigned connectionId
                Record(address);
                int connId = AbtoPhone.StartCall2(address);
                //buttonStartHangupCall2.Enabled = false;
                //buttonStartHangupCall.Enabled = true;
            }

        }//callStartHangupButton_Click
        private void holdRetreiveBtn_Click(object sender, EventArgs e)
        {
            LineInfo lnInfo = GetCurLine();

            //Hold/retrieve
            AbtoPhone.HoldRetrieveCall(lnInfo.m_id);

            //Change button caption
            buttonHoldRetrieve.Text = lnInfo.m_bCallHeld ? "Tut" : "Geri Al";

            //Change flag
            lnInfo.m_bCallHeld = !lnInfo.m_bCallHeld;
        }
        private void playStartStopButton_Click(object sender, EventArgs e)
        {
            stopStartPlaying(false, m_curLineId);
        }
        private void recordStartStopBtn_Click(object sender, EventArgs e)
        {
            LineInfo lnInfo = GetCurLine();
            if (m_lineIdWhereRecStarted != 0)
            {
                AbtoPhone.StopRecording();
                buttonRecordStartStop.Text = "Kaydet";
                displayNotifyMsg("Kayıt Durduruldu");
                m_lineIdWhereRecStarted = 0;
            }
            else
            {
                SaveFileDialog fileDlg = new SaveFileDialog();
                fileDlg.Filter = (m_MP3RecordingEnabled == true) ? "Sound Files (*.mp3)|*.mp3" : "Sound Files (*.wav)|*.wav";
                fileDlg.OverwritePrompt = true;
                if (fileDlg.ShowDialog(this) != DialogResult.OK) return;

                AbtoPhone.StartRecording(fileDlg.FileName);
                buttonRecordStartStop.Text = "Kayıt Durdur";
                displayNotifyMsg("Kayıt file: " + fileDlg.FileName);

                m_lineIdWhereRecStarted = m_curLineId;
            }
        }
        private void transferBtn_Click(object sender, EventArgs e)
        {
            //Get transfer addr
            TransferAddrForm dlg = new TransferAddrForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            //Transfer call
            AbtoPhone.TransferCall(dlg.textBoxAddr.Text);
        }
        private void joinBtn_Click(object sender, EventArgs e)
        {
            //Get line Id
            JoinForm dlg = new JoinForm();
            dlg.m_curLineId = m_curLineId;
            dlg.m_selLineId = 0;
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            //Check same line
            if (dlg.m_selLineId == m_curLineId) return;

            //Check selected line
            LineInfo selLineInfo = GetLine(dlg.m_selLineId);
            LineInfo curLineInfo = GetCurLine();
            if (!selLineInfo.m_bCallEstablished) return;

            //Append
            foreach (ConnectionInfo it in selLineInfo.m_conn) curLineInfo.m_conn.Add(it.Key, it.Value);
            selLineInfo.m_conn.Clear();

            //Displ
            DisplayConnectionsAll(curLineInfo);

            //Join
            AbtoPhone.JoinToCurrentCall(dlg.m_selLineId);
        }
        private void lineBtn_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int lineId = (int)b.Tag;

            AbtoPhone.SetCurrentLine(lineId);
        }
        private void DTFM_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            bool bDtmfSent = false;

            while (!bDtmfSent)
            {
                try
                {
                    //AbtoPhone.SendTone(b.Text);
                    AbtoPhone.SendToneEx(Convert.ToInt32(b.Tag), 200, 1, 1, 0);
                    bDtmfSent = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }

        }
        private void MicVolume_Scroll(object sender, EventArgs e)
        {
            AbtoPhone.RecordVolume = micVolume.Value;
        }
        private void SpkVolume_Scroll(object sender, EventArgs e)
        {
            AbtoPhone.PlaybackVolume = spkVolume.Value;
        }
        private void muteSoundCB_CheckedChanged(object sender, EventArgs e)
        {
            AbtoPhone.PlaybackMuted = muteSpeakerFlag.Checked ? 0 : 1;
        }
        private void muteMicCB_CheckedChanged(object sender, EventArgs e)
        {
            AbtoPhone.RecordMuted = muteMicrophoneFlag.Checked ? 0 : 1;
        }
        private void activateSDK_Click(object sender, EventArgs e)
        {
            //Show form
            ActivationForm dlg = new ActivationForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            //Store cfg values
            CConfig phoneCfg = AbtoPhone.Config;
            phoneCfg.LicenseUserId = dlg.m_licUserId;
            phoneCfg.LicenseKey = dlg.m_licKey;
            phoneCfg.Store(cfgFileName);

            //Message box
            MessageBox.Show("Etkinleştirmeyi sonlandırmak için uygulamayı yeniden başlatın.");
        }
        private void stopStartPlaying(bool bCalledByPlayFinishedEvent, int lineId)
        {
            LineInfo lnInfo = GetLine(lineId);

            //If called by PlayFinished Event and playing has already stopped - do nothing
            if (bCalledByPlayFinishedEvent && !lnInfo.m_bCallPlayStarted) return;

            if (lnInfo.m_bCallPlayStarted)
            {
                AbtoPhone.StopPlayback();
                if (lineId == m_curLineId) buttonPlayStartStop.Text = "Oynat";
            }
            else
            {
                OpenFileDialog fileDlg = new OpenFileDialog();
                fileDlg.Multiselect = false;
                fileDlg.Filter = "Sound Files (*.wav)|*.wav|Sound Files (*.mp3)|*.mp3";
                if (fileDlg.ShowDialog(this) != DialogResult.OK) return;

                int succeded = AbtoPhone.PlayFile(fileDlg.FileName);
                if (succeded == 0) return;

                displayNotifyMsg("Playing file: " + fileDlg.FileName);
                buttonPlayStartStop.Text = "Oynatmayı Durdur";
            }

            lnInfo.m_bCallPlayStarted = !lnInfo.m_bCallPlayStarted;

        }//stopStartPlaying
        delegate void ActivateSDK_Delegate(object sender, EventArgs e);
        private void AbtoPhone_OnInitialized(string Msg)
        {
            displayNotifyMsg(Msg);

            if (Msg.Contains("FAILED"))
                BeginInvoke(new ActivateSDK_Delegate(activateSDK_Click), this, null);
        }
        private void AbtoPhone_OnLineSwiched(int lineId)
        {
            //Display line as pressed button
            HighlightCurLine(m_curLineId, lineId);

            //Remember
            m_curLineId = lineId;

            //Display connections of cur line
            LineInfo lnInfo = GetCurLine();
            DisplayConnectionsAll(lnInfo);

            //Show/Hide call controls
            ChageControlsState(lnInfo);
        }
        private void AbtoPhone_OnEstablishedCall(string adress, int lineId)
        {
            //Update line state
            LineInfo lnInfo = GetLine(lineId);
            lnInfo.m_usrInputStr = "";
            lnInfo.m_bCallEstablished = true;
            lnInfo.m_bCalling = false;

            //Start call duration timer
            startCallDurationTimer(lnInfo);

            //Update controls (only when it's cur line event)
            if (lineId == m_curLineId)
            {
                //Display status
                displayNotifyMsg(adress);

                //Cange controls state
                ChageControlsState(lnInfo);
            }
            else
            {
                ChageLineCaption(lnInfo);
            }
            //FtpTransfer(Fullpath);
        }
        private void AbtoPhone_OnIncomingCall(string adress, int lineId)
        {
            if (m_AutoAnswerEnabled == true) return;
            string _String = adress;

            string[] _Split = _String.Split('"');
            var s2 = _Split[1].Substring(1, 11);


            //AbtoPhone.Config.VideoCallEnabled = 0;//Answer audio or video
            //AbtoPhone.ApplyConfig();
            IncomingForm dlg = new IncomingForm();
            dlg.textBoxCaller.Text = s2;
            dlg.textBoxLine.Text = "Line" + lineId.ToString();

            Message msg = new Message(s2);
            alertControl1.Show(dlg, msg.Caption, msg.Text, "", msg.Image, msg);

            if (dlg.ShowDialog(this) == DialogResult.Yes)
            {
                AbtoPhone.AnswerCallLine(lineId);
            }
            else AbtoPhone.RejectCallLine(lineId);
            
        }
        public class Message
        {
            public Message(string number)
            {
                this.Caption = "Gelen Arama Var";
                this.Text = number;
                this.Image = YonAvm.Properties.Resources.YON_AVM_400;
            }
            public string Caption { get; set; }
            public string Text { get; set; }
            public Image Image { get; set; }
        }
        private void alertControl1_BeforeFormShow(object sender, DevExpress.XtraBars.Alerter.AlertFormEventArgs e)
        {
            e.AlertForm.OpacityLevel = 1;
        }
        private void alertControl1_FormLoad(object sender, AlertFormLoadEventArgs e)
        {
            var s = e.AlertForm.Size;
            e.AlertForm.Size = new Size(s.Width, s.Height * 2);

            e.AlertForm.LayoutChanged();
        }
        private void AbtoPhone_OnClearedCall(string Msg, int status, int lineId)
        {
            //Update line state
            LineInfo lnInfo = GetLine(lineId);
            lnInfo.m_usrInputStr = "";
            lnInfo.m_bCallEstablished = false;
            lnInfo.m_bCalling = false;
            lnInfo.m_callTimeStr = "";
            if (lnInfo.m_callDurationTimer != null) lnInfo.m_callDurationTimer.Stop();

            if (lineId == m_curLineId)
            {
                //Queue.Where(w => w.Status == "").ToList().ForEach(s => s.Status = status.ToString());
                //Update_Call(Queue.Where(w => w.Status == "").ToString());
                displayNotifyMsg(Msg + ". Durum: " + status.ToString());
                ChageControlsState(lnInfo);
                //foreach (var item in Queue)
                //{
                //    Delete(item.Destination);
                //}

            }
            else
            {
                ChageLineCaption(lnInfo);
            }
        }
        private void AbtoPhone_OnToneReceived(int tone, int connectionId, int lineId)
        {
            LineInfo lnInfo = GetLine(lineId);

            StringBuilder sb = new StringBuilder();
            sb.Append(lnInfo.m_usrInputStr);
            sb.Append((char)tone);
            lnInfo.m_usrInputStr = sb.ToString();

            if (lineId == m_curLineId) UInputLabel.Text = lnInfo.m_usrInputStr;
        }
        private void AbtoPhone_OnVolumeUpdated(int IsMicrophone, int level)
        {
            if (IsMicrophone == 0) spkVolumeBar.Value = level;
            else micVolumeBar.Value = level;
        }
        private void AbtoPhone_OnRegistered(string Msg)
        {
            displayNotifyMsg(Msg);
        }
        private void AbtoPhone_OnPlayFinished(string Msg)
        {
            string playStr = "Çalma Tamamlandı: ";

            int idx = Msg.IndexOf(playStr);
            if (idx == 0)
            {
                string lineStr = Msg.Substring(playStr.Length);
                stopStartPlaying(true, int.Parse(lineStr));
            }

            displayNotifyMsg(Msg);
        }
        private void AbtoPhone_OnEstablishedConnection(string addrFrom, string addrTo, int connectionId, int lineId)
        {
            LineInfo lnInfo = GetLine(lineId);
            string addr = lnInfo.m_bCalling ? addrTo : addrFrom;

            lnInfo.m_conn.Add(connectionId, addr);
            lnInfo.m_lastConnId = connectionId;

            if (lineId == m_curLineId) DisplayConnection(new ConnectionInfo(connectionId, addr));
        }
        private void AbtoPhone_OnClearedConnection(int connectionId, int lineId)
        {
            LineInfo lnInfo = GetLine(lineId);
            lnInfo.m_conn.Remove(connectionId);
            lnInfo.m_lastConnId = 0;
            //if (gEstablishedConnectionId == connectionId) gEstablishedConnectionId = 0;
            if (lineId == m_curLineId) RemoveConnection(connectionId);
        }
        public void cagrikapat(int connectionId, int lineId)
        {
            AbtoPhone_OnClearedConnection(connectionId, lineId);
        }
        private void AbtoPhone_OnTextMessageReceived(string from, string message)
        {
            displayNotifyMsg("'" + message + "' alınan: " + from);
        }
        private void AbtoPhone_OnTextMessageSentStatus(string address, string reason, int bSuccess)
        {
            if (bSuccess != 0) displayNotifyMsg("Mesaj başarıyla gönderildi: " + address + " Sebep: " + reason);
            else displayNotifyMsg("Mesaj gönderilemedi: " + address + " Sebep: " + reason);
        }
        private void AbtoPhone_OnPhoneNotify(string message)
        {
            displayNotifyMsg(message);

            //"Redirect Success. Connection: x";
            //"Redirect Failure. Connection: x Status y";
            Match match = Regex.Match(message, @"Redirect.*Connection: \d+");
            if (match.Success)
            {
                string connIdStr = Regex.Match(match.Value, @"\d+").Value;
                AbtoPhone.HangUp(int.Parse(connIdStr));
            }
        }
        private void AbtoPhone_OnRemoteAlerting2(int ConnectionId, int lineid, int responseCode, string reasonMsg)
        {
            string str = "Remote alerting: " + responseCode.ToString() + " " + reasonMsg;
            displayNotifyMsg(str);
        }

        //void AbtoPhone_OnToneDetected(ToneType tType, string ToneStr, int ConnectionId, int LineId)
        //{
        //if (tType == ToneType.eToneMF)
        //{
        //	if(ToneStr=="450");
        //	if(ToneStr=="480");
        //	if(ToneStr=="620");
        //	if(ToneStr=="AnsweringMachine T-Mobile 1250");
        //	if(ToneStr=="AnsweringMachine Verizon 1500");
        //	if(ToneStr=="AnsweringMachine Sprint/Nextel 1400");
        //	if(ToneStr=="AnsweringMachine AT&T/Cingular 1700");
        //	if (ToneStr == "FAX") ;
        //}
        //}


        private void activeConnListbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Get sel connection
            int connectionId;
            if (GetSelectedConnection(out connectionId) != true) return;

            //Display form
            ContributionForm dlg = new ContributionForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            //Modify
            AbtoPhone.SetConnectionContribution(connectionId, dlg.m_inputGain, dlg.m_outputGain);
        }
        private void sendTextBtn_Click(object sender, EventArgs e)
        {
            //Display form
            SendTextForm dlg = new SendTextForm();
            dlg.address = AddressBox.Text;

            //Check return result - send message
            if ((dlg.ShowDialog(this) == DialogResult.OK) &&
               (dlg.address.Length != 0) && (dlg.message.Length != 0))
                AbtoPhone.SendTextMessage(dlg.address, dlg.message, 0);
        }
        private void unregisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AbtoPhone.Config.RegExpire = 0;
            this.AbtoPhone.ApplyConfig();
        }
        private void startCallDurationTimer(LineInfo lnInfo)
        {
            if (lnInfo.m_callDurationTimer == null)
            {
                lnInfo.m_callDurationTimer = new System.Windows.Forms.Timer();
                lnInfo.m_callDurationTimer.Tick += new EventHandler(OnCallDurationTimerEvent);
                lnInfo.m_callDurationTimer.Tag = lnInfo.m_id;
                lnInfo.m_callDurationTimer.Interval = 1000;
            }

            lnInfo.m_callTime = new TimeSpan(0, 0, 0);
            lnInfo.m_callTimeStr = "Çagrı Süresi: 00:00:00";

            lnInfo.m_callDurationTimer.Start();
        }
        private void OnCallDurationTimerEvent(object sender, EventArgs e)
        {
            //Get timer
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            if (sender == null) return;
            int lineId = (int)timer.Tag;

            //Get line info
            LineInfo lnInfo = GetLine(lineId);
            if (lnInfo == null) return;

            //Increment duration
            lnInfo.m_callTime = lnInfo.m_callTime.Add(new TimeSpan(0, 0, 1));
            lnInfo.m_callTimeStr = "Çagrı Süresi: " + lnInfo.m_callTime.ToString();

            //Display current duration
            if (lineId == m_curLineId) callDurationLabel.Text = lnInfo.m_callTimeStr;
        }
        //Play/Record buf source code example
        private void playBufToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TestPlayBuf t = new TestPlayBuf();
            //AbtoPhone.PlayBuffer(ref t.buf[0], t.buf.Length, 8000);

            //		long subscriptionId = AbtoPhone.Subscriptions.SubscribeCustomEvent("call-info", "150");
        }
        private void SIPPhoneForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //for (int i = 0; i < ViewCallList.RowCount; i++)
            //{
            //    Delete(ViewCallList.GetRowCellValue(i, "CURID").ToString());
            //}
        }

        private void checkVoiceMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Subscribe for events
            int subscriptionId = AbtoPhone.Subscriptions.SubscribeMessageSummary();
        }

        void AbtoPhone_OnSubscribeStatus(int subscriptionId, int statusCode, string statusMsg)
        {
            string str = string.Format("OnVoiceMail: Not supported. StatusCode: {0}", statusCode);
            displayNotifyMsg(str);
        }
        void AbtoPhone_OnSubscriptionNotify(int subscriptionId, string StateStr, string NotifyStr)
        {
            string str = string.Format("OnVoiceMail: {0}", StateStr);
            displayNotifyMsg(str);
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();

        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Get cfg values
            CConfig phoneCfg = AbtoPhone.Config;

            //Create and show dlg
            ConfigForm cfgDlg = new ConfigForm();
            cfgDlg.phoneCfg = phoneCfg;
            cfgDlg.m_defLoadStorePath = AbtoPhone.SDKPath();
            cfgDlg.m_versionStr = AbtoPhone.RetrieveVersion();
            cfgDlg.m_addrStr = AbtoPhone.RetrieveExternalAddress();

            if (cfgDlg.ShowDialog(this) != DialogResult.OK)
            {
                AbtoPhone.CancelConfig();
                return;
            }

            //Apply new values
            AbtoPhone.ApplyConfig();

            //Store
            phoneCfg.Store(cfgFileName);

            //Get state
            m_MP3RecordingEnabled = phoneCfg.MP3RecordingEnabled != 0;
            m_AutoAnswerEnabled = phoneCfg.AutoAnswerEnabled != 0;

        }
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AboutForm dlg = new AboutForm();
            dlg.ShowDialog(this);
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ActivationForm dlg = new ActivationForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            //Store cfg values
            CConfig phoneCfg = AbtoPhone.Config;
            phoneCfg.LicenseUserId = dlg.m_licUserId;
            phoneCfg.LicenseKey = dlg.m_licKey;
            phoneCfg.Store(cfgFileName);

            //Message box
            MessageBox.Show("Etkinleştirmeyi sonlandırmak için uygulamayı yeniden başlatın.");

        }
        string _magaza = "";
        string _tarih = string.Format(DateTime.Now.ToString());
        DateTime _Folderdate = DateTime.Now;
        string pass = "admin";
        string uys = "Madam1367";
        string Dosya = "";
        string url = "ftp://212.174.235.106:1025/";
        string local = "ftp://192.168.4.19/";
        string folder = DateTime.Now.ToString();

        public void CreateFolderFTP()
        {
            try
            {
                string path = _magaza;
                string xmlPath = url + path;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(xmlPath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(pass, uys);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                displayNotifyMsg(ex.Message);
            }
        }
        public void CreateFolderFTP2(string Klasor)
        {
            try
            {
                string xmlPath = url + Klasor;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(xmlPath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(pass, uys);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                displayNotifyMsg(ex.Message);
            }
        }
        public void FtpTransfer(string fullPath)
        {
            CreateFolderFTP();            
            string path = _magaza.Replace(" ", "") + "/Arama Ses Kayıtları/";

            //CreateFolderFTP2("/Arama Ses Kayıtları/");
            CreateFolderFTP2(path);
            if (fullPath == "")
            {
                string userFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                //for (int i = 0; i < ViewCallList.RowCount; i++)
                //{
                //    Fullpath = userFilePath + "\\Music\\" + ViewCallList.GetRowCellValue(i, "Destination").ToString() + ".mp3";
                //}
            }
            string From = path + "/" + Path.GetFileName(fullPath);
            string To = url + From;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url + path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(pass, uys);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string fileData = reader.ReadToEnd();
            string[] files = fileData.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int fileCount = files.Length;

            reader.Close();
            response.Close();

            int adet = 0;
            adet = fileCount;
            adet = adet + 1;

            string newName = "";
            string s1 = fullPath;
            string s2 = "mp3";

            bool b = s1.Contains(s2);

            if (b)
            {
                if (fileCount == 0)
                {
                    newName = Dosya+"_"+folder + "_1.mp3";
                }
                else
                {
                    newName = Dosya + "_" + adet + ".mp3";
                }
            }

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(pass, uys);
                client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, fullPath);
                //client.UploadFileAsync(new Uri(To), newName, fullPath);


                FtpWebRequest FTP;
                try
                {
                    FTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(To));
                    FTP.UseBinary = true;
                    string yeniisim = newName;
                    FTP.RenameTo = yeniisim;
                    FTP.Credentials = new NetworkCredential(pass, uys);
                    FTP.Method = WebRequestMethods.Ftp.Rename;
                    FtpWebResponse response2 = (FtpWebResponse)FTP.GetResponse();
                    displayNotifyMsg(response2.StatusDescription);
                }
                catch (Exception ex)
                {
                    displayNotifyMsg(ex.Message);
                }


            }

        }
        string Fullpath = "";
        int connId = 0;
        
        void Record(string Destination)
        {
            LineInfo lnInfo = GetCurLine();
            if (m_lineIdWhereRecStarted != 0)
            {
                AbtoPhone.StopRecording();
                buttonRecordStartStop.Text = "Kaydet";
                displayNotifyMsg("Kayıt Durduruldu");
                m_lineIdWhereRecStarted = 0;
                buttonRecordStartStop.Enabled = true;
            }
            else
            {
                string userFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                Fullpath = userFilePath+"\\Music\\" + Destination.ToString() + ".mp3";
                AbtoPhone.StartRecording(Fullpath);
                buttonRecordStartStop.Text = "Kayıt Yapılıyor";
                buttonRecordStartStop.Enabled = false;
                displayNotifyMsg("Kayıt file: " + Fullpath);
                //for (int i = 0; i < ViewCallList.RowCount; i++)
                //{
                //    ViewCallList.SetRowCellValue(i, "Status", Fullpath);
                //    ViewCallList.RefreshRow(i);
                //}
                m_lineIdWhereRecStarted = m_curLineId;
            }
        }
        
        private void PhoneForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SIPPhoneForm_FormClosing(null,null);
        }
        public class Root
        {
            public string result { get; set; }
            public string description { get; set; }
        }        
        private void buttonJoin_Click(object sender, EventArgs e)
        {
            //Get line Id
            JoinForm dlg = new JoinForm();
            dlg.m_curLineId = m_curLineId;
            dlg.m_selLineId = 0;
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            //Check same line
            if (dlg.m_selLineId == m_curLineId) return;

            //Check selected line
            LineInfo selLineInfo = GetLine(dlg.m_selLineId);
            LineInfo curLineInfo = GetCurLine();
            if (!selLineInfo.m_bCallEstablished) return;

            //Append
            foreach (ConnectionInfo it in selLineInfo.m_conn) curLineInfo.m_conn.Add(it.Key, it.Value);
            selLineInfo.m_conn.Clear();

            //Displ
            DisplayConnectionsAll(curLineInfo);

            //Join
            AbtoPhone.JoinToCurrentCall(dlg.m_selLineId);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (number != "")
            {
                string last;
                last = AddressBox.Text;
                AddressBox.Text = number;
                number = "";
            }
        }
    }
}
