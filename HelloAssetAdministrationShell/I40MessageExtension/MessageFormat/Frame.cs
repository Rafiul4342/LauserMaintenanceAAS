using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.I40MessageExtension.MessageFormat
{
    public class Frame
    {
        public SemanticProtocol semanticProtocol { get; set; }
        public string type { get; set; }
        public string messageId { get; set; }
        public Sender sender { get; set; }
        public Receiver receiver { get; set; }
        public string replyBy { get; set; }
        public string inReplyTO { get; set; }
        public string conversationId { get; set; }
    }
}
