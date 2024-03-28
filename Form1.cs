using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teamsapi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        connection con;

        class model {
            
            

            public  model(int idx,string senderid,string destteam,string destchannel, string title,string msg) 
            {
                this.idx = idx;
                this.senderid = senderid;
                this.Destteam = destteam;
                this.destchannel = destchannel;
                this.title = title;
                Msg = msg;
            }

            public string Destteam { get; }
            public string title { get; }
            public string Msg { get; }
            public int idx { get; }
            public string senderid { get; }
            public string destchannel { get; }
        }

        List<model> modelList;

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                modelList = new List<model>();
                var sql = "";
                var dT = con.DataSelect(sql);
                var dR = dT.Rows;
                foreach ( DataRow r in dR )
                {
                    
                    var idx = Convert.ToInt16(r["idx"]?.ToString());
                    var title = r["title"]?.ToString();
                    var msg = r["msg"]?.ToString();
                    var senderid = r["senderid"]?.ToString();
                    var Destteam = r["Destteam"]?.ToString();
                    var destchannel = r["destchannel"]?.ToString();

                    modelList.Add(new model(idx, senderid, Destteam, destchannel, title , msg));
                }

                await Task.Factory.StartNew(() => sendMessage());

            }
            catch (Exception ex)
            {

            }
        }

        private async void sendMessage()
        {
            try
            {

                string webHookurl = "https://ibinfo.webhook.office.com/webhookb2/8b4a88f3-18d4-447b-8373-00283c231396@3ba25843-1c3e-4344-8015-7beca7b80483/IncomingWebhook/3b1e03e1f4c6436384df03d91790cf46/6ae48bb4-e0e8-497a-be40-a140606ffba3";

                //https://ibinfo.webhook.office.com/webhookb2/8b4a88f3-18d4-447b-8373-00283c231396@3ba25843-1c3e-4344-8015-7beca7b80483/IncomingWebhook/94926c88b4d64ac4883524d1dfdfb83a/69b84ae4-994c-40d2-ae82-d2949be2d396
                //
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                StringContent content = new StringContent("", Encoding.UTF8, "application/json"); ;
                foreach (model msg in modelList)
                {
                   var adaptiveCardJson =
                       "{ \"type\": \"message\", \"attachments\": [{\"contentType\":\"application/vnd.microsoft.card.adaptive\",\"content\":{\"type\":\"AdaptiveCard\",\"body\":[{\"type\":\"TextBlock\",\"text\":\"msg.message\"}]}}]}";

                   var Json = adaptiveCardJson.Replace("msg.message",$"{msg.title} \n {msg.Msg}");
                    //string serializeJson = JsonConvert.SerializeObject(adaptiveCardJson);
                    content = new StringContent(Json, Encoding.UTF8, "application/json");
                    _ = await client.PostAsync(webHookurl, content);
                    var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var sql = $"UPDATE omms.msg_interface_history SET send_time = '{time}' , issend = 1 " +
                        $"WHERE idx = {msg.idx}";
                    await Task.Factory.StartNew(() => con.DataExeCute(sql));                 
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            con = new connection();
            con.DbConnection();
        }
    }
}
