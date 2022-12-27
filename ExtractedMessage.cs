using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor
{
    public class ExtractedMessage
    {
        public DateTime DateSent;

        public string SenderPhoneNumber;

        public string MessageBody;

        public ExtractedMessage(DateTime dateSent, string senderPhoneNumber, string messageBody)
        {
            DateSent = dateSent;
            SenderPhoneNumber = senderPhoneNumber;
            MessageBody = messageBody;
        }
    }
    public class MyContact
    {
        public string FullName;

        public string PhoneNumber;
    }
    
}
