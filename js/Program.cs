using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace FaceDetection
{

    //Path: /Users/Rohit/Downloads/RCMsmile2016-700x748.jpg
    //Path2: /Users/Rohit/Downloads/scaredPhoto.jpg
    //Path3: /Users/Rohit/Downloads/sad.jpg
    //Path4: /Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/ScreenshotPractice/14 year old boy.jpg
    class Program
    {
        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "24e59ef6bca9430681c3134e14d2e5d5";
        const string uriBase =
            "https://eastus.api.cognitive.microsoft.com/face/v1.0/detect";
        static void Main(string[] args)
        {
            // Get the path and filename to process from the user.
            Console.WriteLine("Detect faces:");
            Console.Write(
                "Enter the path to an image with faces that you wish to analyze: ");
            string imageFilePath = Console.ReadLine();

            //string imageFilePath = "/Users/Rohit/Downloads/RCMsmile2016-700x748.jpg";

            if (File.Exists(imageFilePath))
            {
                try
                {
                    MakeAnalysisRequest(imageFilePath);
                    //Console.WriteLine("\nWait a moment for the results to appear.\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n" + e.Message + "\nPress Enter to exit...\n");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid file path.\nPress Enter to exit...\n");
            }
            Console.ReadLine();
        }

        // Gets the analysis of the specified image by using the Face REST API.
        static async void MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            string requestParameters = 
                "returnFaceAttributes=emotion";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                //Console.WriteLine("\nResponse:\n");
                //Console.WriteLine(JsonPrettyPrint(contentString));
                //Console.WriteLine("\nPress Enter to exit...");
                saveTheEmotionVals(contentString);
            }
        }


        // 0 - anger, 1 - contempt, 2 - disgust, 3 - fear, 4 - happy, 5 - neutral, 6 - sadness, 7 - surprise
        private static void saveTheEmotionVals(String response)
        {
            int[] emotionVals = new int[8];
            int ind = response.IndexOf("anger");
            int end = response.Length - ind - 4;
            response = response.Substring(ind, end);
            int index = 0;
            while(response.Contains(":")){
                emotionVals[index] = (int)(decimal.Parse(response.Substring(response.IndexOf(":") + 1, 3)) * 1000);
                int yeet = response.IndexOf(",") + 1;
                //Console.WriteLine(yeet);
                if (yeet == 0)
                {
                    response = "";
                }
                else
                {
                    response = response.Substring(yeet);
                }
                index++;
                //Console.WriteLine(response);
            }
            //foreach (int a in emotionVals)
            //{
            //    Console.WriteLine(a);
            //}
            doStuffWithEmotions(emotionVals);
        }





        //CHANGE THIS TO MATCH REQUIREMENTS
        private static void doStuffWithEmotions(int[] emotionVals)
        {
            int greatest = 0;
            int inc = 0;
            int ind = 0;

            foreach(int i in emotionVals){
                if(i > greatest)
                {
                    greatest = i;
                    ind = inc;
                }
                inc++;
            }

            if (greatest == 0)
                greatest = -1;

            if (ind == 4)//HAPPY
            {
                string text = File.ReadAllText("/Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/python/txtfiles.txt");
                text = "HAPPY";
                File.WriteAllText("/Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/python/txtfiles.txt", text);
                //Console.WriteLine("happy");
            }
            else if (ind == 3 || ind == 7) // FEAR
            {
                string text = File.ReadAllText("/Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/python/txtfiles.txt");
                text = "FEARR";
                File.WriteAllText("/Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/python/txtfiles.txt", text);
                //Console.WriteLine("afraid");
            }
            else if (ind == 6) // SADNESS
            {
                string text = File.ReadAllText("/Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/python/txtfiles.txt");
                text = "SADDD";
                File.WriteAllText("/Users/Rohit/Documents/GitHub/HackTCNJEnhancedParentControl/python/txtfiles.txt", text);
                //Console.WriteLine("sad");
            }
        }

        // Returns the contents of the specified file as a byte array.
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        // Formats the given JSON string by adding line breaks and indents.
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}
}