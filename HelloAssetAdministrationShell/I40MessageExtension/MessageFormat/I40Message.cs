using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.I40MessageExtension.MessageFormat
{
    public class I40Message
    {
        public Frame frame { get; set; }

        public List<SubmodelElementCollection> interactionElements { get; set; }

        public void SetInteractionElement(List<SubmodelElementCollection> interactionElements)
        {
            this.interactionElements = interactionElements;
        }

        public List<SubmodelElementCollection> GetInteracrionElement()
        {
            return interactionElements;
        }

        public void Setframe(Frame frame)
        {
            this.frame = frame;
        }

        public Frame GetFrame()
        {
            return this.frame;
        }

    }
}
