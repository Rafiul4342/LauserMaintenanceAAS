using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.I40MessageExtension.MessageFormat
{
    public class Frame
    {
        
        public string type { get; set; }
        public Sender sender { get; set; }
        public Receiver receiver { get; set; }
        public string conversationId { get; set; }
        public int messageId { get; set; }
        public string inReplyTO { get; set; }
        public string replyBy { get; set; }
        public SemanticProtocol semanticProtocol { get; set; }

        public string getType()
        {
            return type;
        }

        public void SetType(string Type)
        {
            this.type = Type;
        }
    }
}
