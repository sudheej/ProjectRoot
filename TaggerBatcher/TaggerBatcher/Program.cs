using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace TaggerBatcher
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class TaggerBatcher
    {
        static void Main(string[] args)
        {
            //InvokeRequestResponseService().Wait();
            readSentiTable();
            Console.ReadLine();
        }

        static void readSentiTable()
        {
            try
            {
                string connectionString = "Data Source=INFCHSZA2474;Initial Catalog=ROOT;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand myCommand = new SqlCommand("select * from dbo.Sentiment", conn);
                SqlDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {  // <<- here
                   //Console.WriteLine(myReader["Username"].ToString());
                   //Console.WriteLine(myReader["Item"].ToString());
                   //Console.WriteLine(myReader["Amount"].ToString());
                   //Console.WriteLine(myReader["Complete"].ToString());
                    updateDatabase(Convert.ToInt64(myReader["SentimentTagId"]), myReader["Email"].ToString(), myReader["SentimentText"].ToString());
                    Thread.Sleep(5000);

                }  // <<- here

            }
            catch (SqlException ex)
            {
                //Log exception
                //Display Error message
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }

        static async void updateDatabase(long TagId, string email, string SentimentText)
        {
            //Console.WriteLine(date + " " + clientname + " " + email + " " + sentimenttext + " " );
            string tags = "";
            TaggerBatcher s = new TaggerBatcher();
            tags = await s.getTags(SentimentText);

            /*var c = new JsonSerializer();
            var a = new { Output1 = "", data = new object[] { } };
            dynamic jsonObject = c.Deserialize(new StringReader(sentimentvalue), a.GetType());
            Console.WriteLine(jsonObject.data[0]);*/
            JObject o = JObject.Parse(tags);
            //(string)o.SelectToken("Manufacturers[0].Name");

            //Console.WriteLine(sentimentvalue);
            string tempData = "";
            try
            {

                for (int i = 0; i < 300; i++)
                {
                    string word = (string)o.SelectToken("$.Results.output1.value.Values[" + i + "][0]").ToString();
                    string type = (string)o.SelectToken("$.Results.output1.value.Values[" + i + "][1]").ToString();
                    if (type.Equals("NNP") || type.Equals("NN") || type.Equals("RB") || type.Equals("CD"))
                    {
                        //Console.WriteLine(word);
                        tempData = tempData + word + ",";
                    }

                }
            }
            catch
            {

            }




            Console.WriteLine(SentimentText);

            try
            {
                string connectionString = "Data Source=INFCHSZA2474;Initial Catalog=ROOT;Integrated Security=True";
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("INSERT INTO dbo.Tagger VALUES(" +
                            "@TagId, @Email, @Tags)", conn))
                    {
                        cmd.Parameters.AddWithValue("@TagId", TagId);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Tags", tempData);
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

        public async Task<string> getTags(string message)
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
                const string apiKey = "2Kk2UzKp1lozegly0hJ7O6Zu7ZfQ+inaFONBvACifu+SoNLZ2/7jxjYge63D7gZf0XPqc9IkqYXpFSw/rJ6kGQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/551ddef5909d4c668188f6235ed793ec/services/06b932f3ef7f41e5a00dbe2265f5fce2/execute?api-version=2.0&details=true");

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