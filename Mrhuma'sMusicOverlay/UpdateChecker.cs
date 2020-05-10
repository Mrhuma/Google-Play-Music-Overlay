using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MrhumasMusicOverlay
{
    static class UpdateChecker
    {
        public static void CheckForUpdate()
        {
            //We set both variables to the same value in case of an error
            //If an error occurs it will use this value, and the popup won't appear for the user
            //Having no popup appear, even if a new version is available, is preferred to having a popup appear when it shouldn't
            string currentVersion = "v3.2", onlineVersion = "v3.2";

            using (var client = new HttpClient())
            {
                //Basic headers to access the information
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("MrhumasMusicOverlay", "3.2"));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //Store the response
                HttpResponseMessage response = client.GetAsync("https://api.github.com/repos/Mrhuma/Mrhumas-Music-Overlay/releases").Result;

                //If the request was successful
                if (response.IsSuccessStatusCode)
                {
                    JToken jo = JToken.Parse(response.Content.ReadAsStringAsync().Result);
                    onlineVersion = jo.First.SelectToken("tag_name").ToObject<string>();

                    //If there is an update
                    if (currentVersion != onlineVersion)
                    {
                        //Inform the user an update is available
                        //If the user clicks on the "Yes" button
                        if(MessageBox.Show("There is a new update available. Would you like to go to the download page?",
                            "Update Available",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            //Open the link to let the user download the update
                            System.Diagnostics.Process.Start("https://github.com/Mrhuma/Mrhumas-Music-Overlay/releases");
                        }
                    }
                }
            }
        }
    }
}
