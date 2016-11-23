// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SentiBatcher
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class SentiBatcher
    {
        static void Main(string[] args)
        {
            //InvokeRequestResponseService().Wait();
            readCSV();
            Console.ReadLine();
        }

        static void readCSV()
        {
            var reader = new StreamReader(File.OpenRead(@"data_sentibatcher.csv"));
            Boolean skipFlag = false;
            while (!reader.EndOfStream)
            {

                var line = reader.ReadLine();
                var values = line.Split(',');
                if (skipFlag == true)
                {
                    Thread.Sleep(2000);
                    updateDatabase(Convert.ToDateTime(values[0]), values[1], values[2], values[3]);
                }
                else
                {
                    skipFlag = true;
                }

            }

        }

        static async void updateDatabase(DateTime date, string clientname, string email, string sentimenttext)
        {
            //Console.WriteLine(date + " " + clientname + " " + email + " " + sentimenttext + " " );
            string sentimentvalue = "";
            SentiBatcher s = new SentiBatcher();
            sentimentvalue = await s.getSentiValues(sentimenttext);

            /*var c = new JsonSerializer();
            var a = new { Output1 = "", data = new object[] { } };
            dynamic jsonObject = c.Deserialize(new StringReader(sentimentvalue), a.GetType());
            Console.WriteLine(jsonObject.data[0]);*/
            JObject o = JObject.Parse(sentimentvalue);
            //(string)o.SelectToken("Manufacturers[0].Name");

            //Console.WriteLine(sentimentvalue);

            string sentimentType = (string)o.SelectToken("$.Results.output1.value.Values[1][0]").ToString();
            float sentimentValue = (float)o.SelectToken("$.Results.output1.value.Values[1][1]");

            Console.WriteLine(sentimentType);
            Console.WriteLine(sentimentvalue);
            try
            {
                string connectionString = "Data Source=INFCHSZA2474;Initial Catalog=ROOT;Integrated Security=True";
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("INSERT INTO dbo.Sentiment VALUES(" +
                            "@Email, @SentimentText, @SentimentType,@SentimentScore,@DateUpdated)", conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@SentimentText", sentimenttext.ToString().Replace('"', ' ').Trim());
                        cmd.Parameters.AddWithValue("@SentimentType", sentimentType);
                        cmd.Parameters.AddWithValue("@SentimentScore", sentimentValue);
                        cmd.Parameters.AddWithValue("@DateUpdated", date);
                        int rows = cmd.ExecuteNonQuery();

                        //rows number of record got inserted
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log exception
                //Display Error message
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }

        public async Task<string> getSentiValues(string message)
        {
            string messageResult = await InvokeRequestResponseService(message);
            return messageResult;

        }


        public async Task<string> InvokeRequestResponseService(string message)
        {
            string resultset;
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"review_text"},
                                Values = new string[,] {  { "value" },  { message },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "s/oTf0asCg1WbCDD3Nj8U/myyriT7k6jyG1K99CPYAaOH9nxawnu39yvABItubQvaRnErkB+/hkGqfVD8D+lmQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/551ddef5909d4c668188f6235ed793ec/services/0f6caaaafae840f5bb4e5d6c4b80d9ed/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("Result: {0}", result);
                    resultset = result;
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine(responseContent);
                    resultset = responseContent;
                }

            }
            return resultset;
        }
    }
}