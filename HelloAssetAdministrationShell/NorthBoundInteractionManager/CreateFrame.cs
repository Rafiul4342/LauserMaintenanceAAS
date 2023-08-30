using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloAssetAdministrationShell.I40MessageExtension.MessageFormat;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{

    public static class CreateFrame
    {

        public static Frame GetFrame(string ConversationID, int messageID, string Messagetype)
        {

            var frame = new Frame
            {
                type = Messagetype,
                sender = new Sender
                {
                    identification = new Identification
                    {


                        id = SendMaintenanceOrders.sender,
                        idType = "CUSTOM",


                    },
                    role = new Role
                    {
                        name = "InformationSender"
                    }

                },
                receiver = new Receiver
                {
                    identification = new Identification
                    {

                        id = "MES_AAS",
                        idType = "CUSTOM"
                    },
                    role = new Role
                    {
                        name = "informationReceiver"
                    }
                },
                conversationId = ConversationID,
                messageId = messageID,
                inReplyTO = null,
                replyBy = null,
                semanticProtocol = new SemanticProtocol
                {
                    keys = new List<Key>
                    {

                            new Key
                          {
                              type = "GlobalReference",
                              idType ="CUSTOM",
                              value ="Maintenance"
                          }

                    }
                }

            };
            return frame;
        }
    }
}