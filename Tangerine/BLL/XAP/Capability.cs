using System;
using System.Collections.Generic;
using System.Linq;

namespace Tangerine.BLL
{
    public sealed class Capability
    {
        private static List<Capability> m_capabilities = new List<Capability>();

        public string Id { get; private set; }

        public string Description { get; set; }

        static Capability()
        {
            m_capabilities.Add(new Capability("ID_CAP_APPOINTMENTS", "Applications that access appointment data."));
            m_capabilities.Add(new Capability("ID_CAP_CAMERA", "Applications that use the camera capabilities."));
            m_capabilities.Add(new Capability("ID_CAP_CONTACTS", "Applications that access contact data."));
            m_capabilities.Add(new Capability("ID_CAP_GAMERSERVICES", "Applications that can interact with Xbox LIVE APIs."));
            m_capabilities.Add(new Capability("ID_CAP_IDENTITY_DEVICE", "Applications that use device-specific information such as a unique device ID, manufacturer name, or model name."));
            m_capabilities.Add(new Capability("ID_CAP_IDENTITY_USER", "Applications that use the anonymous LiveID to uniquely identify the user in an anonymous fashion."));
            m_capabilities.Add(new Capability("ID_CAP_ISV_CAMERA", "Applications that use the primary or front-facing camera."));
            m_capabilities.Add(new Capability("ID_CAP_LOCATION", "Applications with access to location services."));
            m_capabilities.Add(new Capability("ID_CAP_MEDIALIB", "Applications that can access media library."));
            m_capabilities.Add(new Capability("ID_CAP_MICROPHONE", "Applications that use the microphone."));
            m_capabilities.Add(new Capability("ID_CAP_NETWORKING", "Applications with access to network services."));
            m_capabilities.Add(new Capability("ID_CAP_PHONEDIALER", "Applications that can place phone calls."));
            m_capabilities.Add(new Capability("ID_CAP_PUSH_NOTIFICATION", "Applications that can receive push notifications from an Internet service."));
            m_capabilities.Add(new Capability("ID_CAP_SENSORS", "Applications that use the Windows Phone sensors."));
            m_capabilities.Add(new Capability("ID_CAP_WEBBROWSERCOMPONENT", "Applications with access to network services."));
            m_capabilities.Add(new Capability("ID_HW_FRONTCAMERA", "Applications that have features which require the front-facing camera."));
        }

        Capability(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public static Capability GetCapability(string id)
        {
            return m_capabilities.Where(xapCap => xapCap.Id == id).FirstOrDefault();
        }
    }
}
