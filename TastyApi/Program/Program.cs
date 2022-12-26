using Newtonsoft.Json;
using Model;
using MySql.Data.MySqlClient;

namespace Controller
{
    class Api_Handler
    {
        static string cs = @"server=localhost;userid=ryan;password=7624;database=CookingSite";

        public static async Task Main(string[] args)
        {

            int start = 0;
            int size = 10;
            bool run = true;
            int total = 0;

            HttpClient client = new HttpClient();

            while (run)
            {


                string requestUrl = $"https://tasty.p.rapidapi.com/recipes/list?from={start}&size={size}";
                System.Console.WriteLine(requestUrl);

                var request = Api_helper.GetRequest(requestUrl);

                string body = await Api_helper.GetBody(request, client);
                Root? dBody = Api_helper.Deserialize(body);

                if (dBody == null)
                {
                    System.Console.WriteLine("error occured");
                }
                else
                {
                    total = dBody.count;


                    foreach (var result in dBody.results)
                    {
                        

                    }
                }

                run = false;
            }

        }

        public static bool AddTags(List<Tag> tags, ref MySqlCommand? cmd)
        {
            if(cmd is null){
                System.Console.WriteLine("cmd is Null");
                return false;
            }

            foreach (Tag tag in tags)
            {
                cmd.CommandText = "INSERT INTO Tags(tID, tag, type) VALUES(@tID, @tag, @type)";
                cmd.Parameters.AddWithValue("@tID", tag.id);
                cmd.Parameters.AddWithValue("@tag", tag.name);
                cmd.Parameters.AddWithValue("@type", tag.type);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
            }

            return true;
        }
        public static void Database()
        {
            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                var stm = "SELECT VERSION()";
                var cmd = new MySqlCommand(stm, con);

                var version = cmd.ExecuteScalar().ToString();
                System.Console.WriteLine($"MySql {version} connection established!");

                cmd.CommandText = "INSERT INTO Tags(tag) VALUES('Asian')";
                cmd.ExecuteNonQuery();
            }
        }
    }
    class Api_helper
    {

        public static Root? Deserialize(string body)
        {
            Root? res = null;
            try
            {
                res = JsonConvert.DeserializeObject<Root>(body);

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.StackTrace);
            }

            return res;
        }

        public static async Task<string> GetBody(HttpRequestMessage request, HttpClient client)
        {
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }

        public static HttpRequestMessage GetRequest(string url)
        {

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", "0dc192d3f6msh5b118f05ffaf328p11ed02jsne2793d627b9c" },
                    { "X-RapidAPI-Host", "tasty.p.rapidapi.com" },
                },
            };
            return request;
        }
    }
}