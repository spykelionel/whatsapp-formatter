using System.Globalization;
using System.Text.RegularExpressions;

namespace Extractor
{
    public class Program
    {
        public static List<ExtractedMessage> ExtractMessagesFromWhatsAppExportedDiscussion(string exportedDiscussion, List<MyContact>? userContacts)
        {
            List<ExtractedMessage> extractedMessages = new List<ExtractedMessage>();
            string[] messages = File.ReadAllLines(exportedDiscussion);
            Console.WriteLine(messages.Length);
            // extract the message part here.
            // Regex to extract parts of the message.
            /// sample date parsing here.
            DateTime dateSent = DateTime.Parse("2/2/22 12:22", CultureInfo.InvariantCulture);
            string testMultiline = @"^([a-zA-Z])?\s?";
            string timeReg = @"([0-1]?[0-9]|2[0-3]):[0-5][0-9]";
            string dateReg = @"^([0-9]*)/([0-9]*)/([0-9]*)";
            string senderReg = @"\+[0-9]+\s?[0-9]+\s?[0-9]+\s?[0-9]+\s?[0-9]+\s?[0-9]+\s?[0-9]+\s?[0-9]+\s?";
            string senderReg2 = @"\-\s[a-zA-Z]+\s?\s?[a-zA-Z]+\s?";

            var regexTime = new Regex(timeReg);
            var regexDate = new Regex(dateReg);
            var regexSender = new Regex(senderReg);
            var regexSender2 = new Regex(senderReg2);
            var mutlineRegex = new Regex(testMultiline);

            string messageBody = "";
            for (int i = 0; i < messages.Length; i++)
            {
                
                var time = regexTime.Match(messages[i].Trim());
                var date = regexDate.Match(messages[i].Trim());
                var sender = regexSender.Match(messages[i].Trim());
                var sender2 = regexSender2.Match(messages[i].Trim());
                var multilineMsg = mutlineRegex.Match(messages[i].Trim());
                string[] body = messages[i].Trim().Split(new string[] { ": " }, StringSplitOptions.None);

                if (i == messages.Length - 1)
                {
                    // since i checks +1 ahead, it might not have checked the last message item.
                    int ln = extractedMessages.Count;

                    if (multilineMsg.Success || !date.Success)
                    {
                        try
                        {
                            // Console.WriteLine("Before..... {0}", extractedMessages[ln - 1].MessageBody);
                            extractedMessages[ln - 1].MessageBody += "\n" + messages[i];

                        }
                        catch
                        {
                            // Handle IndexOutOfBounds exception here.
                        }


                    }
                }
                if (i < messages.Length - 1)
                {
                    int ln = extractedMessages.Count;
                    // check if the next message contains an > messages[i+1]
                    multilineMsg = mutlineRegex.Match(messages[i + 1].Trim());

                    date = regexDate.Match(messages[i + 1].Trim());
                    if (multilineMsg.Success || !date.Success)
                    {
                        try
                        {
                            // Console.WriteLine("Before..... {0}", extractedMessages[ln - 1].MessageBody);
                            extractedMessages[ln - 1].MessageBody += "\n" + messages[i + 1];
                            // Console.WriteLine("After..... {0}", extractedMessages[ln - 1].MessageBody);

                        }
                        catch
                        {
                            // Handle IndexOutOfBounds exception here.
                        }

                    }
                    // match the current message for the date.
                    date = regexDate.Match(messages[i].Trim());
                    if (date.Success && body != null && messages[i].Length > 0)
                    {
                        try
                        {
                            messageBody = body[1];
                        }
                        catch
                        {
                            //Console.WriteLine(" See it failed here {0} becuae", messages[i].Length);
                            messageBody = messages[i].Trim().Split(new string[] { "- " }, StringSplitOptions.None)[1];
                        }
                        var dateString = $"{date} {time}";
                        // Console.WriteLine(dateString);
                        dateSent = DateTime.Parse(dateString, CultureInfo.InvariantCulture);

                        extractedMessages.Add(new ExtractedMessage(dateSent, sender.Success ? sender.ToString().Length < 5 || sender.ToString().Trim() == "-" || sender.ToString().Trim().StartsWith(' ') ? "GROUP": sender.ToString()  : sender2.ToString(), messageBody));
                    }
                }

                if (!date.Success && extractedMessages.Count > 0 && body == null)
                {
                    int ln = extractedMessages.Count;
                    messageBody = messageBody + "\n" + messages[i].Trim();
                    extractedMessages[ln].MessageBody += "\n" + messages[i];

                }


                else if (date.Success && body == null)
                {
                    Console.WriteLine("No body {0}", messages[i]);
                }
            }

            return extractedMessages;
        }

        public static void Main()
        {
            string text = "The format of the timestamp is always yyyy-MM-dd HH:mm:ss, like for example 2022-05-20 10:21:28";

            string date = GetDateFromText(text);
            Console.WriteLine(date);
        }


        // <summary>
        //     Searches the specified input string for the first occurrence of the regular expression
        //
        // Parameters:
        //   text:
        //     The string to search for a match.
        //
        // Returns:
        //     An string that contains information about the match if the was match found, else 
        //     No Match Found
        //  
        // Runtime Analysis:
        //     Worst-case: n + 14
        // References:
        //     https://github.com/microsoft/referencesource/blob/master/System/regex/system/text/regularexpressions/Regex.cs
        //</summary>
        public static string GetDateFromText(string text)
        {
            string dateTimeReg = @"([0-9]*)-([0-9]*)-([0-9]*)+[\s][0-9]*:[[0-9]*:[0-9]*";
            var dateTime = new Regex(dateTimeReg);
            var result = dateTime.Match(text); // t = n * number of elements
            /// DateTime date = new DateTime().Date;
            string msg;
            if (result.Success)
            {
                msg = $"Matched Found: {result}";
                //  date = DateTime.Parse(result.ToString(), CultureInfo.InvariantCulture);
            }
            else msg = "No match found";
            
            return msg;
        }
    }

}